using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoceUI : MonoBehaviour
{
    [SerializeField] private Text _choiceTxt;
    public Text choiceTxt { get { return _choiceTxt; } }

    public void SetChoice(string choice)
    {
        _choiceTxt.text = choice;
    }
}
