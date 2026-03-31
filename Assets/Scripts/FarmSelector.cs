using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FarmSelector : MonoBehaviour
{
    [Header("Farm Visuals")]
    public Sprite[] farmOptions; // Drag your 3 farm drawings here
    public Image farmDisplay;

    private int currentFarmIndex = 0;

    void Start()
    {
        // Set the first farm as default
        if(farmOptions.Length > 0) farmDisplay.sprite = farmOptions[0];
    }

    public void NextFarm() { ChangeFarm(1); }
    public void PrevFarm() { ChangeFarm(-1); }

    private void ChangeFarm(int direction)
    {
        currentFarmIndex += direction;
        if (currentFarmIndex < 0) currentFarmIndex = farmOptions.Length - 1;
        if (currentFarmIndex >= farmOptions.Length) currentFarmIndex = 0;

        farmDisplay.sprite = farmOptions[currentFarmIndex];
    }

    public void ConfirmAndStartGame()
    {
        // 1. Load the current data from the JSON file
        int slot = GameManager.Instance.selectedSlot;
        string filePath = Application.dataPath + "/Saves/SaveSlot_" + (slot + 1) + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // 2. Update ONLY the farm ID
            data.farmID = currentFarmIndex;

            // 3. Save it back to the file
            string updatedJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, updatedJson);

            Debug.Log("Farm Choice Saved: " + currentFarmIndex);

            // 4. Move to the Main Menu or the Game
            SceneManager.LoadScene("MainMenu");
        }
    }
}