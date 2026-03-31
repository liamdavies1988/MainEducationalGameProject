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
    
    [Header("Farm Layouts")]
    public GameObject[] farmLayouts;

    void Start()
    {
        LoadPlayerAndWorld();
    }

    public void LoadPlayerAndWorld()
    {
        int slotID = GameManager.Instance.selectedSlot;
        string filePath = Application.dataPath + "/Saves/SaveSlot_" + (slotID + 1) + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // --- THE CHARACTER PART ---
            headDisplay.sprite = headOptions[data.headID];
            bodyDisplay.sprite = bodyOptions[data.bodyID];
            legsDisplay.sprite = legsOptions[data.legsID];

            // --- THE FARM PART (The loop we discussed) ---
            // This turns off every farm except the one matching data.farmID
            for (int i = 0; i < farmLayouts.Length; i++)
            {
                farmLayouts[i].SetActive(i == data.farmID);
            }

            Debug.Log("World Reconstructed: Farm Type " + data.farmID);
        }
    }
    
}