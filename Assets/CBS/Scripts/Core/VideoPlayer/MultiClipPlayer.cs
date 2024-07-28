using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
[RequireComponent(typeof(VideoPlayer))]
public class MultiClipPlayer : MonoBehaviour
{
    public vClip[] videoClips;
    VideoPlayer player;
    int currentLoop;
    int currentClip;
    private void Awake()
    {
        player = GetComponent<VideoPlayer>();

    }

    private void OnEnable()
    {
        currentClip = -1;
        currentLoop = -1;
        player.loopPointReached += OnVideoEnded;
        PlayNextVideo();
    }

    private void OnDisable()
    {
        player.loopPointReached -= OnVideoEnded;
    }
    private void OnVideoEnded(VideoPlayer source)
    {
        currentLoop++;
        if (!videoClips[currentClip].loop || currentLoop > videoClips[currentClip].loopTime) PlayNextVideo();
    }

    

    void PlayNextVideo()
    {
        if (videoClips.Length == 0) return;
        currentClip++;
        if (currentClip == videoClips.Length) currentClip = 0;
        vClip clip = videoClips[currentClip];
        player.clip = clip.videoClip;
        player.isLooping = clip.loop;
        player.Play();
        
    }
}

[Serializable] public class vClip
{
    public VideoClip videoClip;
    public bool loop;
    public int loopTime;
}