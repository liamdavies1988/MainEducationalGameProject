using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Callbacks;

public class QuestionSetup : MonoBehaviour
{
    [SerializeField]
    private List<QuestionData> questions;
    private QuestionData currentQuestion;

    [SerializeField]
    private TextMeshProUGUI QuestionText;
    [SerializeField]
    private TextMeshProUGUI Topic;
    [SerializeField]
    private AnswerButton[] answerbuttons;

    [SerializeField]
    private int correctAnswerChoice;

    private void Awake()
    {
        GetQuestionAssets();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectNewQuestion();

        SetQuestionValue();

        SetAnswerValue();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GetQuestionAssets()
    {
       questions = new List<QuestionData>(Resources.LoadAll<QuestionData>("Questions"));
    }

    private void SelectNewQuestion()
    {
        int randomQuestionIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        questions.RemoveAt(randomQuestionIndex);
    }
}
