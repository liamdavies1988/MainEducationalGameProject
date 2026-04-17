using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

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

    [Header("UI Reference")]
    public TextMeshProUGUI coinCountText;

    [Header("Topic Settings")]
    public string selectedSubject = "Maths";
    public string selectedDifficulty = "Easy";

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
    }

    public void AddCoin()
    {
        totalCoins += 1;
        UpdateCoinUI();
        SaveCurrentProgress();

        if (audioSource != null && coinSound != null)
            audioSource.PlayOneShot(coinSound);
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
        // 1. Prepare the bundle of data
        PlayerSaveData data = new PlayerSaveData();
        data.playerName = this.playerName;
        data.coins = this.totalCoins;
        data.farmID = this.selectedFarmID;
        data.activeAnimals = new List<string>(this.activeAnimals);

        // Use the CharacterStyleManager logic to fill the rest if available
        // For now, we save the core session data
        SaveGame(data);
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

    public PlayerSaveData LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Sync the Brain
            this.playerName = data.playerName;
            this.totalCoins = data.coins;
            this.selectedFarmID = data.farmID;
            this.activeAnimals = data.activeAnimals != null ? data.activeAnimals : new List<string>();

            UpdateCoinUI();
            Debug.Log("GameManager: Synchronized with Slot " + (selectedSlot + 1));
            return data;
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
        //currentSessionData = null;
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