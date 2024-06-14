using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizData
{
    public string answer { get; set; }
    public List<string> answers { get; set; }
    public string question { get; set; }

    public string answerByUser;
}
