using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{

    private CanvasGroup fadeGroup;
    private float fadeInDuration = 2f;
    private bool isGameStarted;

    public Transform arrow;
    private Transform playerTransform;
    public Objective objective;

    void Start(){  

         playerTransform = FindObjectOfType<PlayerMotor>().transform;

        SceneManager.LoadScene(Manager.Instance.currentLevel.ToString(), LoadSceneMode.Additive);

        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1;
    }

    private void Update() {

        if(objective != null) {
            // if we have an objective 
            
            Vector3 dir = playerTransform.InverseTransformPoint(objective.GetCurrentRing().position);
            float a = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            a += 180;
            arrow.transform.localEulerAngles = new Vector3(0,180,a);
        }

        if(Time.timeSinceLevelLoad <= fadeInDuration) 
        {
            fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);
        } 
        else if(!isGameStarted) 
        {
            isGameStarted = true;
            fadeGroup.alpha = 0;
        }
    }

    public void CompleteLevel() {
        // put the logic to complete the level here
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel);

        // focus the level selection when return to the menu scene

        Manager.Instance.menuFocus = 1;

        ExitScene();
    }

    public void ExitScene() {
        SceneManager.LoadScene("Menu");
    }
}
