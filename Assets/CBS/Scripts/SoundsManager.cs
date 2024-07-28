using PlayFab.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundsManager : SingletonMonoBehaviour<SoundsManager>
{
    public AudioSource source;
    private AudioSource source2;
    public AudioClip[] ButtonClicks;

    public void PlayClick(int i = 0)
    {
        if (PlayerPrefs.GetInt("music") == 0)
        {

            source?.PlayOneShot(ButtonClicks[i]);
        }
    }
    /// <summary>
    /// This method is used to play a music in background
    /// </summary>
    /// <param name="clip">Music to be played in Background</param>
    public void PlayBGM(AudioClip clip)
    {
        if (PlayerPrefs.GetInt("music") != 0) return;
            if (source2 == null) { source2 = gameObject.AddComponent<AudioSource>(); }
        source2.loop = true;
        source2.clip = clip;
        source2.Play();
    }
}
