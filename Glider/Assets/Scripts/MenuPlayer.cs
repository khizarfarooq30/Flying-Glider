using UnityEngine;

public class MenuPlayer : MonoBehaviour {

    
    private void Update() {
        transform.position += 3 * Vector3.forward * Time.deltaTime;
    }
}