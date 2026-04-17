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

    [Header("World UI")]
    public TextMeshProUGUI nameLabel;
    public GameObject[] farmLayouts;

    [Header("Economy UI")]
    public TextMeshProUGUI coinLabel; 

    void Start()
    {
        LoadPlayerAndWorld();
    }

    public void LoadPlayerAndWorld()
    {
        int slotID = GameManager.Instance.selectedSlot;
        string fileName = "SaveSlot_" + (slotID + 1) + ".json";
        string filePath = Path.Combine(Application.persistentDataPath, "Saves", fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // 1. Reconstruct Name and Coins
            if (nameLabel != null) nameLabel.text = data.playerName;
            if (coinLabel != null) coinLabel.text = data.coins.ToString();
            GameManager.Instance.totalCoins = data.coins;

            // 2. Reconstruct Visuals (Clothing/Skin)
            ReconstructPart(hairContainer, "HairStyles", data.hairName);
            ReconstructPart(topsContainer, "Tops", data.topName);
            ReconstructPart(bottomsContainer, "Bottoms", data.bottomName);
            ReconstructPart(bodyContainer, "SkinTone", data.skinName);

            // 3. Reconstruct Environment
            if (farmLayouts != null && farmLayouts.Length > 0)
            {
                for (int i = 0; i < farmLayouts.Length; i++)
                {
                    farmLayouts[i].SetActive(i == data.farmID);
                }
            }

            // 4. THE ANIMAL FIX (Stop the Zoom/Crash)
            RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();

            // GUARD: Only loop if the list exists and isn't empty
            if (data.activeAnimals != null && data.activeAnimals.Count > 0 && rm != null)
            {
                foreach (string animalSpriteName in data.activeAnimals)
                {
                    // We use "SavedAnimal" as the name to distinguish it from a fresh buy
                    rm.SpawnAnimalWithData("SavedAnimal", animalSpriteName);
                }
                Debug.Log("<color=green>Loader:</color> Successfully restored " + data.activeAnimals.Count + " animals.");
            }
        }
    }

    private void ReconstructPart(Transform container, string folder, string assetName)
{
    if (container == null || string.IsNullOrEmpty(assetName)) return;

    // 1. Find the "Box" (hairstyle1__000_0 -> hairstyle1)
    string prefix = assetName.Split('_')[0].ToLower();

    // 2. CLEAN the asset name for loading (hairstyle1__000_0 -> hairstyle1__000)
    // We remove the very last "_0" which Unity adds to the sprite object name
    string cleanAssetName = assetName;
    if (assetName.EndsWith("_0")) 
    {
        cleanAssetName = assetName.Substring(0, assetName.Length - 2);
    }

    // 3. LOAD the sprite using the clean name
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
                // This will tell you if the path is still slightly wrong
                Debug.LogError("LOADER: Image not found at Resources/" + path);
            }
        }
        else if (folder != "SkinTone") 
        {
            box.gameObject.SetActive(false);
        }
    }
}
}

// --- RECENTLY EDITED FILES ---