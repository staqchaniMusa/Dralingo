using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFadeable : MonoBehaviour
{
    bool doubleClicked;
    float lastClicedTime = float.MinValue;
    public CanvasGroup fadeable;
    bool fading;
    // Start is called before the first frame update
    void Start()
    {
        doubleClicked = false;
        fadeable.alpha = 0;
    }

    private void Update()
    {
        if (fading) return;
        if ((Time.time - lastClicedTime > 2f) && fadeable.alpha > 0)
        {
            StartCoroutine(FadeDown(1, 0));
        }
    }
    public void Clicked()
    {
        if (fading)
        {
            return;
        }
        //doubleClicked = Time.time - lastClicedTime < 0.5f;
        lastClicedTime = Time.time;
        Debug.Log("Fading up...");
        if(fadeable.alpha < 1f) StartCoroutine(FadeUp(0, 1));
    }

    IEnumerator FadeUp(float from,float to)
    {
        fading = true;
        while (from <=to)
        {
            fadeable.alpha += Mathf.Clamp01(from);
            from += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        fadeable.alpha = to;
        fading = false;
    }

    IEnumerator FadeDown(float from, float to)
    {
        fading = true;
        while (from >= to)
        {
            fadeable.alpha += Mathf.Clamp01(from);
            from -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        fadeable.alpha = to;
        fading = false;
    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause) Clicked();
    }
}
