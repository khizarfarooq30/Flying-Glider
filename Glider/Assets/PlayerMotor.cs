using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
     
    private float baseSpeed = 10.0f;
    private float rotationSpeedX = 3.0f;
    private float rotationSpeedY = 1.5f;

    private void Start() {
        controller = GetComponent<CharacterController>();

        // create the trial 
        GameObject trial = Instantiate(Manager.Instance.playerTrial[SaveManager.Instance.state.activeTrial]);
        // set the trial as the children of model
        trial.transform.SetParent(transform.GetChild(0));
        
        // Fix the rotation of the trial
        trial.transform.localEulerAngles = Vector3.forward * -90f;
    }

    private void Update() {
        // give player the forward velocity
        Vector3 moveVector = transform.forward * baseSpeed;

        // gather player input
        Vector3 inputs = Manager.Instance.GetPlayerInput();
        
        // Get the delta direction
        Vector3 yaw = inputs.x * transform.right * rotationSpeedX * Time.deltaTime;
        Vector3 pitch = inputs.y * transform.up * rotationSpeedY * Time.deltaTime;
        Vector3 direction = yaw + pitch;

        //make sure we limit the player from doing the loop
        float maxX = Quaternion.LookRotation(moveVector + direction).eulerAngles.x;

        // if he's not going too far up or down, add direction to the moveVector

        if(maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290) {
            // too far don't do anything
        } else {
            // add the direction to the current move

            moveVector += direction;

            // have the player facing where he's going
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        controller.Move(moveVector * Time.deltaTime);
    }
}
