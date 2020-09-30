using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    private Objective objectiveScript;
    private bool ringIsActive = false;

    private void Start() {
       objectiveScript = FindObjectOfType<Objective>(); 
    }

    public void ActivateRing() {
        ringIsActive = true;
    }

    private void OnTriggerEnter(Collider other) 
    {
        // if the ring is active
        // tell the objective script that it is collected

        if(ringIsActive) {
            objectiveScript.NextRing();
        }
    }
}
