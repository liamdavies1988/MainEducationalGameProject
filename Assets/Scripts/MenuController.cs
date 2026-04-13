using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Save Slot UI")]
    public TextMeshProUGUI[] slotTexts; // Only fill these in the Slots Scene

    [Header("Pop-up Windows")]
    public GameObject difficultyPopup; // For Subject Scene
    public GameObject deletePopup; // For Slots Scene
    public TextMeshProUGUI deletePopupText;

    [Header("Load Confirmation Pop-up")]
    public GameObject loadPopup;
    public TextMeshProUGUI loadPopupText;

    private int slotIndexToProcess; // Remembers which slot was clicked

    private void Start()
    {
        // Refresh save slot names when this controller starts.
        if (slotTexts != null && slotTexts.Length > 0)
        {
            RefreshSlotLabels();
        }
    }

    // --- SAVE SLOT LOGIC ---

    // Update the UI text for each save slot.
    public void RefreshSlotLabels()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (i + 1) + ".json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
                slotTexts[i].text = data.playerName;
            }
            else
            {
                slotTexts[i].text = "New Game";
            }
        }
    }

    // Called when a save slot is clicked.
    public void OnSlotClicked(int id)
    {
        slotIndexToProcess = id; // Remember which slot is being processed.
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";

        if (File.Exists(path))
        {
            // If a save exists, show the load confirmation pop-up.
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            if (loadPopupText != null)
            {
                loadPopupText.text = "Do you want to play as " + data.playerName + "?";
            }

            if (loadPopup != null)
            {
                loadPopup.SetActive(true);
            }
        }
        else
        {
            // No save found: go directly to the player creation scene.
            GameManager.Instance.selectedSlot = id;
            SceneManager.LoadScene("PlayerCreation");
        }
    }

    // Show the delete confirmation pop-up for a slot.
    public void OpenDeleteConfirmation(int id)
    {
        slotIndexToProcess = id;
        if (deletePopup != null)
        {
            deletePopup.SetActive(true);
        }
    }

    // Delete the selected save slot and refresh UI.
    public void ConfirmDelete()
    {
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (slotIndexToProcess + 1) + ".json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (deletePopup != null)
        {
            deletePopup.SetActive(false);
        }

        RefreshSlotLabels();
    }

    // Confirm loading an existing save slot.
    public void ConfirmLoad()
{
    // 1. Tell the GameManager which slot we are using
    GameManager.Instance.selectedSlot = slotIndexToProcess;

    // 2. TELL THE GAMEMANAGER TO LOAD THE DATA NOW
    // This ensures coins and names are ready BEFORE the next scene loads
    GameManager.Instance.LoadGameData();

    // 3. Move to the next scene
    SceneManager.LoadScene("TopicChoice"); 
}

    // --- CURRICULUM LOGIC ---

    // Called when the player chooses a subject.
    public void OnSubjectClicked(string subject)
{
    // We tell the GameManager (The Brain) to save this choice
    GameManager.Instance.SetSubject(subject);

    // Then we show the difficulty popup
    if (difficultyPopup != null)
    {
        difficultyPopup.SetActive(true);
    }
}

// Called when the player chooses a difficulty (e.g., Easy)
public void OnDifficultyClicked(string difficulty)
{
    // We tell the GameManager to save the difficulty and START THE GAME
    GameManager.Instance.SelectDifficultyAndStart(difficulty);
}

    // --- GENERAL UI ---

    // Close any active pop-up windows.
    public void CloseAllPopups()
    {
        // Turn off the Delete Pop-up if it exists
        if (deletePopup != null) 
            deletePopup.SetActive(false);

        // Turn off the Load Pop-up if it exists
        if (loadPopup != null) 
            loadPopup.SetActive(false);
        
        // Optional: Log it so you can see it working in the console
        Debug.Log("UI: All pop-ups closed.");
    }
}
