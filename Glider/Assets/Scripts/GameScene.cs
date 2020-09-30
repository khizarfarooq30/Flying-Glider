using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{

    private CanvasGroup fadeGroup;
    private float fadeInDuration = 2f;
    private bool isGameStarted;

    void Start(){ 
        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1;
    }

    private void Update() {
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
