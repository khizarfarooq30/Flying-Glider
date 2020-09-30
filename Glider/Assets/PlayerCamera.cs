using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
   public Transform lookAt;

   private Vector3 desirePosition;
   private float offset = 1.5f;
   private float distance = 3.5f;

   private void Update() {
       // update the pos
        desirePosition = lookAt.position + (-transform.forward * distance) + (transform.up * offset);
        transform.position = Vector3.Lerp(transform.position, desirePosition, 0.05f);

       // update the rotation
       transform.LookAt(lookAt.position + (Vector3.up * offset));
   }
}
