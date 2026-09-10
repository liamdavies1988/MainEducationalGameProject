using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Save Slot UI")]
    public TextMeshProUGUI[] slotTexts;

    [Header("Pop-up Windows")]
    public GameObject difficultyPopup; 
    public GameObject deletePopup;
    public TextMeshProUGUI deletePopupText;
    public GameObject loadPopup;
    public TextMeshProUGUI loadPopupText;
    public GameObject amountPopup; 

    private int slotIndexToProcess;

    private void Start() { if (slotTexts != null && slotTexts.Length > 0) RefreshSlotLabels(); }

    public void OnSubjectClicked(string subject) {
        GameManager.Instance.selectedSubject = subject;
        if (amountPopup != null) amountPopup.SetActive(true);
    }

    public void OnAmountSelected(int amount) {
        GameManager.Instance.SetQuestionAmount(amount);
        if (amountPopup != null) amountPopup.SetActive(false);
        if (difficultyPopup != null) difficultyPopup.SetActive(true);
    }

    public void RefreshSlotLabels() {
        for (int i = 0; i < slotTexts.Length; i++) {
            string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (i + 1) + ".json";
            if (File.Exists(path)) {
                string json = File.ReadAllText(path);
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
                slotTexts[i].text = data.playerName;
            } else {
                slotTexts[i].text = "New Game";
            }
        }
    }

    public void OnSlotClicked(int id) {
        slotIndexToProcess = id;
        GameManager.Instance.selectedSlot = id;
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (id + 1) + ".json";
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            GameManager.Instance.totalCoins = data.coins;
            GameManager.Instance.playerName = data.playerName;
            GameManager.Instance.selectedFarmID = data.farmID;
            loadPopup.SetActive(true);
            if (loadPopupText != null) loadPopupText.text = "Do you want to load " + data.playerName + "?";
        } else {
            GameManager.Instance.ResetData();
            SceneManager.LoadScene("PlayerCreation");
        }
    }

    public void ConfirmLoad() {
        GameManager.Instance.selectedSlot = slotIndexToProcess;
        GameManager.Instance.LoadGameData();
        SceneManager.LoadScene("PlayerAndFarm");
    }

    public void OpenDeleteConfirmation(int id) {
        slotIndexToProcess = id;
        if (deletePopup != null) deletePopup.SetActive(true);
    }

    public void ConfirmDelete() {
        string path = Application.persistentDataPath + "/Saves/SaveSlot_" + (slotIndexToProcess + 1) + ".json";
        if (File.Exists(path)) {
            File.Delete(path);
            GameManager.Instance.ResetData();
            GameManager.Instance.SyncToBrowser(); // <--- Tell browser storage the file was deleted!
        }
        if (deletePopup != null) deletePopup.SetActive(false);
        RefreshSlotLabels();
    }

    public void OnDifficultyClicked(string difficulty) => GameManager.Instance.SelectDifficultyAndStart(difficulty);

    public void CloseAllPopups() {
        if (deletePopup) deletePopup.SetActive(false);
        if (loadPopup) loadPopup.SetActive(false);
    }
}