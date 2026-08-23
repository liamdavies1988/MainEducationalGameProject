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

        // The farm scene uses this manager for spawning and deletion but has no shop UI.
        if (itemGrid != null && ButtonPrefab != null)
        {
            BuildShopUI();
        }
        else
        {
            Debug.Log("<color=yellow>[RewardsManager]</color> Shop UI not present in this scene; skipping UI build.");
        }
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

            // Your CSV now has 6 columns, so we check for at least 6
            if (columns.Length < 6) continue;

            AnimalData newItem = new AnimalData();
            newItem.name = columns[0].Trim();
            newItem.price = int.Parse(columns[1].Trim());
            newItem.spritePath = columns[2].Trim();

            // columns[3] is the SheetName ("RewardsAnimalSpritesheet"), which we don't need to store in the object

            newItem.prefabPath = columns[4].Trim(); // Column 4 is the PrefabName
            newItem.soundPath = columns[5].Trim();  // Column 5 is your SoundPath ("sfx_chicken")

            shopInventory.Add(newItem);
        }
    }

    void BuildShopUI()
    {
        Debug.Log("<color=magenta>[RewardsManager]</color> Building shop UI...");

        // 1. Safety Check: Item Grid
        if (itemGrid == null)
        {
            Debug.LogError("FATAL: itemGrid is not assigned in the Inspector!");
            return;
        }

        // 2. Clear grid safely
        foreach (Transform child in itemGrid) { Destroy(child.gameObject); }

        // 3. Load spritesheet
        string sheetPath = "Images/CharacterItems/Animals/RewardsAnimalSpritesheet";
        Sprite[] allSprites = Resources.LoadAll<Sprite>(sheetPath);

        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogError($"FATAL: No sprites found at {sheetPath}. Check folder naming and capital letters!");
            return;
        }

        shopInventory = shopInventory.OrderBy(animal => animal.price).ToList();

        // 4. Build buttons with individual null checks
        foreach (AnimalData data in shopInventory)
        {
            if (ButtonPrefab == null)
            {
                Debug.LogError("ButtonPrefab is missing!");
                break;
            }

            GameObject newBtn = Instantiate(ButtonPrefab, itemGrid);

            try
            {
                // Check Name Text
                Transform nameTrans = newBtn.transform.Find("Animal Image/AnimalText");
                if (nameTrans != null) nameTrans.GetComponent<TextMeshProUGUI>().text = data.name.ToUpper();
                else Debug.LogWarning($"Could not find 'Animal Image/AnimalText' in prefab for {data.name}");

                // Check Price Text 1
                Transform p1 = newBtn.transform.Find("PriceCoin/PriceText1");
                if (p1 != null) p1.GetComponent<TextMeshProUGUI>().text = data.price.ToString();

                // Check Price Text 2
                Transform p2 = newBtn.transform.Find("PriceCoin/PriceText2");
                if (p2 != null) p2.GetComponent<TextMeshProUGUI>().text = data.price.ToString();

                // Find Icon
                Transform iconTrans = newBtn.transform.Find("Animal Image");
                if (iconTrans != null)
                {
                    Image icon = iconTrans.GetComponent<Image>();
                    foreach (Sprite sprite in allSprites)
                    {
                        if (sprite.name == data.spritePath)
                        {
                            icon.sprite = sprite;
                            icon.preserveAspect = true;
                            break;
                        }
                    }
                }

                // Setup Drag Script
                ShopItemManager dragScript = newBtn.GetComponent<ShopItemManager>();
                if (dragScript == null) dragScript = newBtn.AddComponent<ShopItemManager>();

                dragScript.animalType = data.name;
                dragScript.price = data.price;
                dragScript.prefabToSpawn = data.spritePath;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error setting up button for {data.name}: {e.Message}");
            }
        }
        Debug.Log("<color=magenta>[RewardsManager]</color> Shop UI built!");
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

    public void SpawnAnimalWithData(string animalName, string spriteName, string soundName)
    {
        // SAFETY CHECKS
        if (animalTemplatePrefab == null || farmWorldParent == null) return;

        // CREATE THE ANIMAL
        GameObject newAnimal = Instantiate(animalTemplatePrefab, farmWorldParent);
        newAnimal.name = "Farmer_" + animalName;

        // APPLY ARTWORK
        string sheetPath = "Images/CharacterItems/Animals/RewardsAnimalSpritesheet";
        Sprite[] allSprites = Resources.LoadAll<Sprite>(sheetPath);
        foreach (Sprite s in allSprites)
        {
            if (s.name == spriteName)
            {
                SpriteRenderer sr = newAnimal.GetComponent<SpriteRenderer>();
                sr.sprite = s;
                sr.sortingLayerName = "Animals";
                sr.sortingOrder = 10;
                break;
            }
        }

        // THE AUDIO INJECTION
        // Load the sound from Resources/Audio/Animals/ (Ensure folder exists!)
        AudioClip animalClip = null;
        if (!string.IsNullOrEmpty(soundName))
        {
            animalClip = Resources.Load<AudioClip>("Audio/Animals/" + soundName);
        }

        AnimalAI aiScript = newAnimal.GetComponent<AnimalAI>();

        if (aiScript != null && animalClip != null)
        {
            aiScript.animalSound = animalClip; // Corrected: Hand the sound to the AI script
            Debug.Log("<color=green>AUDIO:</color> Injected " + soundName + " into " + animalName);
        }

        // POSITIONING
        float spawnX = Random.Range(topLeftMarker.localPosition.x, bottomRightMarker.localPosition.x);
        float spawnY = Random.Range(bottomRightMarker.localPosition.y, topLeftMarker.localPosition.y);
        newAnimal.transform.localPosition = new Vector3(spawnX, spawnY, -1f);
    }

    // Fallback overload to handle Save File loading and missing parameters
    public void SpawnAnimalWithData(string animalName, string animalSpriteName)
    {
        string soundToInject = "";

        // Look up the missing sound path from our loaded CSV shop inventory
        AnimalData foundData = shopInventory.FirstOrDefault(a => a.spritePath == animalSpriteName);
        if (foundData != null)
        {
            soundToInject = foundData.soundPath;

            // Fix the generic "Animal" name for save file loading
            if (string.IsNullOrEmpty(animalName) || animalName == "Animal" || animalName == "v")
            {
                animalName = foundData.name;
            }
        }

        // Pass everything to your main spawning method
        SpawnAnimalWithData(animalName, animalSpriteName, soundToInject);
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
    public string soundPath;
}