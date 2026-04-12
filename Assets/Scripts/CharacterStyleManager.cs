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
    
    // --- RECONSTRUCTION LOGIC ---
    [Header("Default Sprites (Drag and drop images here)")]
    public Sprite defaultHairSprite; 
    public Sprite defaultTopSprite; 
    public Sprite defaultBottomSprite; 
    public Sprite defaultSkinSprite;
    
    [Header("Identity")]
    public TMP_InputField nameInput; // Drag your 'Type Your Name' box here
    public TextMeshProUGUI warningText; // Optional: A small red text that says "Enter Name!"


    void Start()
    {
        CloseMenu();        
        SetDefaultStyle();  
    }


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
            string rawName = s.name;
            string prefix = rawName.Split('_')[0].ToLower(); // Handles both 'hairstyle1_000' and 'hairstyle1_0' by taking the part before the underscore and making it lowercase
            
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
            if (spriteName.EndsWith("_0")) spriteName = spriteName.Substring(0, spriteName.Length - 2); // Handles 'hairstyle1_0' -> 'hairstyle1'
            

            GameObject localBox = targetBox.gameObject;
            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                EquipItem(localBox, spriteName, activeContainer, folderName); // Pass the original folderName to ensure correct path in EquipItem
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

    // 1. Fill the data
    data.playerName = GameManager.Instance.playerName;
        data.coins = GameManager.Instance.totalCoins;
        data.farmID = 0; // You can set this based on the player's choice later

    // 2. Get the clean names of what is currently equipped
        data.hairName = GetActiveSpriteName(hairContainer);
        data.topName = GetActiveSpriteName(topsContainer);
        data.bottomName = GetActiveSpriteName(bottomsContainer);
        data.skinName = GetActiveSpriteName(bodyContainer);

    // 3. Accessories
        data.hasGlasses = (glassesObj != null) ? glassesObj.activeSelf : false;
        data.hasHearingAid = (hearingAidObj != null) ? hearingAidObj.activeSelf : false;
        data.hasCrutches = (crutchesObj != null) ? crutchesObj.activeSelf : false;

    // 4. CALL THE MASTER SAVE (Instead of manual File writing)
        // This uses the GameManager's SaveGame function we built earlier
        GameManager.Instance.SaveGame(data);

    Debug.Log("<color=green>CharacterStyleManager:</color> Character choices saved for " + data.playerName);
}

    // Combined and refined ConfirmNameAndContinue function
    public void ConfirmNameAndContinue()
    {
        // 2. The Validation Check
        // .Trim() removes accidental spaces.
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            Debug.LogWarning("Player tried to continue without a name!");

            // Start the visual feedback for empty name input
            StartCoroutine(FlashNameBoxRed());
            nameInput.GetComponent<Animator>().SetTrigger("Shake"); // Trigger the shake animation

            // If you have a warning text, show it
            if (warningText != null)
            {
                warningText.text = "Please enter a name!";
                warningText.gameObject.SetActive(true);
            }

            // Optional: You could play a 'buzzer' sound effect here
            return;
        }

        // 3. If they passed the check, Save and Move on!
        // Update the GameManager with the player's name before saving
        GameManager.Instance.playerName = nameInput.text.Trim();
        SaveAndProceedToFarm();
    }

    // Coroutine to visually indicate an invalid name input by flashing the input field red.
    IEnumerator FlashNameBoxRed()
    {
        // Store the original color to revert to
        Color originalColor = nameInput.image.color;

        // 1. Turn the box red
        nameInput.image.color = Color.red;

        // 2. Wait for a split second
        yield return new WaitForSeconds(0.5f);

        // 3. Smoothly fade back to the original color
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f; // Speed of the fade
            nameInput.image.color = Color.Lerp(Color.red, originalColor, t);
            yield return null;
        }
        // Ensure the color is exactly the original color after the lerp
        nameInput.image.color = originalColor;
    }

    // Saves the character choices and proceeds to the farm scene.
    void SaveAndProceedToFarm()
    {
        // Ensure player name is saved to GameManager before saving to file
        if (GameManager.Instance != null && string.IsNullOrWhiteSpace(GameManager.Instance.playerName))
        {
            GameManager.Instance.playerName = nameInput.text.Trim();
        }

        // Call the actual save logic
        SaveCharacterChoices();

        Debug.Log("Character " + nameInput.text.Trim() + " saved! Loading Farm...");
        SceneManager.LoadScene("FarmSelection"); // Change this to your next scene name
    }

private string GetActiveSpriteName(Transform container)
    {
        foreach (Transform child in container)
        {
            if (child.gameObject.activeSelf)
            {
                Image img = child.GetComponent<Image>();
                if (img != null && img.sprite != null) 
                {
                    string rawName = img.sprite.name;

                    // This removes the Unity-generated "_0" but keeps the variant number
                    // e.g. "hairstyle1__003_0" becomes "hairstyle1__003"
                    if (rawName.EndsWith("_0"))
                    {
                        return rawName.Substring(0, rawName.Length - 2);
                    }

                    return rawName; 
                }
            }
        }
        return "";
    }

    public void ToggleLayer(GameObject layerObject) { layerObject.SetActive(!layerObject.activeSelf); }
    public void CloseMenu() { popupWindow.SetActive(false); if(hairScrollView != null) hairScrollView.SetActive(false); }
    public void ResetPlayerStyles() { SetDefaultStyle(); }
}