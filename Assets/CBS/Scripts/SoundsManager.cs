using PlayFab.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundsManager : SingletonMonoBehaviour<SoundsManager>
{
    public AudioSource source;
    public AudioClip ButtonClick;

    public void PlayClick()
    {
        if (PlayerPrefs.GetInt("music") == 0)
        {
            
            source?.PlayOneShot(ButtonClick);
        }
    }
}
