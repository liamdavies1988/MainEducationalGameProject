using UnityEngine;
using UnityEngine.UI;

public class HairPopulator : MonoBehaviour
{
    [Header("Setup")]
    public GameObject HairStyleButton; // Drag your BLUE Prefab here
    public Transform gridParent;        // Drag the white box (HairGrid) here
    public Image characterHairLayer;    // Drag the hair layer ON the character here

    void Start()
    {
        // 1. Load every hair sprite from Assets/Resources/HairStyles
        Sprite[] hairSprites = Resources.LoadAll<Sprite>("HairStyles");

        foreach (Sprite s in hairSprites)
        {
            // 2. Spawn a wooden button
            GameObject newBtn = Instantiate(HairStyleButton, gridParent);

            // 3. Find the "HairIcon" we made inside the button and set the hair
            // We use 'Find' or 'GetComponentInChildren'
            Image previewImage = newBtn.transform.Find("HairIcon").GetComponent<Image>();
            previewImage.sprite = s;

            // 4. Make it work! When clicked, the character's hair changes
            newBtn.GetComponent<Button>().onClick.AddListener(() => {
                characterHairLayer.sprite = s;
                Debug.Log("Equipped: " + s.name);
            });
        }
    }
}