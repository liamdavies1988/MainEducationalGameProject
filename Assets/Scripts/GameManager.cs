using UnityEngine;
using TMPro;
using System;


[System.Serializable] // This allows us to easily convert this class to JSON for saving and loading
public class PlayerSaveData
{
    public string playerName;
    public int coins;
    public int farmID;
    public string hairName;
    public string topName;
    public string bottomName;
    public string skinName;
    public bool hasGlasses;
    public bool hasHearingAid;
    public bool hasCrutches;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for easy access from other scripts

    [Header("Audio")] // Grouping for audio-related variables
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip wrongAnswerSound;

    [Header("Economy")] // Grouping for coin-related variables
    public int totalCoins = 0;

    [Header("Player Data")] // Grouping for player-related variables
    public string playerName = "Player1";

    [Header("UI Reference")] // Grouping for UI-related variables
    public TextMeshProUGUI coinCountText;

    [Header("Save System")] // Grouping for save system-related variables
    public int selectedSlot;

    [Header("Topic Settings")]
    public string selectedSubject = "Maths"; // Default
    public string selectedDifficulty = "Easy"; // Default

    [Header("Difficulty Popup")]
    public GameObject difficultyPopup;

    private PlayerSaveData currentSessionData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure the folder exists
            string folderPath = Application.persistentDataPath + "/Saves/";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }
        else
        {
            // If a duplicate is found, kill it
            Destroy(gameObject);
            return; // Stop running the rest of this function
        }

        // Now safe to load
        LoadGameData();
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin() 
{
    totalCoins += 1;
    UpdateCoinUI();
    
    // This calls the function we just updated!
    SaveCurrentProgress(); 

    if (audioSource != null && coinSound != null)
        audioSource.PlayOneShot(coinSound);
}

    public void PlayWrongSound() // Call this when the player selects a wrong answer
    {
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

public void SaveCurrentProgress()
{
    // 1. Get the current data from the disk if memory is empty
    if (currentSessionData == null)
    {
        currentSessionData = LoadGameData();
    }

    // 2. SAFETY: If there is still no data (e.g., a brand new game with no file yet)
    // create a fresh object so the game doesn't crash.
    if (currentSessionData == null)
    {
        currentSessionData = new PlayerSaveData();
        currentSessionData.playerName = this.playerName; // Use the default "Player1"
    }

    // 3. UPDATE THE GAMEPLAY DATA
    // We only update what changes during the actual game levels
    currentSessionData.coins = totalCoins;
    currentSessionData.playerName = playerName;
    
    // Note: HairName, TopName, etc., stay as they were when loaded, 
    // ensuring we don't "lose" the character's look during gameplay.

    // 4. THE PATH (Consistent with your new system)
    string folderPath = Application.persistentDataPath + "/Saves/";
    if (!System.IO.Directory.Exists(folderPath)) System.IO.Directory.CreateDirectory(folderPath);

    string filePath = folderPath + "SaveSlot_" + (selectedSlot + 1) + ".json";

    // 5. WRITE TO DISK
    string json = JsonUtility.ToJson(currentSessionData, true);
    System.IO.File.WriteAllText(filePath, json);
    
    Debug.Log($"<color=green>Economy Saved:</color> Slot {selectedSlot + 1} updated with {totalCoins} coins.");
}

// Call this at the start of a scene to get the data back
public PlayerSaveData LoadGameData()
{
    // 1. Consistent Path
    string folderPath = Application.persistentDataPath + "/Saves/";
    string filePath = folderPath + "SaveSlot_" + (selectedSlot + 1) + ".json";

    if (System.IO.File.Exists(filePath))
    {
        // 2. Read and Convert
        string json = System.IO.File.ReadAllText(filePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        
        // 3. SYNC THE CACHE (The missing piece!)
        // This ensures that when you call SaveCurrentProgress() later, 
        // the GameManager already knows the Hair/Clothes from this file.
        this.currentSessionData = data;

        // 4. Sync the GameManager variables for immediate use
        this.playerName = data.playerName;
        this.totalCoins = data.coins;
        
        Debug.Log($"<color=cyan>GameManager:</color> Loaded data for {data.playerName} from Slot {selectedSlot + 1}");
        
        return data;
    }

    Debug.LogWarning("LoadGameData: No save file found at " + filePath);
    return null; 
}

    public void UpdateCoinUI() // Call this whenever the coin count changes to update the UI text
    {
        
        if (coinCountText == null)
        {
            GameObject textObj = GameObject.Find("CoinCountText");
            if (textObj != null)
            {
                coinCountText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }

        // If we have a reference, update the text
        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + totalCoins.ToString();
        }
    }


    public void WrongAnswerSound()
    {
    if (audioSource != null && wrongAnswerSound != null)
    {
        audioSource.PlayOneShot(wrongAnswerSound);
    }
}
    public void SelectSubject(string subject)
    {
        selectedSubject = subject;
        difficultyPopup.SetActive(true); // Show the Easy/Hard choice
    }
    public void SetSubject(string newSubject)
    {
        selectedSubject = newSubject;
        Debug.Log("Subject changed to: " + selectedSubject);
    }
    public void SelectDifficultyAndStart(string difficulty)
    {
        selectedDifficulty = difficulty;

        // Now that we have both Subject AND Difficulty, we can play!
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    public void SetDifficulty(string newDifficulty)
    {
        selectedDifficulty = newDifficulty;
        Debug.Log("Difficulty changed to: " + selectedDifficulty);
    }

    // This variable holds the 'switch' to your pop-up
public GameObject myPopup;
    public int selectedFarmID;
    public int farmID;

    public void ShowThePopup()
{
    // This 'switches on' the lightbulb
    myPopup.SetActive(true); 
}

    public void SaveGame(PlayerSaveData data)
    {
    // 1. Update the 'Current Session' cache so the hair/clothes are remembered
    this.currentSessionData = data;

    // 2. Sync the basic variables
    this.playerName = data.playerName;
    this.totalCoins = data.coins;

    // 3. Save the whole thing to the JSON file
    SaveCurrentProgress();
    
    Debug.Log("<color=green>MASTER SAVE:</color> All character and economy data synced to file.");
}
}