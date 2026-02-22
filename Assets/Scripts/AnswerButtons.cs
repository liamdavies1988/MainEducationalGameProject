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
    // This "Coroutine" waits before refreshing the screen
    IEnumerator NextQuestionDelay()
    {
    yield return new WaitForSeconds(1.0f); // Wait for 1 second
    
    // Find the QuestionSetup script and tell it to refresh
    FindObjectOfType<QuestionSetup>().StartNextQuestion();
    }
}