// =================================================================================================
// File: QuestionData.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: April 20, 2026
//
// Description:
// ScriptableObject data container for individual educational questions, storing 
// the prompt text, associated topic, and a collection of possible answers.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData", order = 1)]
public class QuestionData : ScriptableObject
{
    [Header("Question Details")]
    public string question; 
    public string topic;    
    
    [Header("Answer Settings")]
    // Note: The correct answer must be placed at index [0] for the randomization logic
    public string[] answers; 
}