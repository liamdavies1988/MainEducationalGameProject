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
    // 1. THIS IS THE FIX: Tell the popup which slot we are talking about!
    slotIndexToProcess = id; 

    // 2. Tell the Brain which slot we are using
    GameManager.Instance.selectedSlot = id;

    // 3. CRITICAL PATH FIX: Change 'dataPath' to 'persistentDataPath'
    // If you don't do this, the popup will check the wrong folder!
    string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";

    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        GameManager.Instance.totalCoins = data.coins;
        GameManager.Instance.playerName = data.playerName;
        GameManager.Instance.selectedFarmID = data.farmID;
        
        loadPopup.SetActive(true);
        
        Debug.Log("Popup showing for Slot: " + (id + 1));
    if(loadPopupText != null) loadPopupText.text = "Do you want to load " + data.playerName + "?";
    }
    else
    {
        GameManager.Instance.ResetData();
        SceneManager.LoadScene("PlayerCreation");
    }
}
  // Confirm loading an existing save slot.
    public void ConfirmLoad()
{
    // Use the variable we just set in OnSlotClicked
    GameManager.Instance.selectedSlot = slotIndexToProcess;

    // Synchronize the Brain with the file
    GameManager.Instance.LoadGameData();

    Debug.Log("ConfirmLoad: Loading Data for Slot: " + (slotIndexToProcess + 1));

    SceneManager.LoadScene("PlayerAndFarm"); 
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
            Debug.Log("Deleted File: " + path);

            GameManager.Instance.ResetData();
        }

        if (deletePopup != null)
        {
            deletePopup.SetActive(false);
        }

        RefreshSlotLabels();
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
// --- RECENTLY EDITED FILES ---