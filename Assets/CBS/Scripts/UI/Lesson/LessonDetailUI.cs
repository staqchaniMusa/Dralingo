using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using CBS.UI;
using Unity.VisualScripting;
using CBS;
using CBS.Context;
using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos.UI;
using RenderHeads.Media.AVProVideo.Demos;

public class LessonDetailUI : MonoBehaviour
{
    [SerializeField] private Image Title;
    [SerializeField] private Button flashCardBtn;
    [SerializeField] private Button quizBtn;
    [SerializeField] private Button infoBtn;
    [SerializeField] private Image normalScreenPauseText;
    [SerializeField] private TextMeshProUGUI[] videoCurrentTime;
    [SerializeField] private TextMeshProUGUI[] videoTotalTime;
    [SerializeField] private Slider[] videoSlider;
    [SerializeField] private Animator screenAnimator;
    [SerializeField] private LoadingScreen loading;
    //[SerializeField] AppContext.instance.game.videoPlayer AppContext.instance.game.videoPlayer;
    //[SerializeField] private VideoController videoPlayer;
    [SerializeField] private MediaPlayer mediaPlayer;
    [SerializeField] private MediaPlayerController videoPlayer;
    [SerializeField] Sprite[] lessonsSprites;
    [SerializeField] Sprite playImage, pauseImage;
    [SerializeField] Sprite fullScreenSprite, normalScreenSprite;
    [SerializeField] Image fullScreenButton;
    [SerializeField] Scrubber Scrubber;
    private string quizPrefsKey = "hasQuiz";
    private string flasCardPrefsKey = "hasFlashcard";
    private string infoPrefsKey = "hasSecretKey";

    private int currentLesson;

    bool playing;

    private void Awake()
    {
        //AppContext.instance.game.videoPlayer.Prepare();
        //AppContext.instance.game.videoPlayer.seekCompleted += VideoPlayerCompleted;
        //AppContext.instance.game.videoPlayer.prepareCompleted += Prepared;
    }
    bool backPress;
    public void Back()
    {
       
        videoPlayer.PauseVideo();
        //mediaPlayer.Pause();
        StartCoroutine(BackButton());
    }

    public MediaPlayerUI playerUI;
    IEnumerator BackButton()
    {
        //playerUI.SetInBackground(true);
        SoundsManager.instance.PlayClick(2);
        yield return new WaitForEndOfFrame();
        AppContext.instance.game.BackToLesson();
        backPress = false;
    }
    public void ResumeVideo() { videoPlayer.PlayVideo(); }
    public void PauseVideo(bool pause)
    {
        SoundsManager.instance.PlayClick(1);
        if (videoPlayer.IsPlayeing)
            videoPlayer.PauseVideo();
        else videoPlayer.PlayVideo();
       /* if (mediaPlayer && mediaPlayer.Control != null)
        {
            *//*if ( mediaPlayer.Info.HasAudio())
            {
                if (mediaPlayer.Control.IsPlaying())
                {
                    *//*if (_overlayManager)
                    {
                        _overlayManager.TriggerFeedback(OverlayManager.Feedback.Pause);
                    }*//*
                    //_isAudioFadingUpToPlay = false;
                }
                else
                {
                    //_isAudioFadingUpToPlay = true;
                    Play();
                }
                //_audioFadeTime = 0f;
            }
            else*//*
            {
                if (mediaPlayer.Control.IsPlaying())
                {
                    Pause();
                    normalScreenPauseText.sprite = playImage;
                }
                else
                {
                    Play();
                    normalScreenPauseText.sprite = pauseImage;
                }
            }
        }*/
    }

    private void Play()
    {
        if (mediaPlayer && mediaPlayer.Control != null)
        {
           /* if (_overlayManager)
            {
                _overlayManager.TriggerFeedback(OverlayManager.Feedback.Play);
            }*/
            mediaPlayer.Play();
#if UNITY_ANDROID
					Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        }
    }

    private void Pause(bool skipFeedback = false)
    {
        if (mediaPlayer && mediaPlayer.Control != null)
        {
            if (!skipFeedback)
            {
                /*if (_overlayManager)
                {
                    _overlayManager.TriggerFeedback(OverlayManager.Feedback.Pause);
                }*/
            }
            mediaPlayer.Pause();
#if UNITY_ANDROID
					Screen.sleepTimeout = SleepTimeout.SystemSetting;
#endif
        }
    }

    double previousTime = 0;
    double currentTime = 0;
    int counter = 0;
    public float forwardStep = 0.5f;
    private void Update()
    {
        loading.gameObject.SetActive(!videoPlayer.IsPrepared);
        //loading.gameObject.SetActive(mediaPlayer.Control.IsPlaying());
    }
    
    public void ShowFullScreen(bool fullScreen)
    {
        SoundsManager.instance.PlayClick();
        bool isFullScreen = screenAnimator.GetBool("screen");
        screenAnimator.SetBool("screen",!isFullScreen);
        fullScreenButton.sprite = isFullScreen ? fullScreenSprite : normalScreenSprite;
        //videoPlayer.slider = videoSlider[fullScreen ? 1 : 0];
    }

