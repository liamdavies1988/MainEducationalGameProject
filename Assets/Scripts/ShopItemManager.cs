using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string animalType;
    [HideInInspector] public int price;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private Transform originalParent;
    public string prefabToSpawn;

    private void Awake() {
        Debug.Log("<color=cyan>[ShopItem]</color> Initializing " + gameObject.name);
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log("<color=cyan>[ShopItem]</color> CanvasGroup added to " + gameObject.name);
        }
        Debug.Log("<color=cyan>[ShopItem]</color> Setup complete for " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("<color=yellow>[ShopItem]</color> Begin drag: " + animalType + " (Price: " + price + ")");
        startPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        Debug.Log("<color=yellow>[ShopItem]</color> Start position saved: " + startPosition);
        
        // Move it to the root Canvas so it's not cut off by the ScrollView mask
        transform.SetParent(transform.root);
        Debug.Log("<color=yellow>[ShopItem]</color> Moved to root canvas");
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // Mouse "sees through" this to the FarmBox
        Debug.Log("<color=yellow>[ShopItem]</color> Alpha set to 0.6, raycasts disabled");
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.position = eventData.position;
        Debug.Log("<color=magenta>[ShopItem]</color> Dragging " + animalType + " to position: " + eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("<color=blue>[ShopItem]</color> End drag: " + animalType);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject droppedOn = eventData.pointerEnter;
        Debug.Log("<color=blue>[ShopItem]</color> Dropped on: " + (droppedOn != null ? droppedOn.name : "nothing"));
        
        // Ensure your wooden frame is named "FarmBox" or "RawImage"
        if (droppedOn != null && (droppedOn.name == "FarmBox" || droppedOn.name == "RawImage")) {
            Debug.Log("<color=green>[ShopItem]</color> Valid drop target detected: " + droppedOn.name);
            
            // Call the manager to process the buy
            RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();
            if (rm != null)
            {
                Debug.Log("<color=green>[ShopItem]</color> RewardsManager found. Processing purchase...");
                rm.TryBuyAnimal(animalType, price, prefabToSpawn);
            }
            else
            {
                Debug.LogError("<color=red>[ShopItem]</color> RewardsManager not found in scene!");
            }
        }
        else
        {
            Debug.Log("<color=orange>[ShopItem]</color> Invalid drop target or no target found. Returning item to original position.");
        }

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = startPosition;
        Debug.Log("<color=blue>[ShopItem]</color> Item returned to original parent and position");
    }
}

// --- RECENTLY EDITED FILES ---