
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoController : MonoBehaviour
{
    public Slider slider;
    public VideoPlayer player;
    bool isDone;
    bool hasWatched;
    bool isSeeking;
    bool isScrubbing;
    int bufferCount;
    float previousFrameCount;

    public bool IsBuffering {  get { return /*IsPlayeing && */( !player.isPrepared || bufferCount > 60f); } }
    public bool IsPlayeing { get { return player.isPlaying; } }
    public bool IsLooping { get { return player.isLooping; } }
    public bool IsPrepared { get { return player.isPrepared; } }
    public bool IsDone { get { return isDone; } }
    public bool HasWatched { get { return hasWatched; } }
    public double Time {  get { return player.time; } }
    public ulong Duration { get { return (ulong)(player.frameCount / player.frameRate); } }
    public double NTime { get { return Time / Duration; } }

    public Action OnVideoCompleted;
    public Action OnVidoPlayStatusChanged;
    public Action OnVideoPlayerReady;

    private void OnEnable()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        player.errorReceived += OnErrorReceived;
        player.loopPointReached += OnLoopPointReached;
        player.frameReady += OnFrameReady;
        player.prepareCompleted += OnPrepareCompleted;
        player.seekCompleted += OnSeekCompleted;
        player.started += OnStarted;
    }

  
    private void OnDisable()
    {
        //PauseVideo();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        inBackground = false;
       
        player.targetTexture.Release();
        player.errorReceived -= OnErrorReceived;
        player.loopPointReached -= OnLoopPointReached;
        player.frameReady -= OnFrameReady;
        player.prepareCompleted -= OnPrepareCompleted;
        player.seekCompleted -= OnSeekCompleted;
        player.started -= OnStarted;
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
        UpdateVidePlayer();
    }
   
    void UpdateVidePlayer()
    {
        if (!player.isPrepared || !player.isPlaying || isScrubbing) return;
        if (!isSeeking)
            slider.value = (float)NTime;
        // Check if frame count has changed
        if (slider.value == previousFrameCount)
        {
            // If frame count hasn't changed, it might be buffering
            bufferCount++;
        }
        else bufferCount = 0;
        if (!hasWatched && slider.value > 0.8f)
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
        if(!HasWatched)
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
        if(player.url == url) return;
        player.url = url;
        player.Prepare();
    }
    public void PlayVideo()
    {
        inBackground = false;
        if(!player.isPrepared) return ;
        player.Play();
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void PauseVideo()
    {
        inBackground = true;
        if(player == null || !player.isPlaying) return ;
        player.Pause();
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void RestartVideo()
    {
        if (!player.isPrepared) return;
        Seek(0);
        OnVidoPlayStatusChanged?.Invoke();

    }

    public void LoopVideo(bool toggle)
    {
        player.isLooping = toggle;
    }

    public void Seek(float nTime)
    {
        if(!player.canSetTime) return ;
        if (!player.isPrepared) return;
        //player.Stop();
        isSeeking = true;
        nTime = Mathf.Clamp01(nTime);
        player.time = nTime * Duration;
        OnVidoPlayStatusChanged?.Invoke();
    }

    public void IncrementPlaybackSpeed()
    {
        Seek(slider.value + 1f/Duration * 10f);
        /*if(!player.canSetPlaybackSpeed) return ;
        player.playbackSpeed += 1;
        player.playbackSpeed = Mathf.Clamp(player.playbackSpeed, 0, 10);*/
    }

    public void DecrementPlaybackSpeed()
    {
        Seek(slider.value - 1f/Duration * 10f);
        /*if (!player.canSetPlaybackSpeed) return;
        player.playbackSpeed -= 1;
        player.playbackSpeed = Mathf.Clamp(player.playbackSpeed, 0, 10);*/
    }


    private void OnApplicationPause(bool pause)
    {
       PauseVideo();
    }
  
}