    bool checkIfNotNull(int lesson)
    {
        return AppContext.instance.game.profile != null && AppContext.instance.game.profile.UserLessons != null && AppContext.instance.game.profile.UserLessons.Count > lesson;
    }
    public void ShowLessonDetail(int lesson)
    {
        //loading.gameObject.SetActive(true);
        playerUI.TriggerStalled();
        currentLesson = lesson;
        quizBtn.onClick.RemoveAllListeners();
        flashCardBtn.onClick.RemoveAllListeners();
        infoBtn.onClick.RemoveAllListeners();
        videoPlayer.slider = videoSlider[0];
        videoPlayer.slider.value = 0;
        normalScreenPauseText.sprite = playImage;
        
        LoadContent();
        Title.sprite = lessonsSprites[lesson];
       
        if (CBSModule.Get<CBSAuthModule>().isAdmin || checkIfNotNull(lesson) && AppContext.instance.game.profile.UserLessons[currentLesson].hasWatchVideo)
        {
            quizBtn.transform.GetChild(0).gameObject.SetActive(false);
            quizBtn.onClick.AddListener(ShowQuiz);
            flashCardBtn.onClick.AddListener(ShowFlashCard);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(false);
            Scrubber.enabled = false;
           
            playerUI.CreateTimelineDragEvents();
        }
        else
        {
            quizBtn.transform.GetChild(0).gameObject.SetActive(true);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(true);
            Scrubber.enabled = true;
            playerUI.RemoveTimelineDragEvents();
            Scrubber.GetComponent<Slider>().enabled = true;
        }
        if (CBSModule.Get<CBSAuthModule>().isAdmin || checkIfNotNull(lesson) && AppContext.instance.game.profile.UserLessons[currentLesson].hasClearedLesson) infoBtn.onClick.AddListener(ShowSecretCode);
       
    }
    string getUrl(string url)
    {
        string newUrl = "";

        newUrl = url.Replace(" ", "%20");
        Debug.Log(newUrl);
        return newUrl;
    }
    void LoadContent(int errorCount=0)
    {
        //Debug.Log("Downloading video url...");
        loading.gameObject.SetActive(true);
        AppContext.instance.DB.LoadFileUrl(AppContext.instance.game.Lessons[currentLesson].videoUrl, result =>
        {
            loading.gameObject.SetActive(false);
            /*mediaPlayer.OpenMedia(new MediaPath(getUrl(result.ToString()),
MediaPathType.AbsolutePathOrURL), autoPlay: true);*/
            videoPlayer.LoadVideo(getUrl(result.ToString()));
            //playerUI.SetInBackground(false);
        }, error =>
        {
            //Debug.Log(error);
            if (errorCount > 3)
                AppContext.instance.game.ShowError("Internet connection error");
            else LoadContent(++errorCount);
        });
    }
    private void OnEnable()
    {
        videoPlayer.OnVidoPlayStatusChanged += OnVideoStatusChanged;
        videoPlayer.OnVideoCompleted += OnVideoCompleted;
        videoPlayer.OnVideoPlayerReady += OnVideoPlayerReady;
        AppContext.instance.game.PlayVideo(false);
        //Invoke(nameof(AddEvent), 0.5f);
    }

   
    private void OnDisable()
    {
        videoPlayer.OnVidoPlayStatusChanged -= OnVideoStatusChanged;
        videoPlayer.OnVideoCompleted -= OnVideoCompleted;
        videoPlayer.OnVideoPlayerReady -= OnVideoPlayerReady;
        AppContext.instance.game.PlayVideo(true);
        //mediaPlayer.Events.RemoveListener(HandleEvent);
    }

    void OnVideoStatusChanged()
    {
        if (!videoPlayer.IsPlayeing)
        {
            //AppContext.instance.game.videoPlayer.Pause();
            normalScreenPauseText.sprite = playImage;
        }
        else
        {
            //AppContext.instance.game.videoPlayer.Play();
            normalScreenPauseText.sprite = pauseImage;
            //playerUI.SetInBackground(false);
        }
    }
    private void OnVideoCompleted()
    {
        VideoPlayerCompleted();
    }

    void OnVideoPlayerReady()
    {
        //loading.gameObject.SetActive(false);
    }
    void VideoPlayerCompleted()
    {
        Debug.Log("1 Video Watched");
        if (string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId) || AppContext.instance.game.profile != null && AppContext.instance.game.profile.UserLessons != null /*&& (AppContext.instance.game.profile.UserLessons.Count == currentLesson)*/)
        {
            Debug.Log("2");

            quizBtn.onClick.AddListener(ShowQuiz);
            flashCardBtn.onClick.AddListener(ShowFlashCard);
            quizBtn.transform.GetChild(0).gameObject.SetActive(false);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(false);
            playerUI.CreateTimelineDragEvents();
            if (!string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId))
            {
                UserLesson lessson = new UserLesson();
                lessson.hasWatchVideo = true;
                AppContext.instance.DB.UpdateLesson(currentLesson, lessson);
                AppContext.instance.game.profile.UserLessons.Add(lessson);
            }
            
        }
        
        //normalScreenPauseText.sprite = playImage;
    }
    void ShowFlashCard()
    {
        //mediaPlayer.Stop();
        //mediaPlayer.EndOpenChunkedVideoFromBuffer();
        mediaPlayer.AutoStart = false;
        videoPlayer?.PauseVideo();
        AppContext.instance.game.ShowFlashCard(currentLesson);
        //playerUI.SetInBackground(true);
    }

    void ShowQuiz()
    {
        mediaPlayer.AutoStart = false;
        videoPlayer?.PauseVideo();
        //playerUI.SetInBackground(true);
        AppContext.instance.game.ShowQuizUI(currentLesson);
    }

    void ShowSecretCode()
    {
        new PopupViewer().ShowRewardPopup(new PopupRequest()
        {
            Title = "Here is your secret code",
            Body = AppContext.instance.game.Lessons[currentLesson].secretCode
        });
        SoundsManager.instance.PlayClick(7);
    }

    
}
