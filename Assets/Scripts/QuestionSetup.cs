// =================================================================================================
// File: QuestionSetup.cs
// Author: Liam Davies (lid37)
// Supervisor: Helen Miles (hem23)
// Project: Gamifying the Curriculum: An Educational Application for Primary Education
// Date Created: February 15, 2026
// Last Modified: April 20, 2026
//
// Description:
// Handles the retrieval and configuration of quiz questions, managing the session 
// flow, randomized answer placement, and UI synchronization for the quiz environment.
//
// Third-Party Assets / Code:
// - Logic assistance and structural debugging provided by Google Gemini API.
// - UI Assets sourced from Kenney.nl and Vecteezy (see Appendix B of report).
// - Sound assets sourced from Pixabay.
// =================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionSetup : MonoBehaviour
{
    [Header("Question Data")]
    [SerializeField] private List<QuestionData> questions = new List<QuestionData>();
    private QuestionData currentQuestion;

    [Header("UI Labels")]
    [SerializeField] private TextMeshProUGUI QuestionCountText;
    [SerializeField] private TextMeshProUGUI QuestionText;
    [SerializeField] private TextMeshProUGUI TopicDisplay;
    [SerializeField] private TextMeshProUGUI DifficultyDisplay;

    [Header("Buttons")]
    [SerializeField] private AnswerButton[] answerbuttons;
    
    [Header("Settings")]
    [SerializeField] private int totalQuestionsPerSession = 5;

    private int correctAnswerChoice;
    private int currentQuestionNumber = 1;

    // --- Unity Callbacks ---

    private void Start()
    {
        // Sync the session limit with the player's menu selection
        if (GameManager.Instance != null)
        {
            this.totalQuestionsPerSession = GameManager.Instance.totalQuestionsRequested;
        }

        // Initialize the first question
        GetQuestionAssets();
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    // --- Session Management ---

    private void GetQuestionAssets()
    {
        if (GameManager.Instance == null) return;

        // Determine the Resource path based on user selection in the menu
        string subject = GameManager.Instance.selectedSubject;     
        string difficulty = GameManager.Instance.selectedDifficulty; 
        string path = "Questions/" + subject + "/" + difficulty;

        // Load all available ScriptableObjects from the directory
        QuestionData[] loadedQuestions = Resources.LoadAll<QuestionData>(path);

        if (loadedQuestions.Length > 0)
        {
            questions = new List<QuestionData>(loadedQuestions);
        }
        else
        {
            Debug.LogError($"QuestionSetup: No question assets found in Resources/{path}.");
        }
    }

    private void SelectNewQuestion()
    {
        // Randomly pick a question from the loaded pool
        if (questions.Count > 0)
        {
            int randomQuestionIndex = Random.Range(0, questions.Count);
            currentQuestion = questions[randomQuestionIndex];
            
            // Remove it from the list so it doesn't repeat this session
            questions.RemoveAt(randomQuestionIndex);
        }
        else
        {
            currentQuestion = null;
        }
    }

    public void StartNextQuestion()
    {
        // Check if the session limit has been reached or if questions are exhausted
        if (currentQuestionNumber < totalQuestionsPerSession && questions.Count > 0)
        {
            currentQuestionNumber++;
            SelectNewQuestion();
            SetQuestionValues();
            SetAnswerValues();
        }
        else
        {
            // Session complete, navigate to the rewards environment
            UnityEngine.SceneManagement.SceneManager.LoadScene("RewardsScene");
        }
    }

    // --- UI Configuration ---
    
    // These methods ensure that the UI reflects the current question and its answers.
    private void SetQuestionValues()
    {
        if (currentQuestion == null)
        {
            Debug.LogError("[QuestionSetup] currentQuestion is NULL! Check if questions exist in Resources/Questions.");
            return;
        }

        // 1. Check Question Count Text
        if (QuestionCountText != null)
            QuestionCountText.text = "Question " + currentQuestionNumber;
        else
            Debug.LogError("[QuestionSetup] 'QuestionCountText' is unassigned in the Inspector!");

        // 2. Check Main Question Text
        if (QuestionText != null)
            QuestionText.text = currentQuestion.question;
        else
            Debug.LogError("[QuestionSetup] 'QuestionText' is unassigned in the Inspector!");

        // 3. Check Topic Display
        if (TopicDisplay != null)
            TopicDisplay.text = currentQuestion.topic;
        else
            Debug.LogWarning("[QuestionSetup] 'TopicDisplay' is unassigned in the Inspector!");

        // 4. Check Difficulty Display
        if (DifficultyDisplay != null)
        {
            if (GameManager.Instance != null)
                DifficultyDisplay.text = GameManager.Instance.selectedDifficulty;
        }
        else
        {
            Debug.LogWarning("[QuestionSetup] 'DifficultyDisplay' is unassigned in the Inspector!");
        }
    }

    // --- Answer Configuration ---
    // This method randomizes the answer order and assigns them to the UI buttons.
    private void SetAnswerValues()
    {
        if (currentQuestion == null || currentQuestion.answers == null) return;

        List<string> randomizedAnswers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerbuttons.Length; i++)
        {
            // Make sure the button slot in the array isn't null
            if (answerbuttons[i] == null)
            {
                Debug.LogError($"[QuestionSetup] AnswerButton at index {i} is MISSING (None) in the Inspector!");
                continue;
            }

            if (i < randomizedAnswers.Count)
            {
                answerbuttons[i].gameObject.SetActive(true);
                bool isThisCorrect = (i == correctAnswerChoice);
                answerbuttons[i].SetIsCorrect(isThisCorrect);
                answerbuttons[i].SetAnswerText(randomizedAnswers[i]);
            }
            else
            {
                answerbuttons[i].gameObject.SetActive(false);
            }
        }
    }

    // --- Utility Methods ---

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        // This logic ensures that the "Correct" answer (always at index 0 in the ScriptableObject)
        // is tracked correctly after the list is shuffled for the UI.

        bool correctAnswerFound = false;
        List<string> newList = new List<string>();
        int originalCount = originalList.Count;

        for (int i = 0; i < originalCount; i++)
        {
            int random = Random.Range(0, originalList.Count);

            // If the randomly picked item was index 0, mark the current UI button index as correct
            if (random == 0 && !correctAnswerFound)
            {
                correctAnswerChoice = i;
                correctAnswerFound = true;
            }
            newList.Add(originalList[random]);
            originalList.RemoveAt(random);
        }
        return newList;
    }
}
