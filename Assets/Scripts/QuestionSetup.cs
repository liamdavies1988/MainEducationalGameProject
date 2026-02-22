using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI QuestionCountText;
    [SerializeField] private TextMeshProUGUI QuestionText;
    [SerializeField] private TextMeshProUGUI TopicDisplay;
    [SerializeField] private AnswerButton[] answerbuttons;
    [SerializeField] private string selectedTopic = "Maths";
    [SerializeField] private int totalQuestionsPerSession = 3;

    private int correctAnswerChoice;
    private int currentQuestionNumber = 1; // Start at 1 for the UI

    private void Awake()
    {
        GetQuestionAssets();
    }

    void Start()
    {
        // On the very first start, we don't need to increment, 
        // we just set the values for Question 1.
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    private void GetQuestionAssets()
    {
        string path = "Questions/" + selectedTopic;
        QuestionData[] loadedQuestions = Resources.LoadAll<QuestionData>(path);

        if (loadedQuestions.Length == 0)
        {
            Debug.LogError($"No questions found in {path}");
        }
        else
        {
            questions = new List<QuestionData>(loadedQuestions);
        }
    }

    private void SelectNewQuestion()
    {
        // Safety check to make sure we actually have questions in the folder
        if (questions.Count > 0)
        {
            int randomQuestionIndex = Random.Range(0, questions.Count);
            currentQuestion = questions[randomQuestionIndex];
            questions.RemoveAt(randomQuestionIndex);
        }
    }

    private void SetQuestionValues()
    {
        QuestionCountText.text = "Question " + currentQuestionNumber;
        QuestionText.text = currentQuestion.question;
        TopicDisplay.text = currentQuestion.topic;
    }
    public void StartNextQuestion()
    {
        // CHECK: Have we reached the limit?
        if (currentQuestionNumber < totalQuestionsPerSession)
        {
            currentQuestionNumber++;

            SelectNewQuestion();
            SetQuestionValues();
            SetAnswerValues();
        }
        else
        {
            // END OF SESSION
            Debug.Log("Session Finished! No more questions.");

            // This stops the game in the editor for testing
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    private void SetAnswerValues()
    {
        List<string> randomizedAnswers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for (int i = 0; i < answerbuttons.Length; i++)
        {
            bool isThisCorrect = (i == correctAnswerChoice);
            answerbuttons[i].SetIsCorrect(isThisCorrect);
            answerbuttons[i].SetAnswerText(randomizedAnswers[i]);
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