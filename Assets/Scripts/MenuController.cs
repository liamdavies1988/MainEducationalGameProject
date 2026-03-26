using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Save Slot UI")]
    public TextMeshProUGUI[] slotTexts; // Only fill these in the Slots Scene

    [Header("Pop-up Windows")]
    public GameObject difficultyPopup;  // For Subject Scene
    public GameObject deletePopup;      // For Slots Scene
    public TextMeshProUGUI deletePopupText;

    private int slotIndexToProcess; // Remembers which slot was clicked

    void Start()
    {
        // If we are in the Slots Scene, refresh the names
        if (slotTexts != null && slotTexts.Length > 0)
        {
            RefreshSlotLabels();
        }
    }

    // --- SAVE SLOT LOGIC ---

    public void RefreshSlotLabels()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            string path = Application.dataPath + "/Saves/SaveSlot_" + (i + 1) + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
                slotTexts[i].text = data.playerName;
            }
            else { slotTexts[i].text = "New Game"; }
        }
    }

    public void OnSlotClicked(int id)
    {
        GameManager.Instance.selectedSlot = id;
        string path = Application.dataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";

        if (File.Exists(path)) { SceneManager.LoadScene("MainMenu"); }
        else { SceneManager.LoadScene("PlayerCreator"); }
    }

    public void OpenDeleteConfirmation(int id)
    {
        slotIndexToProcess = id;
        if (deletePopup != null) deletePopup.SetActive(true);
    }

    public void ConfirmDelete()
    {
        string path = Application.dataPath + "/Saves/SaveSlot_" + (slotIndexToProcess + 1) + ".json";
        if (File.Exists(path)) { File.Delete(path); }
        if (deletePopup != null) deletePopup.SetActive(false);
        RefreshSlotLabels();
    }

    // --- CURRICULUM LOGIC ---

    public void OnSubjectClicked(string subject)
    {
        GameManager.Instance.selectedSubject = subject;
        if (difficultyPopup != null) difficultyPopup.SetActive(true);
    }

    public void OnDifficultyClicked(string difficulty)
    {
        GameManager.Instance.selectedDifficulty = difficulty;
        SceneManager.LoadScene("GameScene");
    }

    // --- GENERAL UI ---

    public void CloseAllPopups()
    {
        if (difficultyPopup != null) difficultyPopup.SetActive(false);
        if (deletePopup != null) deletePopup.SetActive(false);
    }
}