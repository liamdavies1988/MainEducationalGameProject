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

            // 1. Sync Text and GameManager
            if (nameLabel != null) nameLabel.text = data.playerName;
            if (coinLabel != null) coinLabel.text = data.coins.ToString();
            GameManager.Instance.totalCoins = data.coins;
            GameManager.Instance.playerName = data.playerName;
            GameManager.Instance.selectedFarmID = data.farmID;

            // 2. Register Faces with the Brain
            if (GameManager.Instance != null)
            {
                GameManager.Instance.smileFace = sceneSmile;
                GameManager.Instance.sadFace = sceneSad;
            }

            // 3. Reconstruct Visuals
            ReconstructPart(hairContainer, "HairStyles", data.hairName);
            ReconstructPart(topsContainer, "Tops", data.topName);
            ReconstructPart(bottomsContainer, "Bottoms", data.bottomName);
            ReconstructPart(bodyContainer, "SkinTone", data.skinName);

            // 4. Set Accessories
            if (glassesObj != null) glassesObj.SetActive(data.hasGlasses);
            if (hearingAidObj != null) hearingAidObj.SetActive(data.hasHearingAid);
            if (crutchesObj != null) crutchesObj.SetActive(data.hasCrutches);

            // 5. THE PREFAB FIX: Spawn the farm world
            if (farmPrefabs != null && farmPrefabs.Length > data.farmID)
            {
                // Create the farm as a child of the Studio at X:1000
                GameObject activeFarm = Instantiate(farmPrefabs[data.farmID], farmStudioParent);
                activeFarm.transform.localPosition = Vector3.zero; // Center it
                Debug.Log("<color=green>Loader:</color> Spawned Farm Type " + data.farmID);
            }

            // 6. Spawn the Animals
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
