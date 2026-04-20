// =================================================================================================
// File: CharacterStyleManager.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: March 21, 2026
// Last Modified: April 20, 2026
//
// Description:
// Manages the character customization interface, including dynamic menu generation, 
// item equipment, name validation, and persistent storage of visual choices.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Accessory Objects")]
    public GameObject glassesObj;
    public GameObject hearingAidObj;
    public GameObject crutchesObj;

    [Header("Default Settings")]
    public string defaultHairName = "hairstyle1_000"; 
    public string defaultTopName = "tshirt_0"; 
    public string defaultBottomName = "shorts_0"; 
    public string defaultSkinToneName = "body_0"; 
    
    [Header("Default Sprites")]
    public Sprite defaultHairSprite; 
    public Sprite defaultTopSprite; 
    public Sprite defaultBottomSprite; 
    public Sprite defaultSkinSprite;
    
    [Header("Identity Components")]
    public TMP_InputField nameInput; 
    public TextMeshProUGUI warningText; 

    // --- Initialization Logic ---

    void Start()
    {
        // Hide menus and apply base character appearance on load
        CloseMenu();        
        SetDefaultStyle();  
    }

    public void SetDefaultStyle()
    {
        // Apply inspector-assigned sprites to the character containers
        if (defaultHairSprite != null) ApplyDefaultPart(hairContainer, defaultHairSprite, "Hair");
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
            
            // Enable the child object if it matches the sprite naming convention
            if (spriteName.Contains(boxName) || boxName.Contains(spriteName))
            {
                child.gameObject.SetActive(true);
                child.GetComponent<Image>().sprite = sprite;
                foundBox = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (!foundBox) Debug.LogError("CharacterStyleManager: No box match found for " + spriteName);
    }

    // --- Customization Menu Logic ---

    public void OpenMenu(string folderName)
    {
        // Clear previous grid items and activate menu UI
        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        Transform activeContainer = null;
        if (folderName == "HairStyles") activeContainer = hairContainer;
        else if (folderName == "Tops") activeContainer = topsContainer;
        else if (folderName == "Bottoms") activeContainer = bottomsContainer;
        else if (folderName == "SkinTone") activeContainer = bodyContainer;

        if (activeContainer == null) return;
        
        if (!popupWindow.activeSelf) popupWindow.SetActive(true);
        if (hairScrollView != null) hairScrollView.SetActive(true);

        string buttonPath = "Images/CharacterItems/" + folderName;
        if (folderName == "SkinTone") buttonPath += "/Icons";

        Sprite[] sprites = Resources.LoadAll<Sprite>(buttonPath);

        foreach (Sprite s in sprites)
        {
            // Identify corresponding character part via prefix naming
            string rawName = s.name;
            string prefix = rawName.Split('_')[0].ToLower(); 
            
            Transform targetBox = null;
            foreach (Transform child in activeContainer)
            {
                if (child.name.ToLower() == prefix) { targetBox = child; break; }
            }

            if (targetBox == null) continue;

            // Instantiate button and assign icon/click listeners
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
        // Load full-resolution sprite and apply it to the selected character part
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

    // --- Save and Navigation Logic ---

    public void ConfirmNameAndContinue()
    {
        // Validate that the player has entered a non-empty name
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            StartCoroutine(FlashNameBoxRed());
            nameInput.GetComponent<Animator>().SetTrigger("Shake"); 

            if (warningText != null)
            {
                warningText.text = "Please enter a name!";
                warningText.gameObject.SetActive(true);
            }
            return;
        }

        GameManager.Instance.playerName = nameInput.text.Trim();
        SaveAndProceedToFarm();
    }

    IEnumerator FlashNameBoxRed()
    {
        Color originalColor = nameInput.image.color;
        nameInput.image.color = Color.red;
        
        yield return new WaitForSeconds(0.5f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            nameInput.image.color = Color.Lerp(Color.red, originalColor, t);
            yield return null;
        }
        nameInput.image.color = originalColor;
    }

    void SaveAndProceedToFarm()
    {
        SaveCharacterChoices();
        SceneManager.LoadScene("FarmSelection"); 
    }

    public void SaveCharacterChoices()
    {
        // Package current customization into the persistent save data format
        PlayerSaveData data = new PlayerSaveData();
        data.playerName = GameManager.Instance.playerName;
        data.coins = GameManager.Instance.totalCoins;
        data.farmID = GameManager.Instance.selectedFarmID; 

        data.hairName = GetActiveSpriteName(hairContainer, "Hair");
        data.topName = GetActiveSpriteName(topsContainer, "Top");
        data.bottomName = GetActiveSpriteName(bottomsContainer, "Bottom");
        data.skinName = GetActiveSpriteName(bodyContainer, "Skin");

        data.hasGlasses = (glassesObj != null) && glassesObj.activeSelf;
        data.hasHearingAid = (hearingAidObj != null) && hearingAidObj.activeSelf;
        data.hasCrutches = (crutchesObj != null) && crutchesObj.activeSelf;

        data.activeAnimals = new List<string>(GameManager.Instance.activeAnimals);

        GameManager.Instance.SaveGame(data);
    }

    private string GetActiveSpriteName(Transform container, string category)
{
    if (container == null) 
    {
        Debug.LogError("SAVE ERROR: " + category + " container is missing in the Inspector!");
        return "";
    }

    foreach (Transform child in container)
    {
        if (child.gameObject.activeSelf)
        {
            Image img = child.GetComponent<Image>();
            if (img != null && img.sprite != null) 
            {
                string rawName = img.sprite.name;

                // Handle resource naming conventions (stripping suffixes where necessary)
                if (rawName.EndsWith("_0")) 
                {
                    if (category.ToLower().Contains("skin")) 
                    {
                        return rawName; 
                    }
                    else 
                    {
                        return rawName.Substring(0, rawName.Length - 2);
                    }
                }
                return rawName; 
            }
        }
    }
    return "";
}

    // --- UI Utility Methods ---

    public void ToggleLayer(GameObject layerObject) { layerObject.SetActive(!layerObject.activeSelf); }
    public void CloseMenu() { popupWindow.SetActive(false); if(hairScrollView != null) hairScrollView.SetActive(false); }
    public void ResetPlayerStyles() { SetDefaultStyle(); }
}