using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
public class CharacterCreator : MonoBehaviour
{
    [Header("Sprite Options")]
    public Sprite[] headOptions;
    public Sprite[] bodyOptions;
    public Sprite[] legsOptions;

    [Header("UI Displays")]
    public Image headDisplay;
    public Image bodyDisplay;
    public Image legsDisplay;
    public TMP_InputField nameInput;

    // Track which item we are looking at
    private int currentHeadIndex = 1;
    private int currentBodyIndex = 1;
    private int currentLegsIndex = 1;
    


void Start()
{
    
}

    

    // --- ARROW FUNCTIONS ---

    public void NextHead() { ChangeIndex(ref currentHeadIndex, 1, headOptions, headDisplay); }
    public void PrevHead() { ChangeIndex(ref currentHeadIndex, -1, headOptions, headDisplay); }

    public void NextBody() { ChangeIndex(ref currentBodyIndex, 1, bodyOptions, bodyDisplay); }
    public void PrevBody() { ChangeIndex(ref currentBodyIndex, -1, bodyOptions, bodyDisplay); }

    public void NextLegs() { ChangeIndex(ref currentLegsIndex, 1, legsOptions, legsDisplay); }
    public void PrevLegs() { ChangeIndex(ref currentLegsIndex, -1, legsOptions, legsDisplay); }

    // Helper to handle the looping logic
    private void ChangeIndex(ref int index, int direction, Sprite[] options, Image display)
    {
        index += direction;
        if (index < 0) index = options.Length - 1;
        if (index >= options.Length) index = 0;

        display.sprite = options[index];
    }

    // --- SAVE FUNCTION ---

    public void SaveAndGoToFarmSelect()
    {
        string playerName = nameInput.text;

        // 1. Basic validation: Don't let them proceed without a name
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player must enter a name before proceeding.");
            // Optional: You could trigger a small shake animation on the name box here
            return; 
        }

        // 2. Prepare the data object
        PlayerSaveData data = new PlayerSaveData();
        data.playerName = playerName;
        data.headID = currentHeadIndex;
        data.bodyID = currentBodyIndex;
        data.legsID = currentLegsIndex;
        
        // Ensure we carry over any coins from the GameManager
        data.coins = GameManager.Instance.totalCoins;

        // 3. Save to the specific slot file
        int slot = GameManager.Instance.selectedSlot;
        string folderPath = Application.dataPath + "/Saves/";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        string filePath = folderPath + "SaveSlot_" + (slot + 1) + ".json";
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Character Progress Saved. Moving to Farm Selection.");

        // 4. Transition to the Farm Selection Scene
        // Make sure you name your scene exactly "FarmSelection" in Unity
        SceneManager.LoadScene("FarmSelection");
    }

    public void LoadPlayer()
    {
        string filePath = Application.dataPath + "/Saves/PlayerProfile.json";

        // 1. Check if the file actually exists
        if (File.Exists(filePath))
        {
            // 2. Read the text from the file
            string json = File.ReadAllText(filePath);

            // 3. Convert that text back into a Data Object
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // 4. Update the variables in the code
            nameInput.text = data.playerName;
            currentHeadIndex = data.headID;
            currentBodyIndex = data.bodyID;
            currentLegsIndex = data.legsID;

            // 5. Update the images on the screen so the player sees the change
            headDisplay.sprite = headOptions[currentHeadIndex];
            bodyDisplay.sprite = bodyOptions[currentBodyIndex];
            legsDisplay.sprite = legsOptions[currentLegsIndex];

            Debug.Log("Character Loaded! Welcome back, " + data.playerName);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + filePath);
        }
        
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public string playerName;
    public int headID;
    public int bodyID;
    public int legsID;
    public int farmID;
    public int coins;
}