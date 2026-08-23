// =================================================================================================
// File: FarmSelector.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: March 20, 2026
// Last Modified: April 20, 2026
//
// Description:
// Manages the farm selection interface, allowing users to cycle through available 
// farm environments and saving the choice to their persistent profile.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class FarmSelector : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CommitInternalSave();
#endif

    [Header("UI Visuals")]
    public Sprite[] farmOptions; 
    public Image displayImage;
    private int currentFarmIndex = 0;

    void Start() { if (farmOptions.Length > 0) displayImage.sprite = farmOptions[0]; }

    public void NextFarm() {
        currentFarmIndex = (currentFarmIndex + 1) % farmOptions.Length;
        UpdateUI();
    }

    public void PrevFarm() {
        currentFarmIndex = (currentFarmIndex - 1 + farmOptions.Length) % farmOptions.Length;
        UpdateUI();
    }

    private void UpdateUI() => displayImage.sprite = farmOptions[currentFarmIndex];

    public void ConfirmAndStartGame() {
        int slot = GameManager.Instance.selectedSlot;
        string filePath = Path.Combine(Application.persistentDataPath, "Saves", "SaveSlot_" + (slot + 1) + ".json");

        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            data.farmID = currentFarmIndex;
            File.WriteAllText(filePath, JsonUtility.ToJson(data, true));

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                CommitInternalSave();
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Browser sync skipped.");
            }
#endif

            SceneManager.LoadScene("PlayerAndFarm");
        }
    }
}