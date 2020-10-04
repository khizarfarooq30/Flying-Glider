using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
   private List<Transform> rings = new List<Transform>();

   public Material activeRing;
   public Material inActiveRing;
   public Material finalRing;
   
   private int ringPassed = 0;

   public Transform GetCurrentRing() {
       return rings[ringPassed];
   }

   private void Start() 
   {

       FindObjectOfType<GameScene>().objective = this;

       // at the start of the level, assign inactive to all the rings
       foreach (Transform t in transform)
       {
            rings.Add(t);
           t.GetComponent<MeshRenderer>().material = inActiveRing;
       }

       // Making sure that we have some rings
       if(rings.Count == 0) 
        {
            Debug.Log("there is no objective in the level, make sure you put some rings");

            return;
        }

        // active the first ring

        rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
   }

   public void NextRing() 
   {
       // play some fx on the current ring
       // ??

       // up the int value 
       ringPassed++;

       // if this is the last ring, let's call the victory function

       if(ringPassed == rings.Count) {
           Victory();
       }


       // if this is the previous last, give the next ring as final ring
       if(ringPassed == rings.Count - 1) {
           rings[ringPassed].GetComponent<MeshRenderer>().material = finalRing;
       } else {
           rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
       }

        // in both cases we need to active the ring 
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
   }

   private void Victory() 
   {
       FindObjectOfType<GameScene>().CompleteLevel();
   }
    
}
