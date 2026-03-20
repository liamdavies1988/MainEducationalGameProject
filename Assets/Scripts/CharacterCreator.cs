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
        string playerName = nameInput.text;

        // Basic check for empty name
        if (string.IsNullOrEmpty(playerName)) playerName = "Farmer";

        // Create the data object
        PlayerSaveData data = new PlayerSaveData();
        data.playerName = playerName;
        data.headID = currentHeadIndex;
        data.bodyID = currentBodyIndex;
        data.legsID = currentLegsIndex;
        data.coins = GameManager.Instance != null ? GameManager.Instance.totalCoins : 0;

        // Save to project folder
        string folderPath = Application.dataPath + "/PlayerSaveFiles/";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(folderPath + "PlayerProfile.json", json);

        Debug.Log("Character Saved! Name: " + playerName);

        // OPTIONAL: Move to next scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
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