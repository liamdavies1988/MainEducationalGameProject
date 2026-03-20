using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance for easy access from other scripts

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip wrongAnswerSound;

    [Header("Economy")]
    public int totalCoins = 0;

    [Header("Player Data")]
    public string playerName = "Player1";

    [Header("UI Reference")]
    public TextMeshProUGUI coinCountText;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this alive across scenes
            LoadGame();
        }
        else
        {
            // If another instance already exists, destroy this one to enforce the singleton pattern
            Destroy(gameObject);
        }
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
        PlayerPrefs.SetInt("SavedCoins", totalCoins);
        PlayerPrefs.SetString("SavedName", playerName);
        PlayerPrefs.Save();
        // Added a space between name and 'has'
        Debug.Log("Game Saved! " + playerName + " has " + totalCoins + " coins remaining.");
    }

    public void LoadGame() // Call this at the start of the game to load saved data
    {
        totalCoins = PlayerPrefs.GetInt("SavedCoins", 0);
        playerName = PlayerPrefs.GetString("SavedName", "Player1");
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
}