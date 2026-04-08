using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StyleManager : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject woodButtonPrefab;
    public GameObject popupWindow;
    public GameObject hairScrollView;
    public Transform hairGrid; // Ensure this is the 'Content' object in Unity

    [Header("Character - HAIR (The 6 Hidden Boxes)")]
    public Transform hairContainer;

    [Header("Folder Configuration")]
    public string hairFolderPath = "CharacterItems/HairStyles";

    void Start()
    {
        BuildSmartMenu();
        CloseAll();
    }

    void BuildSmartMenu()
    {
        foreach (Transform child in hairGrid) { Destroy(child.gameObject); }

        Sprite[] sprites = Resources.LoadAll<Sprite>(hairFolderPath);
        Debug.Log("Found " + sprites.Length + " hair sprites.");

        foreach (Sprite s in sprites)
        {
            // 1. Get the prefix (e.g., 'hairstyle1')
            string prefix = s.name.Split('_')[0];

            // 2. Find the matching object on the player
            Transform targetBox = hairContainer.Find(prefix);

            // SAFETY CHECK: If the name doesn't match an object in your Hierarchy, skip it!
            if (targetBox == null)
            {
                Debug.LogWarning("Object " + prefix + " not found under HairContainer. Check your names!");
                continue;
            }

            // 3. Create the Button
            GameObject newBtn = Instantiate(woodButtonPrefab, hairGrid);

            // Find the image on the button template (Assumes name is 'HairIcon')
            Image icon = newBtn.transform.Find("HairIcon").GetComponent<Image>();
            if (icon != null) icon.sprite = s;

            // 4. THE CAPTURE FIX (Prevents clicking the wrong item)
            Sprite localSprite = s;
            GameObject localBox = targetBox.gameObject;

            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                EquipSmartHair(localBox, localSprite);
            });
        }
    }

    void EquipSmartHair(GameObject box, Sprite hairSprite)
    {
        // Safety: ensure the box still exists
        if (box == null) return;

        // Hide all hair styles first
        foreach (Transform t in hairContainer)
        {
            t.gameObject.SetActive(false);
        }

        // Show selected and update color
        box.SetActive(true);
        Image boxImage = box.GetComponent<Image>();
        if (boxImage != null) boxImage.sprite = hairSprite;
    }

    public void OpenHairMenu()
    {
        popupWindow.SetActive(true);
        hairScrollView.SetActive(true);
    }

    public void CloseAll()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
    }
}