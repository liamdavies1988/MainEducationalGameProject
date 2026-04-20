using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // ONLY ONE INSTANCE DEFINITION HERE
    public static GameManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip wrongAnswerSound;

    [Header("Economy")]
    public int totalCoins = 0;

    [Header("Player Data")]
    public string playerName = "Player1";
    public int selectedSlot;
    public int selectedFarmID;
    public List<string> activeAnimals = new List<string>();
    private PlayerSaveData masterCachedProfile;

    [Header("UI Reference")]
    public TextMeshProUGUI coinCountText;

    [Header("UI Animation Settings")]
    private Color originalCoinColor = Color.white; 
    private bool isFlashing = false;

    [Header("Character Feedback UI")]
    public GameObject smileFace; // These will be filled by the CharacterLoader
    public GameObject sadFace;

    [Header("Topic Settings")]
    public string selectedSubject = "Maths";
    public string selectedDifficulty = "Easy";

    [Header("Session Settings")]
    public int totalQuestionsRequested = 10; // Default to 10

    [Header("Difficulty Popup")]
    public GameObject difficultyPopup;





    //private PlayerSaveData currentSessionData;

    private void Awake()
    {
        // SINGLETON LOGIC
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure the folder exists
            string folderPath = Application.persistentDataPath + "/Saves/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            LoadGameData(); // Initial load
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
    }

    private void Start()
    {
        UpdateCoinUI();
        SetupCoinColor();
    }

    public void AddCoin() 
{
    // --- THE DYNAMIC REWARD FIX ---
    // If the difficulty is set to "Hard", give 2 coins. 
    // For anything else (Easy/Medium), give 1.
    int rewardAmount = (selectedDifficulty == "Hard") ? 2 : 1;
    
    totalCoins += rewardAmount;
    // ------------------------------

    UpdateCoinUI();
    SaveCurrentProgress(); 

    if (audioSource != null && coinSound != null)
    {
        audioSource.PlayOneShot(coinSound);
    }
    
    Debug.Log($"<color=yellow>Economy:</color> Awarded {rewardAmount} coins for {selectedDifficulty} mode.");
}

    private void SetupCoinColor()
    {
        if (coinCountText != null)
            originalCoinColor = coinCountText.color;
    }

    public void TriggerCoinFlash()
    {
        // Prevent multiple flashes from fighting each other
        if (isFlashing) return;

        // Ensure we have the current scene's text reference
        if (coinCountText == null)
        {
            GameObject textObj = GameObject.Find("CoinCountText");
            if (textObj != null) coinCountText = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (coinCountText != null)
        {
            StartCoroutine(FlashCoinsRoutine());
        }
    }

    IEnumerator FlashCoinsRoutine()
    {
        isFlashing = true;
        // Store the color of the text in THIS scene
        Color normalColor = coinCountText.color;

        // Flash Red 3 times
        for (int i = 0; i < 3; i++)
        {
            coinCountText.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            coinCountText.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }

        isFlashing = false;
    }

    public void SetQuestionAmount(int amount)
    {
        totalQuestionsRequested = amount;
        Debug.Log("<color=green>GameManager:</color> Question amount set to: " + amount);
    }

    public void PlayWrongSound()
    {
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    public void SaveCurrentProgress()
{
    if (masterCachedProfile == null) masterCachedProfile = LoadGameData();

    if (masterCachedProfile != null)
    {
        masterCachedProfile.coins = totalCoins;
        masterCachedProfile.playerName = playerName;
        masterCachedProfile.farmID = selectedFarmID;

        // CRITICAL: Put the live list BACK into the profile before saving
        // This stops the Quiz from saving an empty list over your farm!
        masterCachedProfile.activeAnimals = new List<string>(this.activeAnimals);

        string json = JsonUtility.ToJson(masterCachedProfile, true);
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";
        File.WriteAllText(path, json);
        
        Debug.Log("<color=cyan>GameManager:</color> Save successful. Animals preserved.");
    }
}

    public void SaveGame(PlayerSaveData data)
    {
        int slot = selectedSlot + 1;
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + slot + ".json";

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        // Sync internal Brain
        this.totalCoins = data.coins;
        this.playerName = data.playerName;
        this.selectedFarmID = data.farmID;
        this.activeAnimals = data.activeAnimals;

        Debug.Log("<color=cyan>GameManager:</color> Successfully saved data to Slot " + slot);
    }

    public void ShowReaction(bool isCorrect)
    {
        Debug.Log($"<color=cyan>GameManager:</color> ShowReaction called. isCorrect: {isCorrect}");

        // Stop any existing timers
        StopCoroutine("HideReactions");

        if (isCorrect)
        {
            if (smileFace != null) {
                smileFace.SetActive(true);
                Debug.Log("GameManager: smileFace activated.");
            } else Debug.LogError("GameManager: smileFace is NULL! Check if sceneSmile is assigned in CharacterLoader.");

            if (sadFace != null) sadFace.SetActive(false);
        }
        else
        {
            if (smileFace != null) smileFace.SetActive(false);
            if (sadFace != null) {
                sadFace.SetActive(true);
                Debug.Log("GameManager: sadFace activated.");
            } else Debug.LogError("GameManager: sadFace is NULL! Check if sceneSad is assigned in CharacterLoader.");
        }

        StartCoroutine(HideReactions());
    }

    IEnumerator HideReactions()
    {
        yield return new WaitForSeconds(1.5f);
        if (smileFace != null) smileFace.SetActive(false);
        if (sadFace != null) sadFace.SetActive(false);
    }

    public PlayerSaveData LoadGameData()
{
    string filePath = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";

    if (File.Exists(filePath))
    {
        string json = File.ReadAllText(filePath);
        masterCachedProfile = JsonUtility.FromJson<PlayerSaveData>(json);
        
        // --- THE SYNC FIX ---
        this.playerName = masterCachedProfile.playerName;
        this.totalCoins = masterCachedProfile.coins;
        this.selectedFarmID = masterCachedProfile.farmID;

        // CRITICAL: Fill the live list from the file so the Brain 'remembers' them in the Quiz
        if (masterCachedProfile.activeAnimals != null) {
            this.activeAnimals = new List<string>(masterCachedProfile.activeAnimals);
        } else {
            this.activeAnimals = new List<string>();
        }
        // --------------------

        UpdateCoinUI();
        Debug.Log("GameManager: Brain synchronized with " + activeAnimals.Count + " animals.");
        return masterCachedProfile;
    }
    return null;
}

    public void UpdateCoinUI()
    {
        if (coinCountText == null)
        {
            GameObject textObj = GameObject.Find("CoinCountText");
            if (textObj != null) coinCountText = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + totalCoins.ToString();
        }
    }

    public void SetSubject(string newSubject)
    {
        selectedSubject = newSubject;
        Debug.Log("<color=green>GameManager:</color> Subject set to: " + selectedSubject);
    }

    public void SelectDifficultyAndStart(string difficulty)
    {
        selectedDifficulty = difficulty;
        SaveCurrentProgress();
        SceneManager.LoadScene("MultipleChoiceGame");
    }

    public void ResetData()
    {
        totalCoins = 0;
        playerName = "Player1";
        selectedFarmID = 0;
        activeAnimals.Clear();
        masterCachedProfile = null; // Clear the cache so we start fresh next time we load
        UpdateCoinUI();
        Debug.Log("<color=red>GameManager:</color> Session memory wiped.");
    }
    
}

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
    public List<string> activeAnimals;
}

// --- RECENTLY EDITED FILES ---