// =================================================================================================
// File: SceneChanger.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: March 10, 2026
// Last Modified: April 20, 2026
//
// Description:
// A utility script used to handle scene transitions across the application, 
// allowing for simple scene loading via Unity's SceneManager.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // --- Navigation Methods ---

    // Loads a new scene by its string name
    public void ChangeScene(string sceneName)
    {
        // Log the transition for debugging purposes
        Debug.Log($"SceneChanger: Changing scene to {sceneName}");
        
        // Execute the scene load
        SceneManager.LoadScene(sceneName);
    }
}