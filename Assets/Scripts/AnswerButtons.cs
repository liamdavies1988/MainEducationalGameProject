// =================================================================================================
// File: AnswerButtons.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: April 20, 2026
//
// Description:
// Manages individual answer button logic, providing visual feedback (ticks/crosses), 
// handling currency rewards via the GameManager, and triggering session progression.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using UnityEngine;
using TMPro;
using System.Collections;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrectAnswer;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI AnswerText; 

    [Header("Button Feedback Icons")]
    [SerializeField] private GameObject tickIcon;
    [SerializeField] private GameObject crossIcon;

    

    // --- Initialization Methods ---

    // Updates the text displayed on the button
    public void SetAnswerText(string answer)
    {
        // Check if the TextMeshPro component is assigned before updating
        if (AnswerText != null)
        {
            AnswerText.text = answer;
        }
    }

    // Configures whether this button represents the correct answer
    public void SetIsCorrect(bool correct)
    {
        // Store the state passed from QuestionSetup
        isCorrectAnswer = correct;
    }
   
    // --- Interaction Logic ---

    // Triggered by the Button component's onClick event
    public void OnClick()
    {
        if (isCorrectAnswer)
        {
            // Show the tick icon for visual confirmation
            if (tickIcon != null) tickIcon.SetActive(true);

            if (GameManager.Instance != null)
            {
                // Notify GameManager to award currency and show positive feedback
                GameManager.Instance.AddCoin();
                GameManager.Instance.ShowReaction(true);
            }
            else
            {
                Debug.LogError("AnswerButton: GameManager.Instance is NULL!");
            }

            // Clean up UI and move to the next question
            StartCoroutine(ResetButtonUI());
            StartCoroutine(NextQuestionDelay());
        }
        else
        {
            // Show the cross icon and hide the text to indicate a wrong choice
            if (crossIcon != null) crossIcon.SetActive(true);
            if (AnswerText != null) AnswerText.gameObject.SetActive(false);    

            if (GameManager.Instance != null)
            {
                // Trigger negative feedback audio and visuals
                GameManager.Instance.PlayWrongSound();
                GameManager.Instance.ShowReaction(false);
            }
            else
            {
                Debug.LogError("AnswerButton: GameManager.Instance is NULL!");
            }

            // Save progress and proceed
            GameManager.Instance.SaveCurrentProgress(); 
            StartCoroutine(ResetButtonUI());
            StartCoroutine(NextQuestionDelay());
        }
    }

    // --- Feedback & Progression Coroutines ---

    // Delays the transition to the next question to allow feedback to be seen
    IEnumerator NextQuestionDelay()
    {
        // Pause briefly so the player can process the feedback icons
        yield return new WaitForSeconds(1.0f);

        // Find the setup controller in the scene and trigger the next question
        QuestionSetup qs = Object.FindFirstObjectByType<QuestionSetup>();
        if (qs != null)
        {
            qs.StartNextQuestion();
        }
    }

    // Restores the button's visual state for the next question
    IEnumerator ResetButtonUI()
    {
        // Match the delay of the question transition
        yield return new WaitForSeconds(1.0f);

        // Hide feedback icons and restore text visibility for the next round
        if (tickIcon != null) tickIcon.SetActive(false);
        if (crossIcon != null) crossIcon.SetActive(false);
        if (AnswerText != null) AnswerText.gameObject.SetActive(true);
    }
}
