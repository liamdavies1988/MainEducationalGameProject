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

    [Header("Button Feedback Icons")]
    [SerializeField] private GameObject tickIcon;
    [SerializeField] private GameObject crossIcon;

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
    IEnumerator ResetWrongBool()
    {
        yield return new WaitForSeconds(0.5f); // Length of your wobble
        anim.SetBool("IsWrong", false);
    }

    public void OnClick()
    {
        if (anim == null) anim = GetComponent<Animator>();// Double check we have the animator
        {
            if (isCorrectAnswer)
            {
                Debug.Log("Correct!");

                 // 1. Show the Tick, hide the Text
                if (tickIcon != null) tickIcon.SetActive(true);
                if (AnswerText != null) AnswerText.gameObject.SetActive(false);

                anim.SetBool("IsCorrect", true); //TESTING
                
                
                
                anim.SetTrigger("CoinIcon");
                GameManager.Instance.AddCoin();// Player wins a coin

                StartCoroutine(ResetButtonUI()); //TESTING
                
                StartCoroutine(NextQuestionDelay());// Starts next question after delay, giving time for the animation to play
            }
            else if (anim != null)
            {
                Debug.Log("Wrong!");

                // 2. Show the Cross, hide the Text
                if (crossIcon != null) crossIcon.SetActive(true);
                if (AnswerText != null) AnswerText.gameObject.SetActive(false);
                GameManager.Instance.WrongAnswerSound(); // Play the wrong answer sound effect
                // Change from SetTrigger to SetBool
                anim.SetBool("IsWrong", true);// This will trigger the wobble animation in the Animator
                StartCoroutine(ResetButtonUI());
                //StartCoroutine(ResetWrongBool());// This will reset the bool after the wobble animation is done
                StartCoroutine(NextQuestionDelay());// Starts next question after delay, giving time for the animation to play
                

            }
        }
    }

    

    // This is the clean function you wanted!
   
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
    
    anim.SetBool("IsCorrect", false);
    anim.SetBool("IsWrong", false);
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