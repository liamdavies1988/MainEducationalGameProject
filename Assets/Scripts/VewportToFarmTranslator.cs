using UnityEngine;
using UnityEngine.EventSystems;

public class ViewportToFarmTranslator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public Camera farmCamera; // Drag your FarmCamera here

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. Convert the mouse click into a position inside the RawImage
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(), 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint);

        // 2. Normalize the click (get a 0 to 1 value of where you clicked in the box)
        Rect rect = GetComponent<RectTransform>().rect;
        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        // 3. Tell the Farm Camera to fire a ray from that relative spot
        Ray ray = farmCamera.ViewportPointToRay(new Vector3(normalizedX, normalizedY, 0));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            // 4. We found an animal! Tell it it's being clicked.
            AnimalAI animal = hit.collider.GetComponent<AnimalAI>();
            if (animal != null)
            {
                // We call a new public function we will add to AnimalAI
                animal.OnMouseDown();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Tell all animals to stop being held
        AnimalAI[] allAnimals = FindObjectsByType<AnimalAI>(FindObjectsSortMode.None);
        foreach (var a in allAnimals) a.OnMouseUp();
    }
}