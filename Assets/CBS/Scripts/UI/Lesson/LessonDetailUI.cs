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
    [SerializeField] private VideoController videoPlayer;
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
        /*if (backPress) return;
        backPress = true;
        if (AppContext.instance.game.videoPlayer.isPlaying) AppContext.instance.game.videoPlayer.Pause();
        *//*AppContext.instance.game.videoPlayer.prepareCompleted -= Prepared;
        AppContext.instance.game.videoPlayer.seekCompleted -= VideoPlayerCompleted;*//*
        //AppContext.instance.game.videoPlayer.EnableAudioTrack(0, false);
        StartCoroutine(BackButton());*/
        videoPlayer.PauseVideo();
        StartCoroutine(BackButton());
    }

    IEnumerator BackButton()
    {
        SoundsManager.instance.PlayClick(2);
        yield return new WaitForEndOfFrame();
        AppContext.instance.game.BackToLesson();
        backPress = false;
    }
    public void PauseVideo(bool pause)
    {
        SoundsManager.instance.PlayClick(1);
        if (videoPlayer.IsPlayeing)
            videoPlayer.PauseVideo();
        else videoPlayer.PlayVideo();
        
    }
    /*void Prepared(VideoPlayer vPlayer)
    {
        
        double maxTime = (AppContext.instance.game.videoPlayer.frameCount / AppContext.instance.game.videoPlayer.frameRate);
        videoSlider[0].maxValue = (float)maxTime;
        videoSlider[1].maxValue = (float)maxTime;
    }*/
    double previousTime = 0;
    double currentTime = 0;
    int counter = 0;
    public float forwardStep = 0.5f;
    private void Update()
    {
        loading.gameObject.SetActive(videoPlayer.IsBuffering);
        /*if (AppContext.instance.game.videoPlayer.isPlaying)
        {
            foreach (var item in videoSlider)
            {
                
                currentTime = (AppContext.instance.game.videoPlayer.frame / AppContext.instance.game.videoPlayer.frameRate);
                item.value = (float)currentTime;
                if (counter % 30 == 0)
                {
                    loading.gameObject.SetActive(previousTime == currentTime);
                    previousTime = currentTime;
                    counter = 0;
                }
                counter++;
            }
        }
        else loading.gameObject.SetActive(false);*/
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
        loading.gameObject.SetActive(true);
        currentLesson = lesson;
        quizBtn.onClick.RemoveAllListeners();
        flashCardBtn.onClick.RemoveAllListeners();
        infoBtn.onClick.RemoveAllListeners();
        videoPlayer.slider = videoSlider[0];
        videoPlayer.slider.value = 0;
        normalScreenPauseText.sprite = playImage;
        //AppContext.instance.game.videoPlayer.url = AppContext.instance.game.Lessons[lesson].videoUrl;
        //AppContext.instance.game.videoPlayer.EnableAudioTrack(0, true);
        LoadContent();
        Title.sprite = lessonsSprites[lesson];
        //AppContext.instance.game.videoPlayer.Play();
        //normalScreenPauseText.sprite = pauseImage;
        if (CBSModule.Get<CBSAuthModule>().isAdmin || checkIfNotNull(lesson) && AppContext.instance.game.profile.UserLessons[currentLesson].hasWatchVideo)
        {
            quizBtn.transform.GetChild(0).gameObject.SetActive(false);
            quizBtn.onClick.AddListener(ShowQuiz);
            flashCardBtn.onClick.AddListener(ShowFlashCard);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(false);
            Scrubber.enabled = false;
        }
        else
        {
            quizBtn.transform.GetChild(0).gameObject.SetActive(true);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(true);
            Scrubber.enabled = true;
            Scrubber.GetComponent<Slider>().enabled = true;
        }
        if (CBSModule.Get<CBSAuthModule>().isAdmin || checkIfNotNull(lesson) && AppContext.instance.game.profile.UserLessons[currentLesson].hasClearedLesson) infoBtn.onClick.AddListener(ShowSecretCode);
        /*if (PlayerPrefs.GetInt(quizPrefsKey + lesson) == 1) quizBtn.onClick.AddListener(ShowQuiz);
        if (PlayerPrefs.GetInt(flasCardPrefsKey + lesson) == 1) flashCardBtn.onClick.AddListener(ShowFlashCard);
        if (PlayerPrefs.GetInt(infoPrefsKey + lesson) == 1) infoBtn.onClick.AddListener(ShowSecretCode);*/

    }

    void LoadContent()
    {
        //Debug.Log("Downloading video url...");
        AppContext.instance.DB.LoadFileUrl(AppContext.instance.game.Lessons[currentLesson].videoUrl, result =>
        {
            
            videoPlayer.LoadVideo(result.ToString());
        }, error =>
        {
            Debug.Log(error);
            AppContext.instance.game.ShowError(error);
        });
    }
    private void OnEnable()
    {
        videoPlayer.OnVidoPlayStatusChanged += OnVideoStatusChanged;
        videoPlayer.OnVideoCompleted += OnVideoCompleted;
        videoPlayer.OnVideoPlayerReady += OnVideoPlayerReady;
    }

    private void OnDisable()
    {
        videoPlayer.OnVidoPlayStatusChanged -= OnVideoStatusChanged;
        videoPlayer.OnVideoCompleted -= OnVideoCompleted;
        videoPlayer.OnVideoPlayerReady -= OnVideoPlayerReady;
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
        }
    }
    private void OnVideoCompleted()
    {
        VideoPlayerCompleted();
    }

    void OnVideoPlayerReady()
    {
        loading.gameObject.SetActive(false);
    }
    void VideoPlayerCompleted()
    {
        //AppContext.instance.game.videoPlayer.seekCompleted -= VideoPlayerCompleted;
        //PlayerPrefs.SetInt(quizPrefsKey + currentLesson, 1);
        //PlayerPrefs.SetInt(flasCardPrefsKey + currentLesson, 1);
        
        if (string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId) || AppContext.instance.game.profile != null && AppContext.instance.game.profile.UserLessons != null && AppContext.instance.game.profile.UserLessons.Count == currentLesson)
        {
            
            if (!string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId))
            {
                UserLesson lessson = new UserLesson();
                lessson.hasWatchVideo = true;
                AppContext.instance.DB.UpdateLesson(currentLesson, lessson);
                AppContext.instance.game.profile.UserLessons.Add(lessson);
            }
            quizBtn.onClick.AddListener(ShowQuiz);
            flashCardBtn.onClick.AddListener(ShowFlashCard);
            quizBtn.transform.GetChild(0).gameObject.SetActive(false);
            flashCardBtn.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        normalScreenPauseText.sprite = playImage;
    }
    void ShowFlashCard()
    {
        
        videoPlayer.PauseVideo();
        AppContext.instance.game.ShowFlashCard(currentLesson);
    }

    void ShowQuiz()
    {
        
        videoPlayer.PauseVideo();
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
