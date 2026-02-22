using UnityEngine;
using TMPro; // Crucial for using TextMeshPro!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // The "Singleton" so buttons can find it

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
    }

    void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = totalCoins.ToString();
        }
    }
}