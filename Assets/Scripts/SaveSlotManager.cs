using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSlotManager : MonoBehaviour
{
    [Header("UI Buttons Text")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    void Start()
    {
        // Update the labels as soon as the menu opens
        UpdateSlotLabel(0, slot1Text);
        UpdateSlotLabel(1, slot2Text);
        UpdateSlotLabel(2, slot3Text);
    }

    void UpdateSlotLabel(int slotID, TextMeshProUGUI label)
    {
        string path = Application.dataPath + "/PlayerSaveFiles/SaveSlot_" + slotID + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            label.text = data.playerName; // Show the student's name
        }
        else
        {
            label.text = "Empty Slot"; // Show "Empty" if no one has used it
        }
    }

    // Called when a button is clicked
    public void SelectSlot(int slotID)
    {
        // This line "talks" to the GameManager and fills the box
        GameManager.Instance.selectedSlot = slotID;

        // Now it checks the folder to see if we should load or create
        string path = Application.dataPath + "/Saves/SaveSlot_" + slotID + ".json";

        if (File.Exists(path))
        {
            Debug.Log("Loading Slot " + slotID);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("Creating New Player in Slot " + slotID);
            UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerCreator");
        }
    }
}