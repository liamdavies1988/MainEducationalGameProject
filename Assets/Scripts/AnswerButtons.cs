using UnityEngine;
using TMPro;
using System.Collections;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrectAnswer;
    

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI AnswerText;

    [Header("Button Feedback Icons")]
    [SerializeField] private GameObject tickIcon;
    [SerializeField] private GameObject crossIcon;

    

    // Called by QuestionSetup to update the text on the button
    public void SetAnswerText(string answer)
    {
        if (AnswerText != null)
        {
            AnswerText.text = answer;
        }
    }

    // Called by QuestionSetup to tell the button if it is the right answer
    public void SetIsCorrect(bool correct)
    {
        isCorrectAnswer = correct;
    }
   

    public void OnClick()
    {
        if (isCorrectAnswer)
        {
            Debug.Log("Correct!");

            // 1. Visual Feedback: Show Tick, hide Text
            if (tickIcon != null) tickIcon.SetActive(true);

            // 3. Manager Handshakes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin();
                GameManager.Instance.ShowReaction(true);
                Debug.Log("AnswerButton: Sent 'Correct' signal to GameManager.");
            }
            else
            {
                Debug.LogError("AnswerButton: GameManager.Instance is NULL!");
            }

            // 4. Cleanup and Progression
            StartCoroutine(ResetButtonUI());
            StartCoroutine(NextQuestionDelay());     // MOVE TO NEXT QUESTION
        }
        else
        {
            Debug.Log("Wrong!");

            // 1. Visual Feedback: Show Cross, hide Text
            if (crossIcon != null) crossIcon.SetActive(true);
            if (AnswerText != null) AnswerText.gameObject.SetActive(false);    
            // 2. Animation & Sound
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayWrongSound();
                GameManager.Instance.ShowReaction(false);
                Debug.Log("AnswerButton: Sent 'Wrong' signal to GameManager.");
            }
            else
            {
                Debug.LogError("AnswerButton: GameManager.Instance is NULL!");
            }

            // 3. THE FIX: Reset the UI but DO NOT call NextQuestionDelay
            // This brings the text back and hides the 'X' so they can try again!
            GameManager.Instance.SaveCurrentProgress(); 
            StartCoroutine(ResetButtonUI());
            StartCoroutine(NextQuestionDelay());     // MOVE TO NEXT QUESTION
        }
    }

    IEnumerator NextQuestionDelay()
    {
        // Wait so the child can see the reward/animation
        yield return new WaitForSeconds(1.0f);

        // Find the QuestionSetup script and load the next question
        QuestionSetup qs = Object.FindFirstObjectByType<QuestionSetup>();
        if (qs != null)
        {
            qs.StartNextQuestion();
        }
    }

    IEnumerator ResetButtonUI()
    {
        // Wait for the same time as your animation
        yield return new WaitForSeconds(1.0f);

        // Turn everything back to normal for the next question
        if (tickIcon != null) tickIcon.SetActive(false);
        if (crossIcon != null) crossIcon.SetActive(false);
        if (AnswerText != null) AnswerText.gameObject.SetActive(true);
    }
}
