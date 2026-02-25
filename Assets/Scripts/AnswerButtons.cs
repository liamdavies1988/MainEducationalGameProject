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
        if (isCorrectAnswer)
        {
            Debug.Log("Correct!");

            // 1. Trigger the 'Right' animation (Shrink/Pop)
            if (anim != null) anim.SetTrigger("Pressed");

            // 2. Add the coin to the total
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin();
            }

            // 3. Move to the next question after a 1 second delay
            StartCoroutine(NextQuestionDelay());
        }
        else
        {
            Debug.Log("Wrong!");

            // 1. Trigger the 'Wrong' animation (Wobble)
            if (anim != null) anim.SetTrigger("WrongAnswer");

            // 2. Show the Sad Face on the character
            if (sadFaceEmoji != null)
            {
                sadFaceEmoji.SetActive(true);
                //StartCoroutine(HideSadFace());
            }
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