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

    [Header("Farm World Placement")]
    public Transform farmWorldParent; 

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
        // 1. Load the spritesheet. 
        // Ensure folder is 'Animals' NOT 'Aninmals'
        string sheetPath = "Images/CharacterItems/Animals/RewardsAnimalSpritesheet";
        Sprite[] allSprites = Resources.LoadAll<Sprite>(sheetPath);

        if (allSprites.Length <= 1) 
        {
            Debug.LogError($"<color=red>[PATH ERROR]</color> Found {allSprites.Length} sprites. Check folder name 'Animals' and set Sprite Mode to 'Multiple'!");
            return;
        }

        // 2. Clear grid
        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        // 3. Build buttons
        foreach (AnimalData data in shopInventory)
        {
            GameObject newBtn = Instantiate(ButtonPrefab, itemGrid);
            
            // Check paths inside your prefab
            newBtn.transform.Find("Animal Image/AnimalText").GetComponent<TextMeshProUGUI>().text = data.name.ToUpper();
            newBtn.transform.Find("PriceCoin/PriceText1").GetComponent<TextMeshProUGUI>().text = data.price.ToString();
            newBtn.transform.Find("PriceCoin/PriceText2").GetComponent<TextMeshProUGUI>().text = data.price.ToString();

            Image icon = newBtn.transform.Find("Animal Image").GetComponent<Image>();
            
            // Match the slice name to the CSV string
            foreach (Sprite s in allSprites)
            {
                if (s.name == data.spritePath)
                {
                    icon.sprite = s;
                    icon.preserveAspect = true;
                    break;
                }
            }

            ShopItem dragScript = newBtn.GetComponent<ShopItem>();
            if (dragScript == null) dragScript = newBtn.AddComponent<ShopItem>();
            
            dragScript.animalType = data.name;
            dragScript.price = data.price;
            dragScript.prefabToSpawn = data.prefabPath;
        }
    }

    public void TryBuyAnimal(string animalName, int cost, string prefabName)
    {
        if (GameManager.Instance.totalCoins >= cost)
        {
            GameManager.Instance.totalCoins -= cost;
            GameManager.Instance.UpdateCoinUI();
            SpawnAnimalInWorld(prefabName);
            
            // This ensures the purchase is remembered
            GameManager.Instance.SaveCurrentProgress();
            Debug.Log("<color=green>[RewardsManager]</color> Success: Bought " + animalName);
        }
        else
        {
            Debug.Log("<color=red>[RewardsManager]</color> Not enough coins!");
        }
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