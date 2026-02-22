using UnityEngine;
using TMPro;
using System.Collections;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrectAnswer; // Renamed for clarity
    [SerializeField] private TextMeshProUGUI AnswerText;

    // This needs to match the name called in QuestionSetup
    public void SetAnswerText(string answer)
    {
        AnswerText.text = answer;
    }

    // This needs to match the name called in QuestionSetup
    public void SetIsCorrect(bool correct)
    {
        isCorrectAnswer = correct;
    }

    public void OnClick()
    {
        if (isCorrectAnswer)
        {
            Debug.Log("Correct!");
            

        // 1. Give the reward
        GameManager.Instance.AddCoin(); 
        
        // 2. TODO: Trigger the 'Happy Face' animation/sprite change here
        
        // 3. Load the next question (you'll likely want a small delay here)
        // Invoke("LoadNextQuestion", 1.0f);
        
        StartCoroutine(NextQuestionDelay());
        }
        else
        {
            Debug.Log("Wrong answer, try again.");
            // TODO: Trigger the 'Sad Face' animation/sprite change here
        }
        
    }

    IEnumerator NextQuestionDelay()
    {
        // Wait for 1 second so the child can see their reward
        yield return new WaitForSeconds(1.0f);

        // 2. Use the new "FindFirstObjectByType" instead of the old "FindObjectOfType"
        QuestionSetup qs = Object.FindFirstObjectByType<QuestionSetup>();

        if (qs != null)
        {
            qs.StartNextQuestion();
        }
    }
}