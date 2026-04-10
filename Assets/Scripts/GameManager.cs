using UnityEngine;
using TMPro;
using System;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure the folder exists
            string folderPath = Application.dataPath + "/Saves/";
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
        LoadGame();
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin() // Call this whenever the player earns a coin (e.g., by answering correctly)
    {
        totalCoins += 1;
        UpdateCoinUI();
        Debug.Log("Coin added! New total: " + totalCoins);
        SaveGame();

        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void PlayWrongSound() // Call this when the player selects a wrong answer
    {
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    public void SaveGame() // Call this whenever you want to save the current game state
    {
        // Use the selectedSlot to name the file!
        string filePath = Application.dataPath + "/PlayerSaveFiles/SaveSlot_" + selectedSlot + ".json";

        PlayerPrefs.SetInt("SavedCoins_" + selectedSlot, totalCoins);
        PlayerPrefs.SetString("SavedName_" + selectedSlot, playerName);
        PlayerPrefs.Save();

        Debug.Log("Game Saved to Slot " + selectedSlot);
        Debug.Log("Game Saved! " + playerName + " has " + totalCoins + " coins remaining.");
    }

    public void LoadGame() // Call this at the start of the game to load saved data
    {
        // Make sure we load the coins and name for the specific student in this slot
        totalCoins = PlayerPrefs.GetInt("SavedCoins_" + selectedSlot, 0);
        playerName = PlayerPrefs.GetString("SavedName_" + selectedSlot, "Player1");
        UpdateCoinUI();
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

    public void ResetData() // Call this to clear all saved data and reset the game state
    {
        PlayerPrefs.DeleteAll();
        totalCoins = 0;
        playerName = "Player1";
        UpdateCoinUI();
        Debug.Log("Game data reset! All progress has been cleared.");
    }

    internal void WrongAnswerSound()
    {
        throw new NotImplementedException();
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
    internal int selectedFarmID;
    internal int farmID;

    public void ShowThePopup()
{
    // This 'switches on' the lightbulb
    myPopup.SetActive(true); 
}
    
}