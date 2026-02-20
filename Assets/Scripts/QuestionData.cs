using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData", order = 1)]
public class QuestionData : ScriptableObject
{
    public string question; // Match naming in Setup
    public string topic;    // Match naming in Setup
    
    // Correct answer should be at index [0] in this array
    public string[] answers; 
}