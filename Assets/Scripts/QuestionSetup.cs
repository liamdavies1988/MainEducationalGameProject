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

    private void SetQuestionValues()
    {
        if (currentQuestion == null) return;

        // Update the textual labels for the current question

        QuestionCountText.text = "Question " + currentQuestionNumber; 
        QuestionText.text = currentQuestion.question;
        TopicDisplay.text = currentQuestion.topic;
        
        if (GameManager.Instance != null)
            DifficultyDisplay.text = GameManager.Instance.selectedDifficulty;
    }

    private void SetAnswerValues()
    {
        if (currentQuestion == null) return;

        // Shuffle the answers so the correct one isn't always in the same spot
        List<string> randomizedAnswers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        // Configure each button with its randomized answer and correctness state
        for (int i = 0; i < answerbuttons.Length; i++)
        {
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
