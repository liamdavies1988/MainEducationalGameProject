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
using UnityEngine.SceneManagement;

public class FarmSelector : MonoBehaviour
{
    [Header("UI Visuals")]
    public Sprite[] farmOptions; // Drag your 3 JPEGs here
    public Image displayImage;

    private int currentFarmIndex = 0;

    // --- Unity Callbacks ---

    void Start()
    {
        // Set the initial preview image if options are available
        if (farmOptions.Length > 0)
        {
            displayImage.sprite = farmOptions[0];
        }
    }

    // --- Navigation Logic ---

    public void NextFarm()
    {
        // Increment index and wrap around to the start if necessary
        currentFarmIndex++;
        if (currentFarmIndex >= farmOptions.Length)
        {
            currentFarmIndex = 0;
        }
        UpdateUI();
    }

    public void PrevFarm()
    {
        // Decrement index and wrap around to the end if necessary
        currentFarmIndex--;
        if (currentFarmIndex < 0)
        {
            currentFarmIndex = farmOptions.Length - 1;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Apply the selected sprite to the display component
        displayImage.sprite = farmOptions[currentFarmIndex];
    }

    // --- Data Persistence ---

    public void ConfirmAndStartGame()
    {
        // Identify the current save slot and prepare the file path
        int slot = GameManager.Instance.selectedSlot;
        string fileName = "SaveSlot_" + (slot + 1) + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, "Saves", fileName);

        if (File.Exists(filePath))
        {
            // Load existing profile data to append the farm choice
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Update the farm ID and save the modified JSON back to disk
            data.farmID = currentFarmIndex;
            string updatedJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, updatedJson);

            // Transition to the main game environment scene
            SceneManager.LoadScene("PlayerAndFarm");
        }
    }
}