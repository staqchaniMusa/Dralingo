using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class MediaPlayerController : MonoBehaviour
{
    public Slider slider;
    public MediaPlayer mediaPlayer;
    bool isDone;
    bool hasWatched;
    bool isSeeking;
    bool isScrubbing;
    bool isPrepared;
    bool isBuffering;
    int bufferCount;
    float previousFrameCount;

    //public bool IsBuffering { get { return /*IsPlayeing && */(!isPrepared || bufferCount > 60f); } }
    public bool IsBuffering { get { return isBuffering; } }
    public bool IsPlayeing { get { return mediaPlayer.Control.IsPlaying(); } }
    public bool IsLooping { get { return mediaPlayer.Control.IsLooping(); } }
    public bool IsPrepared { get { return isPrepared; } }
    public bool IsDone { get { return isDone; } }
    public bool HasWatched { get { return hasWatched; } }
    public double Time { get { return mediaPlayer.Control.GetCurrentTime(); } }
    public double Duration { get { return mediaPlayer.Info.GetDuration(); } }
    public double NTime { get { return Time / Duration; } }

    public Action OnVideoCompleted;
    public Action OnVidoPlayStatusChanged;
    public Action OnVideoPlayerReady;

    private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (mediaPlayer != null)
        {
            mediaPlayer.Events.AddListener(HandleEvent);
            return;
        }
       
    }


    private void OnDisable()
    {
        //PauseVideo();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        inBackground = false;
        if (mediaPlayer != null)
        {
            mediaPlayer.Events.RemoveListener(HandleEvent);
            return;
        }
       
    }

    public void StartScrub()
    {
        isScrubbing = true;
    }

    public void StopScrub()
    {
        isScrubbing = false;
        Seek(slider.value);
    }
    // Update is called once per frame
    void Update()
    {
        if (mediaPlayer != null) { UpdatMediaPlayer(); }
       
    }
    void UpdatMediaPlayer()
    {
        if (!mediaPlayer.Control.IsPlaying() || isScrubbing) return;
        if (!isSeeking)
            slider.value = (float)NTime;
        // Check if frame count has changed
        if (slider.value == previousFrameCount)
        {
            // If frame count hasn't changed, it might be buffering
            bufferCount++;
        }
        else bufferCount = 0;
        if (!hasWatched && (mediaPlayer.Control.GetCurrentTime()/ mediaPlayer.Info.GetDuration()) > 0.8f)
        {
            OnWatchedVideo();
        }
        // Update previous frame count
        previousFrameCount = slider.value;
    }
  
    private void OnStarted(VideoPlayer source)
    {

    }
    private void OnWatchedVideo()
    {
        if (!HasWatched)
        {
            hasWatched = true;
            Debug.Log("Video Watched successfully");
            OnVideoCompleted?.Invoke();
        }
    }
    private void OnSeekCompleted(VideoPlayer source)
    {
        isDone = false;
        isSeeking = false;
        PlayVideo();
    }
    bool inBackground;
    private void OnPrepareCompleted(VideoPlayer source)
    {
        isDone = false;
        OnVideoPlayerReady?.Invoke();
        //Debug.LogFormat("Video Resolution {0} x {1}", source.texture.width, source.texture.height);
        //Debug.LogFormat("Target Texture Resolution {0} x {1}", source.targetTexture.width, source.targetTexture.height);
        source.targetTexture.Release();
        source.targetTexture.width = source.texture.width;
        source.targetTexture.height = source.texture.height;
        source.targetTexture.Create();
        //PlayVideo();
        Debug.Log("Video Player is Ready");
        if (!inBackground)
        {
            PlayVideo();
        }
    }

    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {

    }

    private void OnLoopPointReached(VideoPlayer source)
    {
        isDone = true;
        OnVidoPlayStatusChanged?.Invoke();
        //OnVideoCompleted?.Invoke();
    }

    private void OnErrorReceived(VideoPlayer source, string message)
    {
        Debug.Log("On Video Error: " + message);
    }


    public void LoadVideo(string url)
    {
        // Debug.Log(url);
        if (mediaPlayer.MediaPath.Path == url) return;
        
        mediaPlayer.OpenMedia(new MediaPath(url,
MediaPathType.AbsolutePathOrURL), autoPlay: true);
        isPrepared = false;
    }
    public void PlayVideo()
    {
        inBackground = false;
       // if (!isPrepared) return;
        mediaPlayer.Play();
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void PauseVideo()
    {
        inBackground = true;
        if (mediaPlayer == null || !IsPlayeing) return;
        mediaPlayer.Pause();
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void RestartVideo()
    {
        if (!isPrepared) return;
        Seek(0);
        OnVidoPlayStatusChanged?.Invoke();

    }

    public void LoopVideo(bool toggle)
    {
        mediaPlayer.Control.SetLooping(toggle);
    }

    public void Seek(double nTime)
    {
        if (!isPrepared) return;
        //player.Stop();
        isSeeking = true;
        nTime = Mathf.Clamp01((float)nTime);
        //player.time = nTime * Duration;
        mediaPlayer.Control.Seek(nTime * Duration);
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void IncrementPlaybackSpeed()
    {
        Seek(slider.value + 1f / Duration * 10f);
        /*if(!player.canSetPlaybackSpeed) return ;
        player.playbackSpeed += 1;
        player.playbackSpeed = Mathf.Clamp(player.playbackSpeed, 0, 10);*/
    }

    public void DecrementPlaybackSpeed()
    {
        Seek(slider.value - 1f / Duration * 10f);
        /*if (!player.canSetPlaybackSpeed) return;
        player.playbackSpeed -= 1;
        player.playbackSpeed = Mathf.Clamp(player.playbackSpeed, 0, 10);*/
    }


    private void OnApplicationPause(bool pause)
    {
        PauseVideo();
    }
    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        //Debug.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            Debug.LogError("Error: " + code);
            isPrepared = false;

        }
        else if (eventType == MediaPlayerEvent.EventType.Stalled)
        {
            isBuffering = true;
        }
        else if (eventType == MediaPlayerEvent.EventType.Unstalled)
        {
            isBuffering = false;
        }
        else if (eventType == MediaPlayerEvent.EventType.FinishedBuffering)
        {
            //loading.gameObject.SetActive(false);
        }
        else if (eventType == MediaPlayerEvent.EventType.FinishedSeeking)
        {
            isSeeking = false;
            isDone = false;
        }
        else if (eventType == MediaPlayerEvent.EventType.StartedSeeking)
        {
            isSeeking = true;
        }
        else if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            isDone = true;
            OnVidoPlayStatusChanged?.Invoke();
        }
        else if (eventType == MediaPlayerEvent.EventType.FirstFrameReady) { 
            OnVideoPlayerReady?.Invoke();
            isPrepared = true;
        }
    }
    
}