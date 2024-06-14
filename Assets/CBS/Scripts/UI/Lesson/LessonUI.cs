using CBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LessonUI : MonoBehaviour
{
    public Button[] Lessons;

    private string lessonPrefKey = "isUnlocked";
    private void Awake()
    {
        //LoadLesson();
    }

    private void OnEnable()
    {
        LoadLesson();
    }
    void LoadLesson()
    {
        AppContext.instance.DB.LoadLessons(result =>
        {
            AppContext.instance.game.Lessons = result;
            AppContext.instance.game.ShowLoading(false);
            InitLessons();
        }, error =>
        {
            AppContext.instance.game.ShowLoading(false);
            AppContext.instance.game.ShowError(error);
        });
    }

    void InitLessons()
    {
        for (int i = 0; i < Lessons.Length; i++)
        {
            if (i == 0 || isLessonUnlocked(i))
            {
                Lessons[i].onClick.RemoveAllListeners();
                int lesson = i;
                Lessons[i].transform.GetChild(0).gameObject.SetActive(false);
                Lessons[i].onClick.AddListener(() => ShowLessonDetail(lesson));
            }
        }
    }
    void ShowLessonDetail(int lesson)
    {
        Debug.LogFormat("Lesson No {0} event initialized", lesson);
        AppContext.instance.game.ShowLessonDetail(lesson);
    }
    bool checkIfNotNull(int lesson)
    {
        return AppContext.instance.game.profile != null && AppContext.instance.game.profile.UserLessons != null && AppContext.instance.game.profile.UserLessons.Count > lesson - 1;
    }
    bool isLessonUnlocked(int lesson)
    {
        if (AppContext.instance.game.Lessons == null || AppContext.instance.game.Lessons.Count == 0)
            return false;
        //return PlayerPrefs.GetInt(lessonPrefKey + lesson) == 1 || CBS.CBSModule.Get<CBS.CBSAuthModule>().isAdmin;
        return CBSModule.Get<CBSAuthModule>().isAdmin || (CBSModule.Get<CBSAuthModule>().userId != null && lesson == 1) || checkIfNotNull(lesson-1) && AppContext.instance.game.profile.UserLessons[lesson-1].hasClearedLesson;
    }
}
