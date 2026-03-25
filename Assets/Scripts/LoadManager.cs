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

    [Header("Pop-up Reference")]
    public GameObject confirmationPopup;
    public TextMeshProUGUI popupText; // To show "Are you sure ?
    private int slotPendingDeletion; // Temporary "Holding Box" for the slot number

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
        
    }
    public void DeleteSlot(int id)
    {
        // 1. Build the path to the specific file
        string fileName = "SaveSlot_" + (id + 1) + ".json";
        string filePath = Path.Combine(Application.dataPath, "Saves", fileName);

        // 2. Check if it exists before trying to delete
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted file: " + fileName);

            // 3. Refresh the labels immediately so the UI updates
            RefreshSlotLabels();
        }
        else
        {
            Debug.LogWarning("Nothing to delete in slot " + id);
        }
    }
   
    // THIS IS CALLED BY YOUR 3 INDIVIDUAL DELETE BUTTONS
    public void RequestDelete(int id)
    {
        slotPendingDeletion = id; // Remember which one was clicked

        // OPTIONAL: Make the pop-up text show the player's name so it's extra clear
        string path = Application.dataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            popupText.text = "Are you sure you want to delete " + data.playerName + "?";
        }

        confirmationPopup.SetActive(true); // Show the pop-up
    }

    // THIS IS CALLED BY THE "YES" BUTTON ON THE POP-UP
    public void ConfirmDeletion()
    {
        string fileName = "SaveSlot_" + (slotPendingDeletion + 1) + ".json";
        string filePath = Path.Combine(Application.dataPath, "Saves", fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            RefreshSlotLabels(); // Updates the main buttons back to "New Game"
        }

        ClosePopup();
    }

    public void ClosePopup()
    {
        confirmationPopup.SetActive(false);
    }
}