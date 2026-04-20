// =================================================================================================
// File: RewardsManager.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: April 5, 2026
// Last Modified: April 20, 2026
//
// Description:
// Manages the rewards shop system, including CSV data parsing for inventory, 
// animal purchasing logic, randomized world spawning, and persistent deletion.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewardsManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject ButtonPrefab;
    public Transform itemGrid;

    [Header("Data Path")]
    public string csvFileName = "Data/AnimalsDataList";

    [Header("Spawning Setup")]
    public Transform farmWorldParent;
    public GameObject animalTemplatePrefab;

    [Header("Safe Spawn Zone")]
    public Transform topLeftMarker;
    public Transform bottomRightMarker;

    [Header("Deletion UI")]
    public GameObject animalDeletePopup;
    private AnimalAI pendingAnimalToDelete; // Remembers which animal we clicked

    private List<AnimalData> shopInventory = new List<AnimalData>();
    
    [Header("Limits")]
    public int maxAnimals = 20;

    // --- Unity Callbacks ---

    void Start()
    {
        LoadShopData();
        BuildShopUI();
    }

    // --- Data Loading & UI Generation ---

    void LoadShopData()
    {
        // Load and parse the CSV from Resources containing names, prices, and paths
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null) { Debug.LogError("<color=red>[RewardsManager]</color> CSV not found at Resources/" + csvFileName); return; }

        string[] lines = csvFile.text.Split('\n');

        // Skip the header and parse each row
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] columns = lines[i].Split(',');
            if (columns.Length < 4) continue;

            AnimalData newItem = new AnimalData();
            newItem.name = columns[0].Trim();
            newItem.price = int.Parse(columns[1].Trim());
            newItem.spritePath = columns[2].Trim();
            newItem.prefabPath = columns[3].Trim();
            shopInventory.Add(newItem);
        }
    }

    void BuildShopUI()
    {
        Debug.Log("<color=magenta>[RewardsManager]</color> Building shop UI...");

        // 1. Clear grid
        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        // 2. Load the spritesheet once
        string sheetPath = "Images/CharacterItems/Animals/RewardsAnimalSpritesheet";
        Sprite[] allSprites = Resources.LoadAll<Sprite>(sheetPath);

        if (allSprites.Length <= 1)
        {
            Debug.LogError($"<color=red>[PATH ERROR]</color> Found {allSprites.Length} sprites. Check folder name and Sprite Mode!");
            return;
        }
        shopInventory = shopInventory.OrderBy(animal => animal.price).ToList();
        
        // 3. Build buttons
        foreach (AnimalData data in shopInventory)
        {
            GameObject newBtn = Instantiate(ButtonPrefab, itemGrid);

            // UI mapping for Name and Price (Including your shadow text)
            newBtn.transform.Find("Animal Image/AnimalText").GetComponent<TextMeshProUGUI>().text = data.name.ToUpper();
            newBtn.transform.Find("PriceCoin/PriceText1").GetComponent<TextMeshProUGUI>().text = data.price.ToString();
            newBtn.transform.Find("PriceCoin/PriceText2").GetComponent<TextMeshProUGUI>().text = data.price.ToString();
            
            // 4. Find and set the button Icon
            Image icon = newBtn.transform.Find("Animal Image").GetComponent<Image>();
            foreach (Sprite s in allSprites)
            {
                if (s.name == data.spritePath)
                {
                    icon.sprite = s;
                    icon.preserveAspect = true;
                    break;
                }
            }

            ShopItemManager dragScript = newBtn.GetComponent<ShopItemManager>();
            if (dragScript == null) dragScript = newBtn.AddComponent<ShopItemManager>();

            // Pass sprite data to the drag script for the purchase logic
            dragScript.animalType = data.name;
            dragScript.price = data.price;
            dragScript.prefabToSpawn = data.spritePath;
        }
        Debug.Log("<color=magenta>[RewardsManager]</color> Shop UI built and sorted by price!");
    }

    // --- Shop Interaction Logic ---

    public void TryBuyAnimal(string animalName, int cost, string spriteName)
    {
        // Enforce the animal population limit
        if (GameManager.Instance.activeAnimals.Count >= maxAnimals)
        {
            Debug.LogWarning("FARM FULL: Cannot buy more animals!");
            return; 
        }

        // Validate currency before completing purchase
        if (GameManager.Instance.totalCoins >= cost)
        {
            GameManager.Instance.totalCoins -= cost;
            GameManager.Instance.UpdateCoinUI();

            // Spawn the animal and register it in the session save data
            SpawnAnimalWithData(animalName, spriteName);
            GameManager.Instance.activeAnimals.Add(spriteName);
            GameManager.Instance.SaveCurrentProgress();
        }
        else
        {
            GameManager.Instance.TriggerCoinFlash();
            GameManager.Instance.PlayWrongSound(); 
        }
    }

    // --- Spawning System ---

    public void SpawnAnimalWithData(string animalName, string spriteName)
    {
        if (animalTemplatePrefab == null || farmWorldParent == null)
        {
            return;
        }

        // Create a new instance from the template and rename it
        GameObject newAnimal = Instantiate(animalTemplatePrefab, farmWorldParent);
        newAnimal.name = "Farmer_" + animalName;

        // Locate the specific sprite within the animal sheet
        string sheetPath = "Images/CharacterItems/Animals/RewardsAnimalSpritesheet";
        Sprite[] allSprites = Resources.LoadAll<Sprite>(sheetPath);
        Sprite targetSprite = null;

        foreach (Sprite s in allSprites)
        {
            if (s.name == spriteName)
            {
                targetSprite = s;
                break;
            }
        }

        // Set up the renderer with the correct sorting and sprite
        SpriteRenderer sr = newAnimal.GetComponent<SpriteRenderer>();
        if (sr != null && targetSprite != null)
        {
            sr.sprite = targetSprite;
            sr.sortingLayerName = "Animals"; 
            sr.sortingOrder = 10;            
        }

        // Calculate a randomized position between the boundary markers
        float spawnX = 0;
        float spawnY = 0;

        if (topLeftMarker != null && bottomRightMarker != null)
        {
            spawnX = Random.Range(topLeftMarker.localPosition.x, bottomRightMarker.localPosition.x);
            spawnY = Random.Range(bottomRightMarker.localPosition.y, topLeftMarker.localPosition.y);
        }

        newAnimal.transform.localPosition = new Vector3(spawnX, spawnY, -1f);
    }

    // --- Deletion & Confirmation System ---

    public void RequestAnimalDeletion(AnimalAI animal)
    {
        // Store the target animal and show the confirmation prompt
        pendingAnimalToDelete = animal;
        animalDeletePopup.SetActive(true);
    }
   
    public void ConfirmAnimalDelete()
    {
        // Triggered by the "YES" button in the deletion UI
        if (pendingAnimalToDelete != null)
        {
            // Identify the sprite to remove it from the persistent save list
            string spriteName = pendingAnimalToDelete.GetComponent<SpriteRenderer>().sprite.name;
            
            if (GameManager.Instance.activeAnimals.Contains(spriteName))
            {
                GameManager.Instance.activeAnimals.Remove(spriteName);
                GameManager.Instance.SaveCurrentProgress();
            }

            // Start the visual removal sequence
            StartCoroutine(ShrinkAndDestroy(pendingAnimalToDelete.gameObject));
            pendingAnimalToDelete = null;
        }
        animalDeletePopup.SetActive(false);
    }

    public void CancelAnimalDelete()
    {
        // Triggered by the "NO" button; restores the animal to normal behavior
        if (pendingAnimalToDelete != null)
        {
            pendingAnimalToDelete.ResetAnimal();
        }
        animalDeletePopup.SetActive(false);
    }

    private IEnumerator ShrinkAndDestroy(GameObject target)
    {
        // Coroutine to animate the animal scaling down before destruction
        if (target == null) yield break;

        float duration = 2f; 
        float elapsed = 0f;
        Vector3 startScale = target.transform.localScale;

        while (elapsed < duration)
        {
            if (target == null) yield break; // Safety check if destroyed elsewhere
            elapsed += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
            yield return null;
        }
        Destroy(target);
    }
}
    
[System.Serializable]
public class AnimalData
{
    public string name;
    public int price;
    public string spritePath;
    public string prefabPath;
}