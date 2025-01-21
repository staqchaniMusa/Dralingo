using CBS.UI;
using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlasCard : MonoBehaviour
{

    [SerializeField] private RawImage cardHolder;
    //public VideoController videoController;
    public MediaPlayer mediaPlayer;
    public GameObject videoContainer;
    private FlashCardData Card;
    public bool loadingContent;
    public bool contentTypeisImage {  get; private set; }
    bool init = false;
    private Texture thumbnail;

    
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
    Action<Texture, MediaPlayer> OnFullScreen;
    internal void SetCardData(FlashCardData card, Action<FlasCard> onLoad,Action<Texture,MediaPlayer> onFullscreen)
    {
        this.Card = card;
        this.OnLoadContent = onLoad;
        this.OnFullScreen = onFullscreen;
        //Debug.Log("Card Data Set : " + card.url);
    }

    public void FullScreen()
    {
        //thumbnail = this.Card.isVideo ? videoController.player.texture : cardHolder.texture;
        OnFullScreen?.Invoke(cardHolder.texture,mediaPlayer);
    }
    private void OnEnable()
    {
        //if(init)
        LoadContent();
    }
    private void OnDisable()
    {
        if (init)
        {
            StopVideo();
            if(Card.isVideo)
            mediaPlayer.Events.RemoveListener(HandleEvent);
        }
    }
    internal void StopVideo()
    {
        //videoController?.PauseVideo();
        if (Card.isVideo)
            mediaPlayer.Stop();
    }

    void LoadVideo(string url)
    {
        //AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(false);
        videoContainer.gameObject.SetActive(true);
        loadingContent = true;
        AppContext.instance.DB.LoadFileUrl(url, result =>
        {
            //loadingContent = false;
            if(gameObject.activeInHierarchy)
            {

                //videoController?.LoadVideo(getUrl(result.ToString()));
                mediaPlayer?.OpenMedia(new MediaPath(getUrl(result.ToString()),
MediaPathType.AbsolutePathOrURL), autoPlay: true);
            }
            AppContext.instance.game.ShowLoading(false);
        }, error =>
        {
            loadingContent = false;
            if(gameObject.activeInHierarchy)
            AppContext.instance.game.ShowError("Video Load Error!");
            AppContext.instance.game.ShowLoading(false);
        });
        mediaPlayer.Events.AddListener(HandleEvent);
    }

    private void LoadImage(string url)
    {
        Destroy(mediaPlayer);
        if (!gameObject.activeInHierarchy) return;
            loadingContent = true;
        contentTypeisImage = true;
        //AppContext.instance.game.ShowLoading(true);
        cardHolder.gameObject.SetActive(true);
        videoContainer.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(Card.urlOrignal)){
            if (gameObject.activeInHierarchy)
             StartCoroutine(DownloadImageFromUrl(Card.urlOrignal));
            return;
        }
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
        string[] split = httpUrl.Split('=');
        //Debug.Log("Token = " + split[split.Length - 1]);
        if(split.Length > 1)
        {
            Texture2D savedTex = FileUtil.GetImage(split[split.Length -1]);
            if(savedTex != null)
            {
                SetTexture(savedTex);
                Debug.Log("Loading Image from local storage " + savedTex.width);
                yield break;
            }
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(httpUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            AppContext.instance.game.ShowError("Flash Card Load Error!");
            yield break;
        }
        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        Debug.Log(texture.width + "x" + texture.height);
        if (texture != null && split.Length > 1)
        {
            FileUtil.SaveImage(texture, split[split.Length - 1]);
        }
        SetTexture(texture);
    }

    private void SetTexture(Texture2D texture)
    {
        AppContext.instance.game.ShowLoading(false);
        loadingContent = false;
        cardHolder.texture = texture;
        //cardHolder.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width,texture.height), Vector2.zero);
    }
    // This method is called whenever there is an event from the MediaPlayer
    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        // Debug.Log("MediaPlayer " + mp.name + " generated event: " + eventType.ToString());
        if (eventType == MediaPlayerEvent.EventType.Error)
        {
            //Debug.LogError("Error: " + code);
            loadingContent = false;
            AppContext.instance.game.ShowError("Video Load Error!");
        }
        /* else if (eventType == MediaPlayerEvent.EventType.FinishedBuffering)
         {
             loadingContent = false;
         }
         else if (eventType == MediaPlayerEvent.EventType.ReadyToPlay)
         {
             loadingContent = false;
         }*/
        else if (eventType == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            loadingContent = false;
        } else if(eventType == MediaPlayerEvent.EventType.Stalled)
        {
            loadingContent = true;
        }else if(eventType == MediaPlayerEvent.EventType.Unstalled)
        {
            loadingContent = false;
        }
    }
}
