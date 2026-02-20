using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    private bool CorrectAnswer;
    [SerializeField]
    private TextMeshProUGUI AnswerText;

    public void SetAnswer(string answer)
    {
        AnswerText.text = answer;
        
    }
    public void IsCorrect(bool correct)
    {
        CorrectAnswer = correct;
    }
    public void OnClick()
    {
        if (CorrectAnswer)
        {
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Wrong");
        }
    }
}
