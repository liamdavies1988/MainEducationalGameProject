using UnityEngine;
using TMPro;
using System.Collections;

public class AnswerButton : MonoBehaviour
{
    private bool isCorrectAnswer;
    private Animator anim;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI AnswerText;

    [Header("Character Reaction")]
    [SerializeField] private GameObject sadFaceEmoji; // Drag your character's sad face object here

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

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
        if (anim == null) anim = GetComponent<Animator>(); // Double check we have the animator

        if (isCorrectAnswer)
        {
            Debug.Log("Correct!");
            anim.SetTrigger("Pressed");
            GameManager.Instance.AddCoin();
            StartCoroutine(NextQuestionDelay());
        }
        else
        {
            Debug.Log("Wrong!");
            // Make sure "WrongAnswer" matches the name in your Parameters tab EXACTLY
            anim.SetTrigger("WrongAnswer");

            
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

   /* IEnumerator HideSadFace()
    {
        // Keep the sad face visible for 1 second
        yield return new WaitForSeconds(1.0f);

        if (sadFaceEmoji != null)
        {
            sadFaceEmoji.SetActive(false);
        }
    }*/
}