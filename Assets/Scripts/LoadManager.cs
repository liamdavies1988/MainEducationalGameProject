using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [Header("Button Text References")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    void Start()
    {
        // When the scene loads, check our 3 specific slots
        RefreshSlotLabels();
    }

    public void RefreshSlotLabels()
    {
        UpdateLabel(0, slot1Text);
        UpdateLabel(1, slot2Text);
        UpdateLabel(2, slot3Text);
    }

    void UpdateLabel(int id, TextMeshProUGUI label)
    {
        // The path must match the Save function: SaveSlot_1, SaveSlot_2, etc.
        string path = Application.dataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            label.text = data.playerName;
        }
        else
        {
            label.text = "New Game";
        }
    }

    public void ClickSlot(int id)
    {
        // Safety check: is the GameManager actually there?
        if (GameManager.Instance == null)
        {
            Debug.LogError("FATAL ERROR: No GameManager found in this scene! Please drag the GameManager object into the hierarchy.");
            return;
        }

        GameManager.Instance.selectedSlot = id;
        // ... rest of your code
    }
}