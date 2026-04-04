using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FarmSelector : MonoBehaviour
{
    [Header("UI Visuals")]
    public Sprite[] farmOptions; // Drag your 3 JPEGs here
    public Image displayImage;

    private int currentFarmIndex = 0;

    void Start()
    {
        // Default to the first preview
        if(farmOptions.Length > 0) displayImage.sprite = farmOptions[0];
    }

    // --- NAVIGATION LOGIC ---
    public void NextFarm()
    {
        currentFarmIndex++;
        if (currentFarmIndex >= farmOptions.Length) currentFarmIndex = 0;
        UpdateUI();
    }

    public void PrevFarm()
    {
        currentFarmIndex--;
        if (currentFarmIndex < 0) currentFarmIndex = farmOptions.Length - 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        displayImage.sprite = farmOptions[currentFarmIndex];
    }

    // --- SAVING LOGIC ---
    public void ConfirmAndStartGame()
{
    // 1. Ask GameManager which slot we are currently using
    int slot = GameManager.Instance.selectedSlot;
    string fileName = "SaveSlot_" + (slot + 1) + ".json";
    string filePath = Application.dataPath + "/Saves/" + fileName;

    if (File.Exists(filePath))
    {
        // 2. Read the existing file (which has your Name and Character info)
        string json = File.ReadAllText(filePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        // 3. Update the farmID with the one the student just picked
        data.farmID = currentFarmIndex;

        // 4. Save the "Updated" file back to the folder
        string updatedJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, updatedJson);

        Debug.Log("Farm details added to " + fileName + ". Now moving to Rewards Scene.");

        // 5. THIS IS THE LINE THAT CHANGES THE SCREEN
       
        SceneManager.LoadScene("PlayerAndFarm");
    }
}
}