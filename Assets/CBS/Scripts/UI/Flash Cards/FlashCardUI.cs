using CBS.Scriptable;
using CBS.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlashCardUI : MonoBehaviour
{
    int currentCard = -1;
    int currentLesson = 0;
    private FlashCardDataCollection flashCards;
    public VideoController videoController;
    public Image cardHolder;
    public GameObject loadingImg;
    public Button NextButton, PrevButton;
    List<FlashCardData> shuffleCards;
    public void Back()
    {
        //AppContext.instance.game.ShowLessonDetail(currentLesson);
       var ui= CBSScriptable.Get<CommonPrefabs>().FlashCardUI;
        UIView.HideWindow(ui);
    }
    public void InitFlashCards(FlashCardDataCollection cards,int lesson)
    {
        this.flashCards = cards;
        this.currentLesson = lesson;
        currentCard = -1;
        PrevButton.gameObject.SetActive(false);
        ShuffleCards();
    }

    void ShuffleCards()
    {
       shuffleCards = flashCards.flashcards.OrderBy(_ => Guid.NewGuid()).ToList();
       //shuffleCards = flashCards.flashcards;
        NextFlashCard();
    }
    public void NextFlashCard()
    {
        currentCard++;
        if (currentCard >= shuffleCards.Count - 1)
        {
            NextButton.gameObject.SetActive(false);
        }
        else NextButton.gameObject.SetActive(true);
        if (currentCard <= 0)
        {
            PrevButton.gameObject.SetActive(false);
        }
        else PrevButton.gameObject.SetActive(true);

        LoadContent();
    }

    public void PrevFlashCard() { 
        currentCard--;
        if( currentCard <= 0 )
        {
            PrevButton.gameObject.SetActive(false);
        }else PrevButton.gameObject.SetActive(true);
        if (currentCard >= shuffleCards.Count - 1)
        {
            NextButton.gameObject.SetActive(false);
        }else NextButton.gameObject.SetActive(true);
        LoadContent();
    }

    void LoadContent()
    {
        if (shuffleCards.Count > currentCard)
        {
            var card = shuffleCards[currentCard];
            if (card.isVideo) { LoadVideo(card.url); }
            else { LoadImage(card.url); }
        }
    }
    private void Update()
    {
        if (!videoController.isActiveAndEnabled) return;
        loadingImg.SetActive(videoController.IsBuffering);
    }
    void LoadVideo(string url)
    {
        AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(false);
        videoController.gameObject.SetActive(true) ;
        AppContext.instance.DB.LoadFileUrl(url, result =>
        {
            videoController.LoadVideo(result.ToString());
            AppContext.instance.game.ShowLoading(false);
        }, error =>
        {
            AppContext.instance.game.ShowError("Video Load Error!");
            AppContext.instance.game.ShowLoading(false);
        });
    }

    private void LoadImage(string url)
    {

        AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(true);
        videoController.gameObject.SetActive(false);
        AppContext.instance.DB.LoadFileUrl(url, result =>
        {
            StartCoroutine(DownloadImageFromUrl(result.ToString()));
           
        }, error =>
        {
            AppContext.instance.game.ShowError("Flash Card Load Error...");
            AppContext.instance.game.ShowLoading(false);
        });
    }
    string getUrl(string url)
    {
        string newUrl = "";
        
        newUrl = url.Replace(" ", "%20");
        return newUrl;
    }
    IEnumerator DownloadImageFromUrl(string url)
    {
        string httpUrl =getUrl(url);
        Debug.Log(httpUrl);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(httpUrl);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            AppContext.instance.game.ShowError("Flash Card Load Error!");
            yield break;
        }
        AppContext.instance.game.ShowLoading(false);
        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        cardHolder.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
