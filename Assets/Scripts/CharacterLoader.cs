using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CharacterLoader : MonoBehaviour
{
    [Header("UI Character Displays")]
    public Transform hairContainer;
    public Transform topsContainer;
    public Transform bottomsContainer;
    public Transform bodyContainer;

    [Header("Accessory Objects")]
    public GameObject glassesObj;
    public GameObject hearingAidObj;
    public GameObject crutchesObj;

    [Header("Other UI")]
    public TextMeshProUGUI nameLabel;
    public GameObject[] farmLayouts;

    void Start()
    {
        LoadPlayerAndWorld();
    }

    public void LoadPlayerAndWorld()
    {
        int slotID = GameManager.Instance.selectedSlot;
        string filePath = Application.dataPath + "/Saves/SaveSlot_" + (slotID + 1) + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // 1. Set the Name
            if (nameLabel != null) nameLabel.text = data.playerName + "'s Farm";

            // 2. Reconstruct the Clothes and Skin (Dynamic Loading)
            ReconstructPart(hairContainer, "HairStyles", data.hairName);
            ReconstructPart(topsContainer, "Tops", data.topName);
            ReconstructPart(bottomsContainer, "Bottoms", data.bottomName);
            ReconstructPart(bodyContainer, "SkinTone", data.skinName);

            // 3. Set Accessories (Toggles)
            if (glassesObj != null) glassesObj.SetActive(data.hasGlasses);
            if (hearingAidObj != null) hearingAidObj.SetActive(data.hasHearingAid);
            if (crutchesObj != null) crutchesObj.SetActive(data.hasCrutches);

            // 4. Reconstruct the Farm
            for (int i = 0; i < farmLayouts.Length; i++)
            {
                farmLayouts[i].SetActive(i == data.farmID);
            }

            Debug.Log("Character and World Reconstructed successfully.");
        }
    }

    // This logic mirrors your StyleManager 'Equip' logic
    private void ReconstructPart(Transform container, string folder, string assetName)
    {
        if (string.IsNullOrEmpty(assetName)) return;

        // Load the sprite from Resources
        Sprite s = Resources.Load<Sprite>("Images/CharacterItems/" + folder + "/" + assetName);
        if (s == null) return;

        string prefix = assetName.Split('_')[0].ToLower();

        foreach (Transform child in container)
        {
            if (child.name.ToLower() == prefix)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<Image>().sprite = s;
            }
            else if (folder != "SkinTone") // Don't hide the base body!
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}