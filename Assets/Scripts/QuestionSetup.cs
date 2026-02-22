using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField] private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField] private TextMeshProUGUI QuestionText;
    [SerializeField] private TextMeshProUGUI TopicDisplay; // Avoid naming conflicts with variables
    [SerializeField] private AnswerButton[] answerbuttons;
    [SerializeField] private string selectedTopic = "Maths"; // Set this in the Inspector

    private int correctAnswerChoice;

    private void Awake()
    {
        GetQuestionAssets();
    }

    void Start()
    {
        SelectNewQuestion();
        SetQuestionValues();
        SetAnswerValues();
    }

    private void GetQuestionAssets()
    {
        // Now it only loads questions from the specific subfolder!
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
        if (questions.Count > 0)
        {
            int randomQuestionIndex = Random.Range(0, questions.Count);
            currentQuestion = questions[randomQuestionIndex];
            questions.RemoveAt(randomQuestionIndex);
        }
        else 
        {
        // This is where you would load the "Session Summary" screen!
        Debug.Log("Out of questions! Returning to Farm...");
        }
    }

    private void SetQuestionValues()
    {
        QuestionText.text = currentQuestion.question;
        TopicDisplay.text = currentQuestion.topic;
    }
    
    private void SetAnswerValues()
    {
        // Use the correct array name from QuestionData
        List<string> randomizedAnswers = RandomizeAnswers(new List<string>(currentQuestion.answers));

        for(int i = 0; i < answerbuttons.Length; i++)
        {
            bool isThisCorrect = false;
        
            if(i == correctAnswerChoice)
            {
                isThisCorrect = true; // Fixed: removed "bool" so it updates the variable above
            }

            answerbuttons[i].SetIsCorrect(isThisCorrect);
            answerbuttons[i].SetAnswerText(randomizedAnswers[i]);
        }
    }

    private List<string> RandomizeAnswers(List<string> originalList)
    {
        bool correctAnswerFound = false;
        List<string> newList = new List<string>();
        int originalCount = originalList.Count;

        for(int i = 0; i < originalCount; i++)
        {
            int random = Random.Range(0, originalList.Count);
            
            // If we are picking the first item from the original list, 
            // that is our correct answer (based on your ScriptableObject logic)
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
    public void StartNextQuestion()
        {
        // 1. Select a new random question from the list
        SelectNewQuestion();

        // 2. Update the Chalkboard text
        SetQuestionValues();

        // 3. Update the 4 buttons with new answers
        SetAnswerValues();
        }
}