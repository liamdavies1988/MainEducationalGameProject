using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StyleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject woodButtonPrefab;
    public GameObject popupWindow;      // The main wooden frame
    public GameObject hairScrollView;   // MISSING VARIABLE ADDED HERE
    public Transform itemGrid;          // The 'Content' of your single Scroll View

    [Header("Character Containers")]
    public Transform hairContainer;
    public Transform topsContainer;
    public Transform bottomsContainer;

    void Start()
    {
        // Ensure the menu is closed when the game starts
        CloseMenu();
    }

    // --- UNIVERSAL MENU BUILDER ---
    public void OpenMenu(string folderName)
    {
        // 1. Clear old buttons
        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        // 2. Map the Folder Name to the right Hierarchy Container
        Transform activeContainer = null;
        if (folderName == "HairStyles") activeContainer = hairContainer;
        else if (folderName == "Tops") activeContainer = topsContainer;
        else if (folderName == "Bottoms") activeContainer = bottomsContainer;

        if (activeContainer == null)
        {
            Debug.LogError("Menu Error: No container found for " + folderName);
            return;
        }

        // 3. Load the sprites from: Resources/Images/CharacterItems/...
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/CharacterItems/" + folderName);

        foreach (Sprite s in sprites)
        {
            // --- THE SMART PREFIX LOGIC ---
            string prefix = s.name.Split('_')[0];

            Transform targetBox = null;
            foreach (Transform child in activeContainer)
            {
                if (child.name.ToLower() == prefix.ToLower())
                {
                    targetBox = child;
                    break;
                }
            }

            if (targetBox == null)
            {
                Debug.LogWarning("Skipping " + s.name + " - No matching box found in " + activeContainer.name);
                continue;
            }

            // 4. Create the Button
            GameObject newBtn = Instantiate(woodButtonPrefab, itemGrid);

            // Set the icon (Assumes child inside prefab is named 'HairIcon')
            Image icon = newBtn.transform.Find("HairIcon").GetComponent<Image>();
            if (icon != null) icon.sprite = s;

            // 5. Setup the Click Logic
            Sprite localSprite = s;
            GameObject localBox = targetBox.gameObject;
            Transform localContainer = activeContainer;

            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                EquipItem(localBox, localSprite, localContainer);
            });
        }

        // Show the window
        if (popupWindow != null) popupWindow.SetActive(true);
        if (hairScrollView != null) hairScrollView.SetActive(true);
    }

    void EquipItem(GameObject box, Sprite itemSprite, Transform container)
    {
        foreach (Transform t in container) { t.gameObject.SetActive(false); }
        box.SetActive(true);
        Image boxImage = box.GetComponent<Image>();
        if (boxImage != null) { boxImage.sprite = itemSprite; }
    }

    // ONE MASTER CLOSE FUNCTION
    public void CloseMenu()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
        if (hairScrollView != null) hairScrollView.SetActive(false);
        Debug.Log("UI: Style Menu Closed.");
    }
}