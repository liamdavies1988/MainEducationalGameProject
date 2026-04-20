// =================================================================================================
// File: GameManager.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: April 20, 2026
//
// Description:
// Acts as the central hub for the game, managing global state including economy, 
// player profiles, persistent data serialization, and session-specific settings.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
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
    public int totalQuestionsRequested = 10;

    [Header("Difficulty Popup")]
    public GameObject difficultyPopup;

    // --- Unity Callbacks ---

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            string folderPath = Application.persistentDataPath + "/Saves/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            LoadGameData();
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

    // --- Core Game Logic ---

    public void AddCoin()
    {
        // Award more coins for higher difficulty settings
        int rewardAmount = (selectedDifficulty == "Hard") ? 2 : 1;
        totalCoins += rewardAmount;

        UpdateCoinUI();
        SaveCurrentProgress();

        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void SetQuestionAmount(int amount)
    {
        totalQuestionsRequested = amount;
    }

    public void SetSubject(string newSubject)
    {
        selectedSubject = newSubject;
    }

    public void SelectDifficultyAndStart(string difficulty)
    {
        selectedDifficulty = difficulty;
        SaveCurrentProgress();
        SceneManager.LoadScene("MultipleChoiceGame");
    }

    // --- Data Persistence ---

    public void SaveCurrentProgress()
    {
        // Ensure the cache is initialized before updating values
        if (masterCachedProfile == null) masterCachedProfile = LoadGameData();

        if (masterCachedProfile != null)
        {
            masterCachedProfile.coins = totalCoins;
            masterCachedProfile.playerName = playerName;
            masterCachedProfile.farmID = selectedFarmID;
            masterCachedProfile.activeAnimals = new List<string>(this.activeAnimals);

            string json = JsonUtility.ToJson(masterCachedProfile, true);
            string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";
            File.WriteAllText(path, json);
        }
    }

    public void SaveGame(PlayerSaveData data)
    {
        // Serialize a complete data object to the current slot
        int slot = selectedSlot + 1;
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + slot + ".json";

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        // Sync internal state with the saved data
        this.totalCoins = data.coins;
        this.playerName = data.playerName;
        this.selectedFarmID = data.farmID;
        this.activeAnimals = data.activeAnimals;
    }

    public PlayerSaveData LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";

        if (File.Exists(filePath))
        {
            // Deserialize the profile and sync live variables
            string json = File.ReadAllText(filePath);
            masterCachedProfile = JsonUtility.FromJson<PlayerSaveData>(json);
            
            this.playerName = masterCachedProfile.playerName;
            this.totalCoins = masterCachedProfile.coins;
            this.selectedFarmID = masterCachedProfile.farmID;

            this.activeAnimals = masterCachedProfile.activeAnimals != null ? 
                new List<string>(masterCachedProfile.activeAnimals) : new List<string>();

            UpdateCoinUI();
            return masterCachedProfile;
        }
        return null;
    }

    public void ResetData()
    {
        // Clear all session memory and UI
        totalCoins = 0;
        playerName = "Player1";
        selectedFarmID = 0;
        activeAnimals.Clear();
        masterCachedProfile = null; 
        UpdateCoinUI();
    }

    // --- UI & Feedback Logic ---

    public void ShowReaction(bool isCorrect)
    {
        StopCoroutine(nameof(HideReactions));

        // Activate appropriate face reaction based on answer accuracy
        if (isCorrect)
        {
            if (smileFace != null) smileFace.SetActive(true);
            if (sadFace != null) sadFace.SetActive(false);
        }
        else
        {
            if (smileFace != null) smileFace.SetActive(false);
            if (sadFace != null) sadFace.SetActive(true);
        }

        StartCoroutine(HideReactions());
    }

    IEnumerator HideReactions()
    {
        // Deactivate feedback faces after a brief delay
        yield return new WaitForSeconds(1.5f);
        if (smileFace != null) smileFace.SetActive(false);
        if (sadFace != null) sadFace.SetActive(false);
    }

    public void TriggerCoinFlash()
    {
        if (isFlashing) return;

        // Find the coin text if it's not currently cached
        if (coinCountText == null)
        {
            GameObject textObj = GameObject.Find("CoinCountText");
            if (textObj != null) coinCountText = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (coinCountText != null) StartCoroutine(FlashCoinsRoutine());
    }

    IEnumerator FlashCoinsRoutine()
    {
        isFlashing = true;
        Color normalColor = coinCountText.color;

        // Toggle text color to red to indicate insufficient funds
        for (int i = 0; i < 3; i++)
        {
            coinCountText.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            coinCountText.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }
        isFlashing = false;
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

    private void SetupCoinColor()
    {
        if (coinCountText != null)
            originalCoinColor = coinCountText.color;
    }

    public void PlayWrongSound()
    {
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }
}

[System.Serializable]
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