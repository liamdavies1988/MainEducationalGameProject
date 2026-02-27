using UnityEngine;
using TMPro; // Crucial for using TextMeshPro!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // The "Singleton" so buttons can find it

    public AudioSource audioSource; // The speaker
    public AudioClip coinSound;     // The actual sound file 
    public AudioClip wrongAnswerSound; // The actual sound file for wrong answer buzz

    [Header("Economy")]
    public int totalCoins = 0;

    [Header("UI Link")]
    public TextMeshProUGUI coinCountText; // Drag your text object here

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    // This is called by your buttons
    public void AddCoin()
    {
        totalCoins += 1; // Give 1 coin
        UpdateCoinUI();
        Debug.Log("Coin added! New total: " + totalCoins);
        
        // Play the coin sound effect
        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void WrongAnswerSound()
    {
        // Play the wrong answer sound effect
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = totalCoins.ToString();
        }
        
    }
}