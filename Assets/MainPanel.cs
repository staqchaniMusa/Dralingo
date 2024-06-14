using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MainPanel : MonoBehaviour
{
    public Transform Setting;
    public Transform Lesson;
    public Transform startPoint;
    public float delay;

    Vector3 SettingPosition;
    Vector3 LessonPostion;

    private void Awake()
    {
        SettingPosition = Setting.transform.position;
        LessonPostion = Lesson.transform.position;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        
        Setting.position = startPoint.position;
        Lesson.position = startPoint.position;
        PlayNextAnimation(Lesson, LessonPostion, delay);
        PlayNextAnimation(Setting, SettingPosition, delay + 1f);
    }

    void PlayNextAnimation(Transform target, Vector3 destination, float delay)
    {
        target.DOMove(destination,0.5f).OnComplete(() => {
            target.DOMove(destination + Vector3.up * 10, 0.5f);
            target.DOScale(Vector3.one * 0.5f, 0.5f).OnComplete(() =>
            {
                target.DOMove(destination, 1f).SetEase(Ease.OutBounce);
                target.DOScale(Vector3.one * 0.4f, 1f);
            });
        }).SetEase(Ease.InOutSine).SetDelay(delay);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
