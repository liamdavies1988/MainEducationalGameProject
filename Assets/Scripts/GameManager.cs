// =================================================================================================
// File: GameManager.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: August 21, 2026
// =================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Character Feedback UI")]
    public GameObject smileFace;
    public GameObject sadFace;

    [Header("Topic Settings")]
    public string selectedSubject = "Maths";
    public string selectedDifficulty = "Easy";

    [Header("Session Settings")]
    public int totalQuestionsRequested = 10;

    [Header("Difficulty Popup")]
    public GameObject difficultyPopup;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CommitInternalSave();
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
            }

            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() => UpdateCoinUI();

    private void SyncToBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            CommitInternalSave();
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Browser sync skipped.");
        }
#endif
    }

    public void AddCoin()
    {
        totalCoins += (selectedDifficulty == "Hard") ? 2 : 1;
        UpdateCoinUI();
        SaveCurrentProgress();

        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void SetQuestionAmount(int amount) => totalQuestionsRequested = amount;
    public void SetSubject(string newSubject) => selectedSubject = newSubject;

    public void SelectDifficultyAndStart(string difficulty)
    {
        selectedDifficulty = difficulty;
        SaveCurrentProgress();
        SceneManager.LoadScene("MultipleChoiceGame");
    }

    public void SaveCurrentProgress()
    {
        if (masterCachedProfile == null)
        {
            masterCachedProfile = LoadGameData() ?? new PlayerSaveData();
        }

        masterCachedProfile.coins = totalCoins;
        masterCachedProfile.playerName = playerName;
        masterCachedProfile.farmID = selectedFarmID;
        masterCachedProfile.activeAnimals = new List<string>(activeAnimals);

        string json = JsonUtility.ToJson(masterCachedProfile, true);
        File.WriteAllText(Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json", json);
        SyncToBrowser();
    }

    public void SaveGame(PlayerSaveData data)
    {
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";
        File.WriteAllText(path, JsonUtility.ToJson(data, true));

        totalCoins = data.coins;
        playerName = data.playerName;
        selectedFarmID = data.farmID;
        activeAnimals = data.activeAnimals;
        SyncToBrowser();
    }

    public PlayerSaveData LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/Saves/SaveSlot_" + (selectedSlot + 1) + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            masterCachedProfile = JsonUtility.FromJson<PlayerSaveData>(json);

            playerName = masterCachedProfile.playerName;
            totalCoins = masterCachedProfile.coins;
            selectedFarmID = masterCachedProfile.farmID;
            activeAnimals = masterCachedProfile.activeAnimals ?? new List<string>();

            UpdateCoinUI();
            return masterCachedProfile;
        }

        return null;
    }

    public void ResetData()
    {
        totalCoins = 0;
        playerName = "Player1";
        selectedFarmID = 0;
        activeAnimals.Clear();
        masterCachedProfile = null;
        UpdateCoinUI();
    }

    public void ShowReaction(bool isCorrect)
    {
        if (smileFace != null)
        {
            smileFace.SetActive(isCorrect);
        }

        if (sadFace != null)
        {
            sadFace.SetActive(!isCorrect);
        }

        Invoke(nameof(HideFaces), 1.5f);
    }

    private void HideFaces()
    {
        if (smileFace)
        {
            smileFace.SetActive(false);
        }

        if (sadFace)
        {
            sadFace.SetActive(false);
        }
    }

    public void UpdateCoinUI()
    {
        if (coinCountText == null)
        {
            GameObject t = GameObject.Find("CoinCountText");

            if (t)
            {
                coinCountText = t.GetComponent<TextMeshProUGUI>();
            }
        }

        if (coinCountText)
        {
            coinCountText.text = "Coins: " + totalCoins;
        }
    }

    public void PlayWrongSound()
    {
        if (audioSource && wrongAnswerSound)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    public void TriggerCoinFlash()
    {
        /* Logic removed for brevity */
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