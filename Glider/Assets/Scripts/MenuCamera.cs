using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 desirePosition;
    private Quaternion desiredRotation;

    public Transform shopWaypoint;
    public Transform levelWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = desirePosition = transform.localPosition;
        startRotation = desiredRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Manager.Instance.GetPlayerInput().x;

        transform.localPosition = Vector3.Lerp(transform.localPosition, desirePosition + new Vector3(0, x, 0) * 0.01f, 0.1f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, 0.1f);   
    }


    public void BackToMainMenu() 
    {
        desirePosition = startPosition;
        desiredRotation = startRotation;
    }

    public void MoveToShop()
    {
        desirePosition = shopWaypoint.localPosition;
        desiredRotation = shopWaypoint.localRotation;
    }

    public void MoveToLevel() 
    {
        desirePosition = levelWaypoint.localPosition;
        desiredRotation = levelWaypoint.localRotation;
    }

}
