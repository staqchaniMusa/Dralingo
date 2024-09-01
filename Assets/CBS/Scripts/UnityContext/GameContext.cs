using CBS.Scriptable;
using CBS.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.Video;
using VRBeats.ScriptableEvents;

namespace CBS.Context
{
    public class GameContext : MonoBehaviour
    {
        private CommonPrefabs Prefabs { get; set; }
        public GameObject Dragon;
        //public VideoPlayer videoPlayer;
        private List<LessonData> lessons;
        public List<LessonData> Lessons { get { return lessons; } set { lessons = value; } }
        public ProfileData profile { get; private set; }

        public AudioMixer mixer;

        public GameEvent OnVideoPlay;
        public GameEvent OnVideoPause;
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            AppContext.instance.game = this;
            Prefabs = CBSScriptable.Get<CommonPrefabs>();

            var prefab = Prefabs.GameContext;
            UIView.ShowWindow(prefab);
            Invoke(nameof(LoadProfile),0.2f);
            Application.targetFrameRate = 60;
           
        }

        public void PlayVideo(bool play)
        {
            if(play) { OnVideoPlay?.Invoke(); }
            else { OnVideoPause?.Invoke(); }
        }
        private void LoadProfile()
        {
            if (string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId)) return;
            AppContext.instance.DB.LoadUserData(CBSModule.Get<CBSAuthModule>().userId, resul =>
            {
                profile = resul;
            }, error => {
                profile = new ProfileData();

            Debug.Log(error);
            });
        }

        public void ShowLessonDetail(int lesson)
        {
            SoundsManager.instance.PlayClick(3);
            UIView.HideAll();
            var prefab = Prefabs.LessonDetail;
            GameObject lessonGO = UIView.ShowWindow(prefab);
            lessonGO.GetComponent<LessonDetailUI>().ShowLessonDetail(lesson);
        }
        public void ShowSetting()
        {
            SoundsManager.instance.PlayClick(1);
            HideGameContext();
            var prefab = Prefabs.Setting;
            UIView.ShowWindow(prefab);
        }
        public void ShowLessons()
        {
            SoundsManager.instance.PlayClick(3);
            BackToLesson();
        }
        public void BackToLesson()
        {
            
            UIView.HideAll();
            HideGameContext();
            var prefab = Prefabs.Lessons;
            UIView.ShowWindow(prefab);
        }

        public void ShowQuizUI(int lesson)
        {
            SoundsManager.instance.PlayClick(5);
            new PopupViewer().ShowLoadingPopup();
            AppContext.instance.DB.LoadQuiz(lesson, result =>
            {
                UIView.HideAll();
                var prefab = Prefabs.QuizUI;
                GameObject UI = UIView.ShowWindow(prefab);
                UI.GetComponent<QuizUI>().SetupQuiz(result,lesson);
                new PopupViewer().HideLoadingPopup();

            }, error =>
            {
                new PopupViewer().HideLoadingPopup();
            });
            
        }
        public void ShowFlashCard(int lesson)
        {
            SoundsManager.instance.PlayClick(4);
            new PopupViewer().ShowLoadingPopup();
            AppContext.instance.DB.LoadFlashCards(lesson, result =>
            {
                //UIView.HideAll();
                var prefab = Prefabs.FlashCardUI;
                GameObject UI = UIView.ShowWindow(prefab);
                UI.GetComponent<FlashCardUIController>().InitFlashCards(result,lesson);
                new PopupViewer().HideLoadingPopup();

            }, error =>
            {
                new PopupViewer().HideLoadingPopup();
            });
        }


        public void HideLessons()
        {
            //SoundsManager.instance.PlayClick();
            var prefab = Prefabs.Lessons;
            UIView.HideWindow(prefab);
            ShowGameContext();
        }

        public void HideGameContext()
        {
            //SoundsManager.instance.PlayClick();
            var prefab = Prefabs.GameContext;
            UIView.HideWindow(prefab);
            Dragon.SetActive(false);
        }

        public void ShowGameContext()
        {
            SoundsManager.instance.PlayClick(11);
            UIView.HideAll();
            var prefab = Prefabs.GameContext;
            UIView.ShowWindow(prefab);
            Dragon.SetActive(true);
        }

        internal void ShowLoading(bool show)
        {
            if(show) new PopupViewer().ShowLoadingPopup();
            else new PopupViewer().HideLoadingPopup();
        }

        internal void ShowError(string error)
        {
            new PopupViewer().ShowSimplePopup(new PopupRequest()
            {
                Title = "Error",
                Body = error
            });
        }
    }
}
