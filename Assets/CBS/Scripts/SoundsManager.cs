using PlayFab.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundsManager : SingletonMonoBehaviour<SoundsManager>
{
    public AudioSource source;
    private AudioSource source2;
    public AudioClip[] ButtonClicks;
    public AudioMixerGroup mixer;
    public AudioMixer AudioMixer;
    public void PlayClick(int i = 0)
    {
        //if (PlayerPrefs.GetInt("music") == 0)
        {

            source?.PlayOneShot(ButtonClicks[i]);
        }
    }

    protected override void Initialize()
    {
        bool isOn = PlayerPrefs.GetInt("music", 1) == 1;
        AudioMixer.SetFloat("Music", isOn ? 0f : -80f);
    }
    /// <summary>
    /// This method is used to play a music in background
    /// </summary>
    /// <param name="clip">Music to be played in Background</param>
    public void PlayBGM(AudioClip clip)
    {
        //if (PlayerPrefs.GetInt("music") != 0) return;
            if (source2 == null) { source2 = gameObject.AddComponent<AudioSource>();
            source2.outputAudioMixerGroup = mixer;
        }
        source2.loop = true;
        source2.clip = clip;
        source2.Play();
    }

    internal void PauseBGM()
    {
        if(source2 != null) { source2.Pause(); }
    }

    public void ResumeBGM()
    {
        if (source2 != null) source2.Play();
    }
}
