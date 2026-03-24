using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CharacterLoader : MonoBehaviour
{
    [Header("Sprite Lists (Must match Creator exactly)")]
    public Sprite[] headOptions;
    public Sprite[] bodyOptions;
    public Sprite[] legsOptions;

    [Header("UI Displays in this Scene")]
    public Image headDisplay;
    public Image bodyDisplay;
    public Image legsDisplay;
    public TextMeshProUGUI nameLabel; // Optional: To show "Liam's Farm"

    void Start()
    {
        LoadCharacterAppearance();
    }

    public void LoadCharacterAppearance()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("DEBUG: GameManager is MISSING in this scene!");
            return;
        }

        int slotID = GameManager.Instance.selectedSlot;
        string fileName = "SaveSlot_" + (slotID + 1) + ".json";
        string filePath = Path.Combine(Application.dataPath, "Saves", fileName);

        Debug.Log("DEBUG: Attempting to load from: " + filePath);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("DEBUG: JSON Content found: " + json);

            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Check if the arrays are empty
            if (headOptions.Length == 0) Debug.LogError("DEBUG: Head Sprite array is EMPTY in the Inspector!");

            // Update UI
            headDisplay.sprite = headOptions[data.headID];
            bodyDisplay.sprite = bodyOptions[data.bodyID];
            legsDisplay.sprite = legsOptions[data.legsID];

            if (nameLabel != null)
            {
                nameLabel.text = data.playerName + "'s Farm";
                Debug.Log("DEBUG: Name set to: " + data.playerName);
            }
        }
        else
        {
            Debug.LogError("DEBUG: File NOT FOUND at " + filePath);
        }
    }
}