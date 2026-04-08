using UnityEngine;
using UnityEngine.UI;

public class StyleManager : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject woodButtonPrefab; 
    public GameObject popupWindow;      // The main wooden frame
    public GameObject hairScrollView;   // The hair scroll window
    public Transform hairGrid;          // The 'Content' object inside the scroll view

    [Header("Character - HAIR (The 6 Hidden Boxes)")]
    public Transform hairContainer;     // Drag the parent of your 6 Hidden Boxes here

    [Header("Folder Configuration")]
    public string hairFolderPath = "CharacterItems/HairStyles"; 

    void Start()
    {
        // 1. Build the menu using the Smart Prefix System
        BuildSmartMenu();

        // 2. Hide the popup menu at the start
        CloseAll();
    }

    void BuildSmartMenu()
    {
        // Clear the grid to avoid duplicates
        foreach (Transform child in hairGrid) { Destroy(child.gameObject); }

        // Load all sprites from the folder (it finds all 39 regardless of subfolders)
        Sprite[] sprites = Resources.LoadAll<Sprite>(hairFolderPath);

        foreach (Sprite s in sprites)
        {
            // 1. SPLIT THE NAME: hairstyle1_01 -> ["hairstyle1", "01"]
            // This assumes your files are named like: hairstyle1_blue, hairstyle2_red, etc.
            string[] nameParts = s.name.Split('_');
            string prefix = nameParts[0]; 

            // 2. FIND THE BOX: Look for the hidden object with the matching name
            Transform targetBox = hairContainer.Find(prefix);

            if (targetBox == null)
            {
                Debug.LogWarning("Skipping " + s.name + ". No hidden box found named: " + prefix);
                continue;
            }

            // 3. CREATE THE BUTTON
            GameObject newBtn = Instantiate(woodButtonPrefab, hairGrid);
            
            // Set the wooden button icon to match the hair sprite
            Image icon = newBtn.transform.Find("HairIcon").GetComponent<Image>();
            icon.sprite = s;
            icon.preserveAspect = true;

            // 4. CLICK LOGIC
            GameObject boxObj = targetBox.gameObject;
            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                EquipSmartHair(boxObj, s);
            });
        }

        // 5. FIX SCROLLING: Forces the grid to update its height so you can scroll
        LayoutRebuilder.ForceRebuildLayoutImmediate(hairGrid.GetComponent<RectTransform>());
    }

    void EquipSmartHair(GameObject box, Sprite hairSprite)
    {
        // Turn off all hidden hair boxes first
        foreach (Transform t in hairContainer) {
            if (t.name != "Body") t.gameObject.SetActive(false);
        }

        // Turn on the specific box (the "Shape")
        box.SetActive(true);

        // Change the sprite on that box (the "Color")
        Image boxImage = box.GetComponent<Image>();
        boxImage.sprite = hairSprite;
        boxImage.preserveAspect = true;
    }

    // --- UI POPUP CONTROLS ---
    
    public void OpenHairMenu() 
    {
        popupWindow.SetActive(true);
        hairScrollView.SetActive(true);
    }

    public void CloseAll() 
    {
        popupWindow.SetActive(false);
    }
}