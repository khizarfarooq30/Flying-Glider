using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private float loadTime;
    private float minimumTimeToLoadScene = 3f;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = FindObjectOfType<CanvasGroup>();

        canvasGroup.alpha = 1;

        if(Time.time < minimumTimeToLoadScene) {
            loadTime = minimumTimeToLoadScene;
        } else {
            loadTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time < minimumTimeToLoadScene) {
            canvasGroup.alpha = 1 - Time.time;
        }

        if(Time.time > minimumTimeToLoadScene && loadTime != 0) {
            canvasGroup.alpha = Time.time - minimumTimeToLoadScene;
            if(canvasGroup.alpha >= 1) {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
