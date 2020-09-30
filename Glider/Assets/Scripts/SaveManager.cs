using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        //are we using accelorometer and can we use it
        if(state.usingAccelerometer && !SystemInfo.supportsAccelerometer) 
        {
            // if we can't make sure we are not trying next time
            state.usingAccelerometer = false; 
            Save();
        }
        
    }

    //save the whole state of this savestate script to player prefs
    public void Save() {
         PlayerPrefs.SetString("save", HelperScript.Serialize<SaveState>(state));
    }

    public void Load() {
        if(PlayerPrefs.HasKey("save")){
            state = HelperScript.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        } else {
            state = new SaveState();
            Save();
            Debug.Log("no save file found. creating new one");
        }
    }

    public bool IsColorOwned(int index) {
        return (state.colorOwned & 1 << index) != 0;
    }

    public bool IsTrialOwned(int index) {
        return (state.trailOwned & 1 << index) != 0;
    }

    public bool BuyColor(int index, int cost) {

        if(state.gold >= cost) {
            // enough money, remove the gold from stack
            state.gold -= cost;

            UnlockColor(index);
            Save();

            return true;
        } else {
            // not enough money return false
            return false;
        }

    }

    public bool BuyTrial(int index, int cost) {

        if(state.gold >= cost) {
            
            state.gold -= cost;
            UnlockTrial(index);

            Save();

            return true;
        } else {
            return false;
        }
    }

    public void UnlockColor(int index) {
        state.colorOwned |= 1 << index;
    }

    public void UnlockTrial(int index) {
        state.trailOwned |= 1 << index;
    }

    public void CompleteLevel(int index) {
        
        if(state.levelComplete == index) {
            state.levelComplete++; 
            Save();
        }
    }

    public void Reset() {
        PlayerPrefs.DeleteKey("save");
    }

} // class