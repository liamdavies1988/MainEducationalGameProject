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
        if (anim == null) anim = GetComponent<Animator>();// Double check we have the animator
        {
            if (isCorrectAnswer)
            {
                Debug.Log("Correct!");

                PlayFeedbackAnimation(true);// Play the correct answer animation
                anim.SetTrigger("CoinIcon");
                GameManager.Instance.AddCoin();// Player wins a coin
                StartCoroutine(NextQuestionDelay());// Starts next question after delay, giving time for the animation to play
            }
            else if (anim != null)
            {
                Debug.Log("Wrong!");

                // Change from SetTrigger to SetBool
                anim.SetBool("IsWrong", true);// This will trigger the wobble animation in the Animator

                // We MUST turn it back to false after a delay, 
                // otherwise the button will stay in the 'Wrong' state forever.
                StartCoroutine(ResetWrongBool());// This will reset the bool after the wobble animation is done
                StartCoroutine(NextQuestionDelay());// Starts next question after delay, giving time for the animation to play

            }
        }
    }

    IEnumerator ResetWrongBool()
    {
        yield return new WaitForSeconds(0.5f); // Length of your wobble
        anim.SetBool("IsWrong", false);
    }

    // This is the clean function you wanted!
    private void PlayFeedbackAnimation(bool isRight)
    {
        if (isRight)
        {
            // Tell animator to play the Right sequence
            anim.SetTrigger("Pressed");
        }
        else
        {
            // Tell animator to play the Wrong sequence
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