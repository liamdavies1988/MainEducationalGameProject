using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class CharacterStyleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject woodButtonPrefab;      
    public GameObject popupWindow;           
    public GameObject hairScrollView;        
    public Transform itemGrid;               

    [Header("Character Containers")]
    public Transform hairContainer;          
    public Transform topsContainer;          
    public Transform bottomsContainer;       
    public Transform bodyContainer;          

    [Header("Accessory Objects (For Saving)")]
    public GameObject glassesObj;
    public GameObject hearingAidObj;
    public GameObject crutchesObj;

    [Header("Default Settings")]
    // SYNCED NAMES: These now match the function below
    public string defaultHairName = "hairstyle1_000"; 
    public string defaultTopName = "tshirt_0"; 
    public string defaultBottomName = "shorts_0"; 
    public string defaultSkinToneName = "body_0"; 

    void Start()
    {
        CloseMenu();        
        SetDefaultStyle();  
    }

    // --- RECONSTRUCTION LOGIC ---

    [Header("Default Sprites (Drag and drop images here)")]
public Sprite defaultHairSprite; 
public Sprite defaultTopSprite; 
public Sprite defaultBottomSprite; 
public Sprite defaultSkinSprite; 

public void SetDefaultStyle()
{
    Debug.Log("UI: Resetting character using Inspector sprites...");

    // 1. We skip the 'Resources.Load' part because we already have the sprites!
    
    // 2. Apply them using our helper
    if (defaultHairSprite != null) ApplyDefaultPart(hairContainer, defaultHairSprite, "Hair");
    else Debug.LogWarning("Hair Reset skipped: defaultHairSprite is empty in Inspector!");

    if (defaultTopSprite != null) ApplyDefaultPart(topsContainer, defaultTopSprite, "Tops");
    if (defaultBottomSprite != null) ApplyDefaultPart(bottomsContainer, defaultBottomSprite, "Bottoms");
    if (defaultSkinSprite != null) ApplyDefaultPart(bodyContainer, defaultSkinSprite, "Skin");
}

    private void ApplyDefaultPart(Transform container, Sprite sprite, string categoryName)
    {
        if (sprite == null || container == null) return;

        string spriteName = sprite.name.ToLower();
        bool foundBox = false;

        foreach (Transform child in container)
        {
            string boxName = child.name.ToLower();

            // FUZZY MATCH: Handles cases like 'hairstyle1_000' matching 'hairstyle1'
            if (spriteName.Contains(boxName) || boxName.Contains(spriteName))
            {
                child.gameObject.SetActive(true);
                child.GetComponent<Image>().sprite = sprite;
                foundBox = true;
                Debug.Log("<color=green>RESET SUCCESS:</color> Set " + child.name + " as active " + categoryName);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (!foundBox) 
            Debug.LogError("<color=red>RESET FAILED:</color> Could not find any box that matches the name: " + spriteName);
    }

    // --- MENU GENERATION ---

    public void OpenMenu(string folderName)
    {
        if (!popupWindow.activeSelf) popupWindow.SetActive(true);
        if (hairScrollView != null) hairScrollView.SetActive(true);

        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        Transform activeContainer = null;
        if (folderName == "HairStyles") activeContainer = hairContainer;
        else if (folderName == "Tops") activeContainer = topsContainer;
        else if (folderName == "Bottoms") activeContainer = bottomsContainer;
        else if (folderName == "SkinTone") activeContainer = bodyContainer;

        if (activeContainer == null) return;

        // Path to load button icons
        string buttonPath = "Images/CharacterItems/" + folderName;
        if (folderName == "SkinTone") buttonPath += "/Icons";

        Sprite[] sprites = Resources.LoadAll<Sprite>(buttonPath);

        foreach (Sprite s in sprites)
        {
            string prefix = s.name.Split('_')[0].ToLower();
            Transform targetBox = null;
            foreach (Transform child in activeContainer)
            {
                if (child.name.ToLower() == prefix) { targetBox = child; break; }
            }

            if (targetBox == null) continue;

            GameObject newBtn = Instantiate(woodButtonPrefab, itemGrid);
            Image icon = newBtn.transform.Find("HairIcon").GetComponent<Image>();
            if (icon != null) icon.sprite = s;

            string spriteName = s.name;
            if (spriteName.EndsWith("_0")) spriteName = spriteName.Substring(0, spriteName.Length - 2);

            GameObject localBox = targetBox.gameObject;
            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                EquipItem(localBox, spriteName, activeContainer, folderName);
            });
        }
    }

    void EquipItem(GameObject box, string spriteName, Transform container, string folderName)
    {
        string realPath = "Images/CharacterItems/" + folderName + "/" + spriteName;
        Sprite realSprite = Resources.Load<Sprite>(realPath);

        if (realSprite == null) return;

        if (folderName != "SkinTone")
        {
            foreach (Transform t in container) { t.gameObject.SetActive(false); }
        }

        box.SetActive(true);
        Image boxImage = box.GetComponent<Image>();
        if (boxImage != null) { boxImage.sprite = realSprite; }
    }

    // --- SAVE LOGIC ---

    public void SaveCharacterChoices()
    {
        PlayerSaveData data = new PlayerSaveData();

        data.playerName = GameManager.Instance.playerName;
        data.coins = GameManager.Instance.totalCoins;
        data.farmID = GameManager.Instance.selectedSlot; 

        data.hairName = GetActiveSpriteName(hairContainer);
        data.topName = GetActiveSpriteName(topsContainer);
        data.bottomName = GetActiveSpriteName(bottomsContainer);
        data.skinName = GetActiveSpriteName(bodyContainer);

        if (glassesObj != null) data.hasGlasses = glassesObj.activeSelf;
        if (hearingAidObj != null) data.hasHearingAid = hearingAidObj.activeSelf;
        if (crutchesObj != null) data.hasCrutches = crutchesObj.activeSelf;

        string json = JsonUtility.ToJson(data, true);
        string path = Application.dataPath + "/Saves/SaveSlot_" + (GameManager.Instance.selectedSlot + 1) + ".json";
        File.WriteAllText(path, json);

        Debug.Log("SAVE SUCCESS: " + data.playerName + " stored in Slot " + (GameManager.Instance.selectedSlot + 1));
    }

    private string GetActiveSpriteName(Transform container)
    {
        foreach (Transform child in container)
        {
            if (child.gameObject.activeSelf)
            {
                Image img = child.GetComponent<Image>();
                if (img != null && img.sprite != null) return img.sprite.name;
            }
        }
        return "";
    }

    public void ToggleLayer(GameObject layerObject) { layerObject.SetActive(!layerObject.activeSelf); }
    public void CloseMenu() { popupWindow.SetActive(false); if(hairScrollView != null) hairScrollView.SetActive(false); }
    public void ResetPlayerStyles() { SetDefaultStyle(); }
}

[System.Serializable]
public class PlayerSaveData
{
    public string playerName;
    public int coins;
    public int farmID;
    public string hairName;
    public string topName;
    public string bottomName;
    public string skinName;
    public bool hasGlasses;
    public bool hasHearingAid;
    public bool hasCrutches;
}