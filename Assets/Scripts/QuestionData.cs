using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData",order =1)]
public class QuestionData : ScriptableObject
{
    public string QuestionText;
    public string Topic;
    //Correct answer will always be box A and then box A position will be randomised.
    public string[] QuestionAnswer;


}
