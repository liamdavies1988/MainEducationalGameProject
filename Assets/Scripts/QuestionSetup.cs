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

    private void Start()
    { 
        // 1. Load the questions from Resources
        GetQuestionAssets();

        // 2. SAFETY CHECK: Only proceed if we actually found questions
        if (questions != null && questions.Count > 0)
        {
            // 3. THIS IS THE MISSING STEP: Pick the first question!
            SelectNewQuestion();
            
            // 4. Update the UI
            SetQuestionValues();
            SetAnswerValues();
        }
        else
        {
            Debug.LogError("<color=red>GAME HALTED:</color> No questions loaded. Character is standing in an empty classroom!");
        }
    }

    private void GetQuestionAssets()
    {
        if (GameManager.Instance == null) return;

        // Get choices from the Brain (GameManager)
        string subject = GameManager.Instance.selectedSubject;     
        string difficulty = GameManager.Instance.selectedDifficulty; 

        // Build the path: e.g. Questions/Mathematics/Easy
        string path = "Questions/" + subject + "/" + difficulty;

        Debug.Log("<color=cyan>LOADING QUESTIONS FROM:</color> Resources/" + path);

        QuestionData[] loadedQuestions = Resources.LoadAll<QuestionData>(path);

        if (loadedQuestions.Length > 0)
        {
            questions = new List<QuestionData>(loadedQuestions);
            Debug.Log($"<color=green>SUCCESS:</color> Loaded {questions.Count} {subject} questions.");
        }
        else
        {
            Debug.LogError($"FATAL ERROR: No .asset files found in Resources/{path}.");
        }
    }

    private void SelectNewQuestion()
    {
        if (questions.Count > 0)
        {
            int randomQuestionIndex = Random.Range(0, questions.Count);
            currentQuestion = questions[randomQuestionIndex];
            
            // Remove it from the list so it doesn't repeat this session
            questions.RemoveAt(randomQuestionIndex);
            
            Debug.Log("Selected Question: " + currentQuestion.question);
        }
        else
        {
            currentQuestion = null;
        }
    }

    private void SetQuestionValues()
    {
        if (currentQuestion == null) return;

        QuestionCountText.text = "Question " + currentQuestionNumber; 
        QuestionText.text = currentQuestion.question;
        TopicDisplay.text = currentQuestion.topic;
        
        if (GameManager.Instance != null)
            DifficultyDisplay.text = GameManager.Instance.selectedDifficulty;
    }

    private void SetAnswerValues()
    {
        if (currentQuestion == null) return;

        List<string> randomizedAnswers = RandomizeAnswers(new List<string>(currentQuestion.answers));

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

    public void StartNextQuestion()
    {
        if (currentQuestionNumber < totalQuestionsPerSession && questions.Count > 0)
        {
            currentQuestionNumber++;
            SelectNewQuestion();
            SetQuestionValues();
            SetAnswerValues();
        }
        else
        {
            Debug.Log("Session Finished!");
            // Add your transition to Rewards or Main Menu here
            UnityEngine.SceneManagement.SceneManager.LoadScene("RewardsScene");
        }
    }

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        bool correctAnswerFound = false;
        List<string> newList = new List<string>();
        int originalCount = originalList.Count;

        for (int i = 0; i < originalCount; i++)
        {
            int random = Random.Range(0, originalList.Count);
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

// --- RECENTLY EDITED FILES ---