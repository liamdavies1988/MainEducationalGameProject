using UnityEngine;
using TMPro;

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
            // Later you can add: GameManager.instance.AddCoin();
        }
        else
        {
            Debug.Log("Wrong answer, try again.");
        }
    }
}