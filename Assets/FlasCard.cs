using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlasCard : MonoBehaviour
{

    [SerializeField] private Image cardHolder;
    public VideoController videoController;
    private FlashCardData Card;
    public bool loadingContent;
    public bool contentTypeisImage {  get; private set; }
    bool init = false;

    private void Start()
    {
        init = true;
    }
    internal void LoadContent()
    {
        if (Card == null) return;
        OnLoadContent?.Invoke(this);
        if (Card.isVideo) { LoadVideo(Card.url); }
        else { LoadImage(Card.url); }
       // Debug.Log("Loading Content...");
    }
    Action<FlasCard> OnLoadContent;
    Action OnFullScreen;
    internal void SetCardData(FlashCardData card, Action<FlasCard> onLoad,Action onFullscreen)
    {
        this.Card = card;
        this.OnLoadContent = onLoad;
        this.OnFullScreen = onFullscreen;
        //Debug.Log("Card Data Set : " + card.url);
    }

    public void FullScreen()
    {
        OnFullScreen?.Invoke();
    }
    private void OnEnable()
    {
        LoadContent();
    }
    private void OnDisable()
    {
        if(init) 
        StopVideo();
    }
    internal void StopVideo()
    {
        videoController.PauseVideo();
    }

    void LoadVideo(string url)
    {
        //AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(false);
        videoController.gameObject.SetActive(true);
        loadingContent = true;
        AppContext.instance.DB.LoadFileUrl(url, result =>
        {
            loadingContent = false;
            if(gameObject.activeInHierarchy)
            {

                videoController.LoadVideo(getUrl(result.ToString()));
            }
            AppContext.instance.game.ShowLoading(false);
        }, error =>
        {
            loadingContent = false;
            if(gameObject.activeInHierarchy)
            AppContext.instance.game.ShowError("Video Load Error!");
            AppContext.instance.game.ShowLoading(false);
        });
    }

    private void LoadImage(string url)
    {
        loadingContent = true;
        contentTypeisImage = true;
        //AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(true);
        videoController.gameObject.SetActive(false);
        AppContext.instance.DB.LoadFileUrl(url, result =>
        {
            if(gameObject.activeInHierarchy)
            StartCoroutine(DownloadImageFromUrl(result.ToString()));

        }, error =>
        {
            loadingContent = false;
            if(gameObject.activeInHierarchy)
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
        string httpUrl = getUrl(url);
        Debug.Log(httpUrl);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(httpUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            AppContext.instance.game.ShowError("Flash Card Load Error!");
            yield break;
        }
        AppContext.instance.game.ShowLoading(false);
        loadingContent = false;
        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        cardHolder.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
