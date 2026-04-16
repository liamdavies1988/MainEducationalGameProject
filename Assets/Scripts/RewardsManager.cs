using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    // This uses the class defined at the bottom
    private List<AnimalData> shopInventory = new List<AnimalData>();

    void Start()
    {
        Debug.Log("<color=cyan>[RewardsManager]</color> Initializing Rewards Manager...");
        LoadShopData();
        BuildShopUI();
        Debug.Log("<color=cyan>[RewardsManager]</color> Initialization complete!");
    }

    void LoadShopData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null) { Debug.LogError("<color=red>[RewardsManager]</color> CSV not found at Resources/" + csvFileName); return; }

        string[] lines = csvFile.text.Split('\n');

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

            // 5. THE FIX: Link the Drag Logic to the Sprite Name
            ShopItem dragScript = newBtn.GetComponent<ShopItem>();
            if (dragScript == null) dragScript = newBtn.AddComponent<ShopItem>();
            
            dragScript.animalType = data.name;
            dragScript.price = data.price;

            // --- CRITICAL CHANGE HERE ---
            // We tell the drag script to pass the SPRITE name (e.g. animal_bear)
            // to the spawner, NOT the sheet name.
            dragScript.prefabToSpawn = data.spritePath; 
            // ----------------------------
        }
        Debug.Log("<color=magenta>[RewardsManager]</color> Shop UI built successfully!");
    }

     public void TryBuyAnimal(string animalName, int cost, string spriteName)
    {
        if (GameManager.Instance.totalCoins >= cost)
        {
            GameManager.Instance.totalCoins -= cost;
            GameManager.Instance.UpdateCoinUI();
            
            SpawnAnimalWithData(animalName, spriteName);
            
            // Register the purchase in the GameManager's list for JSON saving
            GameManager.Instance.activeAnimals.Add(spriteName);
            GameManager.Instance.SaveCurrentProgress();
            
            Debug.Log("<color=green>SUCCESS:</color> Bought " + animalName);
        }
        else
        {
            Debug.Log("<color=red>FAILED:</color> Not enough coins!");
        }
    }
public void SpawnAnimalWithData(string animalName, string spriteName)
{
    // --- STEP 1: SAFETY CHECKS (Must come FIRST) ---
    if (animalTemplatePrefab == null)
    {
        Debug.LogError("<color=red>FATAL ERROR:</color> Animal Template Prefab is missing in the Inspector!");
        return; 
    }

    if (farmWorldParent == null)
    {
        Debug.LogError("<color=red>FATAL ERROR:</color> Farm World Parent is not assigned! I don't know where to put the animal.");
        return;
    }

    // --- STEP 2: CREATE THE ANIMAL ---
    GameObject newAnimal = Instantiate(animalTemplatePrefab, farmWorldParent);
    newAnimal.name = "Farmer_" + animalName;

    // --- STEP 3: LOAD AND APPLY ARTWORK ---
    Sprite[] allSprites = Resources.LoadAll<Sprite>("Images/CharacterItems/Animals/RewardsAnimalSpritesheet");
    Sprite targetSprite = null;

    foreach (Sprite s in allSprites)
    {
        if (s.name == spriteName) 
        { 
            targetSprite = s; 
            break; 
        }
    }

    if (targetSprite == null)
    {
        Debug.LogWarning("LOADER: Could not find sprite '" + spriteName + "' in the spritesheet.");
    }

    // --- STEP 4: CONFIGURE VISUALS ---
    SpriteRenderer sr = newAnimal.GetComponent<SpriteRenderer>();
    if (sr != null && targetSprite != null)
    {
        sr.sprite = targetSprite;
        sr.sortingLayerName = "Animals"; // Make sure this layer exists in Unity!
        sr.sortingOrder = 5;
    }

    // --- STEP 5: POSITIONING ---
    // Using localPosition because it is a child of the FarmWorldParent
    float randomX = Random.Range(-4f, 4f);
    float randomY = Random.Range(-2f, 2f);
    newAnimal.transform.localPosition = new Vector3(randomX, randomY, -1f); 
    
    Debug.Log("<color=green>SUCCESS:</color> Spawned " + animalName + " at " + newAnimal.transform.localPosition);
}

    void SpawnAnimalInWorld(string prefabName)
    {
        // Path should match your folder: Assets/Resources/Prefabs/Animals/
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Animals/" + prefabName);
        
        if (prefab != null)
        {
            GameObject newAnimal = Instantiate(prefab, farmWorldParent);
            newAnimal.transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-2f, 2f), 0);
        }
        else
        {
            Debug.LogWarning("PREFAB MISSING: " + prefabName + " not found in Resources/Prefabs/Animals/");
        }
    }
}

// --- THE FIX: CHANGED 'internal' TO 'public' ---
[System.Serializable]
public class AnimalData
{
    public string name;
    public int price;
    public string spritePath;
    public string prefabPath;
}

// --- RECENTLY EDITED FILES ---