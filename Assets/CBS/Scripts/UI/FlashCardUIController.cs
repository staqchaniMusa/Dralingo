using CBS.Scriptable;
using CBS.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlashCardUIController : MonoBehaviour
{
    [SerializeField] private FlasCard FlasCard;
    int currentCard = -1;
    int currentLesson = 0;
    int prevLesson = -1;
    private FlashCardDataCollection flashCards;

    [SerializeField] SimpleScrollSnap SimpleScrollSnap;
    [SerializeField] GameObject loadingImg;
    [SerializeField] TextMeshProUGUI pageTxt;
    [SerializeField] GameObject fullScreenUI;
    private FlasCard currentFlashCard;
    private VideoController videoController;
    private bool init;
    private void Update()
    {
        if(videoController == null)
        {
            loadingImg.SetActive(false);
            return;
        }
        if (!videoController.isActiveAndEnabled) {
            loadingImg.SetActive(currentFlashCard.loadingContent);
            return;
        } 
        loadingImg.SetActive(currentFlashCard.loadingContent || (!currentFlashCard.contentTypeisImage && videoController.IsBuffering));
    }
    public void InitFlashCards(FlashCardDataCollection cards, int lesson)
    {
        this.flashCards = cards;
        this.currentLesson = lesson;
        currentCard = -1;
        if (currentLesson == prevLesson) return;
        StartCoroutine(InitialzeScroller());
    }

    IEnumerator ClearContent()
    {
        while(SimpleScrollSnap.NumberOfPanels > 0)
        {
            SimpleScrollSnap.RemoveFromBack();
        }
        yield return new WaitForEndOfFrame();
        /*for (int i = 0; i< SimpleScrollSnap.Content.childCount; i++)
        {
            DestroyImmediate(SimpleScrollSnap.Content.GetChild(i).gameObject);
        }*/
    }
    IEnumerator  InitialzeScroller()
    {
        init = true;
        yield return StartCoroutine(ClearContent());
        prevLesson = currentLesson;
        List<FlashCardData> imageCards = flashCards.flashcards.FindAll(card => !card.isVideo);
        List<FlashCardData> videoCards = flashCards.flashcards.FindAll(card => card.isVideo);
        List<FlashCardData> cards = new List<FlashCardData>();
        int len = imageCards.Count > videoCards.Count ? imageCards.Count : videoCards.Count;
        for (int i = 0; i < len; i++)
        {
            if (imageCards.Count > i) cards.Add(imageCards[i]);
            if(videoCards.Count > i) cards.Add(videoCards[i]);
        }
        flashCards.flashcards = cards;

        for (int i = 0; i < flashCards.flashcards.Count; i++)
        {
            var card = flashCards.flashcards[i];
            var newPanel = SimpleScrollSnap.AddToFront(FlasCard.gameObject);
            newPanel?.GetComponent<FlasCard>().SetCardData(card,OnLoadContent,(texture)=>FullScreen(texture));
            newPanel?.gameObject.SetActive(false);
        }
        //LoadContent(0);
        LoadFlashCardsUrl(0);
       Invoke(nameof(StartFlashCard),0.1f);
    }

    void LoadFlashCardsUrl(int index) {
        if (!gameObject.activeInHierarchy) return;
        if(index >= flashCards.flashcards.Count) return;
        if (flashCards.flashcards[index].isVideo)
        {
            
            LoadFlashCardsUrl(++index);
            return;
        }
        FlashCardData card = flashCards.flashcards[index];
        AppContext.instance.DB.LoadFileUrl(card.url, result =>
        {
            flashCards.flashcards[index].urlOrignal = result.ToString();
            //Debug.Log("Download File Url : " + result.ToString());
            LoadFlashCardsUrl(++index);
        }, error =>
        {
            LoadFlashCardsUrl(++index);
        });
    }
    private void OnLoadContent(FlasCard card)
    {
        currentFlashCard?.StopVideo();
        ClearRenderTexture();
        currentFlashCard = card.GetComponent<FlasCard>();
        //Invoke(nameof(LoadNextFlashCard),0.1f);
        videoController = currentFlashCard?.videoController;
        int selectedPanel = card.transform.GetSiblingIndex();
        int total = SimpleScrollSnap.NumberOfPanels;

        pageTxt.text = string.Format("{0}/{1}",total - selectedPanel, total);
        
    }

    int previousIndex = -1;
    void StartFlashCard()
    {
        SimpleScrollSnap.GoToPanel(SimpleScrollSnap.NumberOfPanels - 1);
    }
    public void LoadContent(int index)
    {
        return;
        if(previousIndex == index) return;
        currentFlashCard?.StopVideo();
        ClearRenderTexture();
        var card = SimpleScrollSnap.GetPanel(index);
        currentFlashCard = card.GetComponent<FlasCard>();
        //Invoke(nameof(LoadNextFlashCard),0.1f);
        videoController = currentFlashCard?.videoController;
        Debug.Log("Loading card " + index);
        previousIndex = index;
    }

    void LoadNextFlashCard()
    {
        
        //currentFlashCard.LoadContent();
    }

    public RenderTexture videoRenderTexture;
    Color clearColor = Color.black;
    void ClearRenderTexture()
    {
        // Set the active render texture
        RenderTexture.active = videoRenderTexture;

        // Clear the render texture with the specified clear color
        GL.Clear(true, true, clearColor);

        // Reset the active render texture
        RenderTexture.active = null;
    }

    public void FullScreen(Texture texture)
    {
        fullScreenUI.GetComponentInChildren<RawImage>().texture = texture;
        fullScreenUI.SetActive(true);
    }
    int barValue;
    public void OnScrollValueChanges(float val)
    {
        Debug.Log("Bar Value : " + val);
        barValue = (int)val;
    }
   public void GotoPage(Scrollbar bar)
    {
        int current = (int)bar.value;
        Debug.Log("Current Value: " + current);
        SimpleScrollSnap.GoToPanel(barValue);
    }
    public void Back()
    {
        //AppContext.instance.game.ShowLessonDetail(currentLesson);
        var ui = CBSScriptable.Get<CommonPrefabs>().FlashCardUI;
        currentFlashCard?.StopVideo();
        UIView.HideWindow(ui);
    }
}
