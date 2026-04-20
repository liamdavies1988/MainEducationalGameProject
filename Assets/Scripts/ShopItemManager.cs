// =================================================================================================
// File: ShopItemManager.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: April 6, 2026
// Last Modified: April 20, 2026
//
// Description:
// Handles the drag-and-drop behavior for shop items, facilitating the purchase 
// flow by detecting valid drop targets within the farm environment.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItemManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string animalType;
    [HideInInspector] public int price;
    public string prefabToSpawn;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private Transform originalParent;
    private int originalSiblingIndex;

    // --- Unity Callbacks ---

    private void Awake()
    {
        // Cache component references and ensure CanvasGroup exists for drag logic
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // --- Interaction Logic ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store current position and parent to allow for item return if purchase fails
        originalParent = transform.parent;
        startPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();

        // Move item to root canvas to avoid being masked by the ScrollView during movement
        transform.SetParent(transform.root, true);

        // Provide visual feedback and allow mouse detection to pass through to the environment
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update the UI position to follow the pointer
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore visual state and physics detection
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        GameObject droppedOn = eventData.pointerEnter;

        // Check if the item was dropped onto the valid farm area
        if (droppedOn != null && (droppedOn.name == "FarmBox" || droppedOn.name == "RawImage"))
        {
            RewardsManager rm = Object.FindFirstObjectByType<RewardsManager>();
            if (rm != null)
            {
                // Initiate the purchase flow via the RewardsManager
                rm.TryBuyAnimal(animalType, price, prefabToSpawn);
            }
        }

        // Return the UI item to its original slot in the shop grid
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = startPosition;
    }
}