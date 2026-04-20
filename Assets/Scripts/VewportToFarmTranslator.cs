// =================================================================================================
// File: VewportToFarmTranslator.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: April 20, 2026
// Last Modified: April 20, 2026
//
// Description:
// Translates UI pointer events on a RawImage (viewport) into world-space raycasts 
// for a secondary camera, allowing interaction with objects in a separate sub-scene.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class ViewportToFarmTranslator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public Camera farmCamera; 

    // --- Interaction Logic ---

    public void OnPointerDown(PointerEventData eventData)
    {
        // Convert the screen point of the mouse click into a local point within the RawImage rectangle
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(), 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint);

        // Normalize the coordinates to a 0-1 range based on the UI element's dimensions
        Rect rect = GetComponent<RectTransform>().rect;
        float normalizedX = (localPoint.x - rect.x) / rect.width;
        float normalizedY = (localPoint.y - rect.y) / rect.height;

        // Project a ray from the farm camera using the normalized viewport coordinates
        Ray ray = farmCamera.ViewportPointToRay(new Vector3(normalizedX, normalizedY, 0));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        // If an object with a collider is hit, notify the associated AnimalAI script
        if (hit.collider != null)
        {
            AnimalAI animal = hit.collider.GetComponent<AnimalAI>();
            if (animal != null)
            {
                animal.OnMouseDown();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Release the 'held' state for all animals in the scene
        AnimalAI[] allAnimals = Object.FindObjectsByType<AnimalAI>(FindObjectsSortMode.None);
        foreach (var a in allAnimals) 
        {
            a.OnMouseUp();
        }
    }
}