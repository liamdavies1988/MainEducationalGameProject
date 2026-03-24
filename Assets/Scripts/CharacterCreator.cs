using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
    private int currentHeadIndex = 0;
    private int currentBodyIndex = 0;
    private int currentLegsIndex = 0;

    string filePath = folderPath + "SaveSlot_" + GameManager.Instance.selectedSlot + ".json"; // Construct the file path using the selected slot from GameManager
    private static string folderPath;

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

    public void SavePlayer()
    {
        // 1. Get the slot ID from the GameManager (0, 1, or 2)
        int slotID = GameManager.Instance.selectedSlot;

        // 2. Create the filename (adds 1 so the file is called Slot 1, 2, or 3)
        string fileName = "SaveSlot_" + (slotID + 1) + ".json";
        string folderPath = Application.dataPath + "/Saves/";
        string filePath = folderPath + fileName;

        // 3. Prepare the data
        PlayerSaveData data = new PlayerSaveData();
        data.playerName = nameInput.text;
        data.headID = currentHeadIndex;
        data.bodyID = currentBodyIndex;
        data.legsID = currentLegsIndex;
        data.coins = GameManager.Instance.totalCoins;

        // 4. Save to the file
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(filePath, json);

        Debug.Log("Saved to: " + fileName);
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
    public int coins;
}