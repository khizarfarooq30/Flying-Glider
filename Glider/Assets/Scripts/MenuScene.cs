using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{   
    private CanvasGroup fadeGroup;
    private float fadeSpeed = 0.33f;

    public RectTransform menuContainer;

    public Transform levelPanel;
    public Transform colorPanel;
    public Transform trialPanel;

    public Button tiltControlButton;
    public Color tiltControlEnabled;
    public Color tiltControlDisabled;

    private int[] colorCost = new int[] {0, 5, 5, 5, 10, 10, 15, 15, 10, 10};
    private int[] trialCost = new int[] {0, 20, 20, 40, 40, 60, 60, 80, 80, 100};
    private int selectedColorindex;
    private int selectedTrialIndex;
    private int activeColorIndex;
    private int activeTrialIndex;

    public Text colorBuySetText; 
    public Text trialColorBuySetText;
    public Text goldText;

    private Vector3 desiredMenuPosition;
    private GameObject currentTrial;

    public AnimationCurve enteringLevelZoomCurve;
    private bool isEnteringLevel = false;
    private float zoomDuration = 3f;
    private float zoomTransition;

    private MenuCamera menuCamera;

    private Texture previousTrial;
    private GameObject lastPreviewObject;

    public Transform trialPreviewObjet;
    public RenderTexture trialPreviewTexture;
    

    // Start is called before the first frame update
    void Start() 
    {
   
        //check if we have a accelorometer
        if(SystemInfo.supportsAccelerometer) {
            // it is currently enabled
            tiltControlButton.GetComponent<Image>().color = SaveManager.Instance.state.usingAccelerometer ? tiltControlEnabled : tiltControlDisabled;
        } else {
            tiltControlButton.gameObject.SetActive(false);
        }

        // Find the only menucamera and assign it
        menuCamera = FindObjectOfType<MenuCamera>();
    
        //position our camera on the focused menu

        SetCameraTo(Manager.Instance.menuFocus); 

        //tell our gold text how much to display

        UpdateGoldText();

        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1;

        // Add button on click events to the shop buttons;;
        InitShop();

        // Add buttons on click event to levels;
        IninLevel();

        //set player preferences for both the color and the trial

        OnColorSelect(SaveManager.Instance.state.activeColor);
        SetColor(SaveManager.Instance.state.activeColor);

        OnTrialSelect(SaveManager.Instance.state.activeTrial);
        SetTrial(SaveManager.Instance.state.activeTrial);

        // Make the buttons bigger for selected item

        colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        trialPanel.GetChild(SaveManager.Instance.state.activeTrial).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

        // create the trial preview
        lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrial[SaveManager.Instance.state.activeTrial ]) as GameObject;
        lastPreviewObject.transform.SetParent(trialPreviewObjet);
        lastPreviewObject.transform.localPosition = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        // Fade-in 

        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeSpeed;

        //Set pos to main menu
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition, desiredMenuPosition, 0.1f);

        //Entering level zoom 
        if (isEnteringLevel) 
        {
            zoomTransition += ( 1 / zoomDuration ) * Time.deltaTime;
            
            // Change the scale (Rect Transform) following the animation curve

            menuContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // change the desired position of the canvas so it can follow the scale up
            // this zoom in the center 

            Vector3 newDesiredPosition = desiredMenuPosition * 5;
            // this adds to the specific level of the campus

            RectTransform rectTransform = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rectTransform.anchoredPosition3D * 5;

            // this line will overrite the previous position update

            menuContainer.anchoredPosition = Vector3.Lerp(desiredMenuPosition, newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            //Fade to white screen overrite first line of update

            fadeGroup.alpha = zoomTransition;

            if(zoomTransition >= 1) {
                // are we done with the animation, 
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void InitShop(){
        //make sure we assign the references..
        if(colorPanel == null || trialPanel == null) {
            Debug.Log("didn't assigned  the color panel in inspector");
        }
            //for every single children transform under our color panel, find the button and add onclick event

            int i = 0;

            foreach(Transform t in colorPanel) { 

                int currentIndex = i;

                Button button = t.GetComponent<Button>();
                button.onClick.AddListener(() => OnColorSelect(currentIndex));

                // Set the color of image based on if its owned or not
                Image img = t.GetComponent<Image>();
                img.color = SaveManager.Instance.IsColorOwned(currentIndex) ? Manager.Instance.colors[currentIndex] :
                Color.Lerp(Manager.Instance.colors[currentIndex], new Color(0, 0, 0, 1), 0.25f);

                i++;
            }

            // reset index
            i = 0;

            foreach (Transform t in trialPanel)
            {

                int currentIndex = i;

                Button button = t.GetComponent<Button>();
                button.onClick.AddListener(() => OnTrialSelect(currentIndex));

                RawImage img = t.GetComponent<RawImage>();

                img.color = SaveManager.Instance.IsTrialOwned(currentIndex) ? Color.white : new Color(0.7f, 0.7f, 0.7f);

                i++;
            }

            previousTrial = trialPanel.GetChild(SaveManager.Instance.state.activeTrial).GetComponent<RawImage>().texture; 
    }

    private void IninLevel()
    {
        if(levelPanel == null) { 
            Debug.Log("level panel is not assigned to the inspector"); 
            }

        int i = 0;

        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;

            Button button = t.GetComponent<Button>();
            button.onClick.AddListener(() => OnLevelSelect(currentIndex));
            
            Image img = t.GetComponent<Image>();

            // It is unlocked?
            if(i <= SaveManager.Instance.state.levelComplete) 
            {
                // it is unlocked!
                if(i == SaveManager.Instance.state.levelComplete) {
                    // it's not completed
                    img.color = Color.white;
                } else {
                    // level is already completed
                    img.color = Color.green;
                }
            }   

            else {
                // level isn't unlocked
                button.interactable = false;

                img.color = Color.grey;
            }


            i++;
        }
    }

    private void SetCameraTo(int menuIndex) { 
        NavigateTo(menuIndex);
        menuContainer.anchoredPosition3D = desiredMenuPosition;
    }

    private void NavigateTo(int menuIndex) {
        switch(menuIndex) {

            default:
            case 0:
            desiredMenuPosition = Vector3.zero;
            menuCamera.BackToMainMenu();
            break;

            case 1:
            desiredMenuPosition = Vector3.right * 1280;
            menuCamera.MoveToLevel();
            break;

            case 2:
            desiredMenuPosition = Vector3.left * 1280;
            menuCamera.MoveToShop();
            break;
        }
    }

    private void SetColor(int index) {
        // set the active index to this index;

        activeColorIndex = index;

        SaveManager.Instance.state.activeColor = index;

        // change the color on player model
        Manager.Instance.playerMaterial.color = Manager.Instance.colors[index];

        // change buy/set button text
        colorBuySetText.text = "current";

        SaveManager.Instance.Save();
    }

    private void SetTrial(int index) {
        // change the trial on player 
        activeTrialIndex = index;

        SaveManager.Instance.state.activeTrial = index;

        if(currentTrial != null) {
            Destroy(currentTrial);
        }

        //create the new trial
        currentTrial = Instantiate(Manager.Instance.playerTrial[index]) as GameObject;

        // set it as a children of player
        currentTrial.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        currentTrial.transform.localPosition = Vector3.zero;
        currentTrial.transform.localRotation = Quaternion.Euler(0, 0, 90);
        currentTrial.transform.localScale = Vector3.one * 0.01f;

        // change buy/set color trial text
        trialColorBuySetText.text = "current";

        //remember preferences
        SaveManager.Instance.Save();
    }

    private void UpdateGoldText() {
        goldText.text = SaveManager.Instance.state.gold.ToString();
    }
    //Button section
    public void OnPlayClick() {
        NavigateTo(1);
    }

    public void OnShopClick() {
        NavigateTo(2);
    }

    public void OnBackClick()   
    {
        NavigateTo(0);
    }

    private void OnColorSelect(int currentIndex)
    {
        Debug.Log("Selecting color button" + currentIndex);



        if (selectedColorindex == currentIndex)
        {
            return;
        }

        // make the selected color slightly bigger 
        colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // put the previous one on normal scale
        colorPanel.GetChild(selectedColorindex).GetComponent<RectTransform>().localScale = Vector3.one;


        // set the selected color
        selectedColorindex = currentIndex;

        // if the button clicked is already selected then return 


        
        // change the content of the buy/set button, depending on the state of the color 

        if(SaveManager.Instance.IsColorOwned(currentIndex)) {
            // Color is owned
            // it is already selected?

            if(activeColorIndex == currentIndex) {
                colorBuySetText.text = "Current";
            } else {
                colorBuySetText.text = "Select";    
            }

            
        
        } else {
            // color isn't owned    
            colorBuySetText.text = "Buy: " + colorCost[currentIndex].ToString();
        }
    }

    private void OnTrialSelect(int currentIndex) 
    {
        Debug.Log("Selecting trial button" + currentIndex);

        if(selectedTrialIndex == currentIndex) { return; }

        //preview trial here
        // Get the image of the preview button
        trialPanel.GetChild(selectedTrialIndex).GetComponent<RawImage>().texture = previousTrial;
        //keep the new trials image in previous trial
        previousTrial = trialPanel.GetChild(currentIndex).GetComponent<RawImage>().texture;
        // set the new trial preview image to the other camera
        trialPanel.GetChild(currentIndex).GetComponent<RawImage>().texture = trialPreviewTexture;

        if(lastPreviewObject != null) 
            Destroy(lastPreviewObject);
            lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrial[currentIndex]) as GameObject;
            lastPreviewObject.transform.SetParent(trialPreviewObjet);
            lastPreviewObject.transform.localPosition = Vector3.zero;
        
       

        // make the selected trial slightly bigger 
        trialPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // put the previous one on normal scale
        trialPanel.GetChild(selectedTrialIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        selectedTrialIndex = currentIndex;

        if(SaveManager.Instance.IsTrialOwned(currentIndex)) {
            // trial is owned 
            if(activeTrialIndex == currentIndex) {
                trialColorBuySetText.text = "Current";
            } else {
                trialColorBuySetText.text = "Select";
            }

        

        } else {
            trialColorBuySetText.text = "Buy: " + trialCost[currentIndex].ToString();
        }


    }

    private void OnLevelSelect(int currentIndex)
    {
        Manager.Instance.currentLevel = currentIndex;
        isEnteringLevel = true;

        Debug.Log("selecting level on index" + currentIndex);
    }

    public void OnColorBuySet()
    {
        Debug.Log("Buy/set color");

        // Is the selected color owned 
        if(SaveManager.Instance.IsColorOwned(selectedColorindex)) {
            // set the color

            SetColor(selectedColorindex);
        } else {
            // Attempt to buy the color
            if(SaveManager.Instance.BuyColor(selectedColorindex, colorCost[selectedColorindex])) {
                // success
                SetColor(selectedColorindex);

                // Change the color of the button
                colorPanel.GetChild(selectedColorindex).GetComponent<Image>().color = Manager.Instance.colors[selectedColorindex];
               
                UpdateGoldText();

            } else {
                // do not have enough gold
                // play feedback sound
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnTrialBuySet()
    {
        Debug.Log("Buy/set trial");

        if(SaveManager.Instance.IsTrialOwned(selectedTrialIndex)) {
            SetTrial(selectedTrialIndex);
        } else {

            if(SaveManager.Instance.BuyTrial(selectedTrialIndex, trialCost[selectedTrialIndex])) {
                SetTrial(selectedTrialIndex);

                trialPanel.GetChild(selectedTrialIndex).GetComponent<Image>().color = Color.white;
                UpdateGoldText();

            } else {

                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnTiltControl() {
        // toggle accelorometer boolean
        SaveManager.Instance.state.usingAccelerometer = !SaveManager.Instance.state.usingAccelerometer;

        // make sure we save the save platform
        SaveManager.Instance.Save();

        // change the display image of the tilt control button
        tiltControlButton.GetComponent<Image>().color = SaveManager.Instance.state.usingAccelerometer ? tiltControlEnabled : tiltControlDisabled;
    }


}
