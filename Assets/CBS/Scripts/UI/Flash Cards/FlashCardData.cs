using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashCardDataCollection 
{
    public List<FlashCardData> flashcards;
}

public class FlashCardData
{
    public string url { get; set; }
    public bool isVideo { get; set; }
}
