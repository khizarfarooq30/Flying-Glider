using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; set; }

    public Color[] colors = new Color[10];
    public GameObject[] playerTrial = new GameObject[10];
    public Material playerMaterial;

    public int currentLevel = 0;    // used when changing from menu to game scene
    public int menuFocus = 0;       // Used when entering the menu scene

    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public Vector3 GetPlayerInput() {
        // first are we using accelerometer
        if(SaveManager.Instance.state.usingAccelerometer) {
            // if we can use it, replace the y param by the z, we don't need that y
            Vector3 a = Input.acceleration;
            a.y = a.z;
            return a;
        }

        // reading all touches from the user 
        Vector3 r = Vector3.zero;

        foreach(Touch touch in Input.touches) {
            // if we just started pressing on the screen then
            if(touch.phase == TouchPhase.Began) {
                activeTouches.Add(touch.fingerId, touch.position);
            }
            // if we remove the finger off the screen
            else if(touch.phase == TouchPhase.Ended) {
              if(activeTouches.ContainsKey(touch.fingerId))    
                activeTouches.Remove(touch.fingerId);
            }   
            // our finger is either moving or stationary, in both cases use delta
            else {
                float mag = 0;

                r = (touch.position - activeTouches[touch.fingerId]);
                mag = r.magnitude / 300;
                r = r.normalized * mag;
            }
        } 
        return r;
    }
}
