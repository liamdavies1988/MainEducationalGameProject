// =================================================================================================
// File: CharacterLoader.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: March 20, 2026
// Last Modified: April 20, 2026
//
// Description:
// Responsible for reconstructing the player character's visual appearance and spawning 
// the selected farm environment upon scene load using persistent JSON save data.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class CharacterLoader : MonoBehaviour
{
    [Header("Hierarchy Containers (Hidden Boxes)")]
    public Transform hairContainer;
    public Transform topsContainer;
    public Transform bottomsContainer;
    public Transform bodyContainer;

    [Header("Accessory Objects")]
    public GameObject glassesObj;
    public GameObject hearingAidObj;
    public GameObject crutchesObj;

    [Header("Face References")]
    public GameObject sceneSmile;
    public GameObject sceneSad;

    [Header("World UI")]
    public TextMeshProUGUI nameLabel;

    [Header("Economy UI")]
    public TextMeshProUGUI coinLabel; 

    [Header("Farm Prefabs")]
    public GameObject[] farmPrefabs; // Drag your 3 blue prefab assets here
    public Transform farmStudioParent; // Drag your 'FARM_STUDIO' object here (at X:1000)

    // --- Unity Callbacks ---

    void Start()
    {
        // Brief delay ensures the GameManager has finished its internal initialization
        Invoke(nameof(LoadPlayerAndWorld), 0.1f); 
    }

    // --- Loading Logic ---

    public void LoadPlayerAndWorld()
    {
        // Locate the save file based on the slot selected in the menu
        int slotID = GameManager.Instance.selectedSlot;
        string fileName = "SaveSlot_" + (slotID + 1) + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, "Saves", fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Sync UI text elements and update GameManager session data
            if (nameLabel != null) nameLabel.text = data.playerName;
            if (coinLabel != null) coinLabel.text = data.coins.ToString();
            GameManager.Instance.totalCoins = data.coins;
            GameManager.Instance.playerName = data.playerName;
            GameManager.Instance.selectedFarmID = data.farmID;

            // Connect local face objects to the GameManager for feedback animations
            if (GameManager.Instance != null)
            {
                GameManager.Instance.smileFace = sceneSmile;
                GameManager.Instance.sadFace = sceneSad;
            }

            // Apply the saved character appearance
            ReconstructPart(hairContainer, "HairStyles", data.hairName);
            ReconstructPart(topsContainer, "Tops", data.topName);
            ReconstructPart(bottomsContainer, "Bottoms", data.bottomName);
            ReconstructPart(bodyContainer, "SkinTone", data.skinName);

            // Toggle accessory visibility based on save data
            if (glassesObj != null) glassesObj.SetActive(data.hasGlasses);
            if (hearingAidObj != null) hearingAidObj.SetActive(data.hasHearingAid);
            if (crutchesObj != null) crutchesObj.SetActive(data.hasCrutches);

            // Instantiate the correct farm environment based on the saved ID
            if (farmPrefabs != null && farmPrefabs.Length > data.farmID)
            {
                GameObject activeFarm = Instantiate(farmPrefabs[data.farmID], farmStudioParent);
                activeFarm.transform.localPosition = Vector3.zero;
            }

            // Repopulate the farm with previously purchased animals
            RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();
            if (data.activeAnimals != null && rm != null)
            {
                foreach (string animalSpriteName in data.activeAnimals)
                {
                    rm.SpawnAnimalWithData("Animal", animalSpriteName);
                }
            }
        }
    }

    private void ReconstructPart(Transform container, string folder, string assetName)
    {
        if (container == null || string.IsNullOrEmpty(assetName)) return;

        // Extract the base prefix to identify which transform "box" to activate
        string prefix = assetName.Split('_')[0].ToLower();

        // Remove Unity's internal naming suffix for Resource loading compatibility
        string cleanAssetName = assetName;
        if (assetName.EndsWith("_0"))
        {
            cleanAssetName = assetName.Substring(0, assetName.Length - 2);
        }

        string path = "Images/CharacterItems/" + folder + "/" + cleanAssetName;
        Sprite s = Resources.Load<Sprite>(path);

        foreach (Transform box in container)
        {
            if (box.name.ToLower() == prefix)
            {
                box.gameObject.SetActive(true);
                if (s != null)
                {
                    box.GetComponent<Image>().sprite = s;
                    box.GetComponent<Image>().preserveAspect = true;
                }
                else
                {
                    Debug.LogError("CharacterLoader: Image not found at Resources/" + path);
                }
            }
            else if (folder != "SkinTone")
            {
                box.gameObject.SetActive(false);
            }
        }
    }
}
