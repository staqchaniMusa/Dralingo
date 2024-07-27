using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CBS.UI;
using System.Linq;
public class QuizUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionTxt;
    [SerializeField] private TextMeshProUGUI answerTxt;
    [SerializeField] private TextMeshProUGUI resultTxt;
    [SerializeField] private ToggleGroup answerChoices;
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private Text[] choices;
    [SerializeField] private GameObject SingleAnswer, ChoiceAnswer,ResultUI;
    private List<QuizData> data;
    int currentQuestion = -1;
    private bool isSingleChoice;
    private int currentQuiz;
    bool allDone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupQuiz(List<QuizData> data,int currentQuiz = 0)
    {
        this.data = data;
        currentQuestion = 0;
        this.currentQuiz = currentQuiz;
        Debug.Log(data.Count);
        if(data.Count > 0) 
        NextQuestion();
    }

    void NextQuestion()
    {
        answerField.text = "";
        var question = data[currentQuestion];
        questionTxt.text = question.question;
        if (question.answers.Count > 1)
        {
            SingleAnswer.SetActive(false);
            ChoiceAnswer.SetActive(true);
            answerChoices.SetAllTogglesOff();
            for (global::System.Int32 i = 0; i < choices.Length; i++)
            {
                answerChoices.transform.GetChild(i).gameObject.SetActive(i < question.answers.Count);
                if(i < question.answers.Count)
                {
                    choices[i].text = question.answers[i];
                }
            }
            isSingleChoice = false;
        }
        else
        {
            SingleAnswer.SetActive (true);
            ChoiceAnswer.SetActive (false);
            isSingleChoice = true;

        }
    }

    public void SubmitAnser()
    {
        if (allDone)
        {
            AppContext.instance.game.BackToLesson();
            SoundsManager.instance.PlayClick(2);
            return;
        }
        if(isSingleChoice)
        {
            
            if (string.IsNullOrEmpty(answerField.text.Trim()))
            {
                new PopupViewer().ShowSimplePopup(new PopupRequest()
                {
                    Title = "Error",
                    Body = "Please write your answer."
                });
                return;
            }
            data[currentQuestion].answerByUser = answerField.text.Trim();
        }
        else
        {
            if (!answerChoices.AnyTogglesOn())
            {
                new PopupViewer().ShowSimplePopup(new PopupRequest()
                {
                    Title = "Error",
                    Body = "Please Tick any choice befor submitting your answer."
                });
                return;
            }
           // var choice = answerChoices.GetComponentsInChildren<Toggle>().Where(t => t.isOn).FirstOrDefault();
            var choice = answerChoices.ActiveToggles().FirstOrDefault();
            if (choice != null && choice.TryGetComponent(out ChoceUI choiceUI))
            {
                data[currentQuestion].answerByUser = choiceUI.choiceTxt.text.Trim();
                Debug.Log(data[currentQuestion].answerByUser);
            }
            else
            {
                Debug.LogError("Choices are empty.");
            }
        }

        currentQuestion++;
        if(currentQuestion>=data.Count)
        {
            currentQuestion = -1;
            CheckResult();
        }else {
            SoundsManager.instance.PlayClick(8);
            NextQuestion();
        }
    }

    void CheckResult()
    {
        int correctAnser = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].answer.ToLower() == data[i].answerByUser.ToLower())
            {
                correctAnser++;
            }
        }
        
        float percent = 100 * correctAnser/ data.Count;
        string remarks = $"You've got {correctAnser} / {data.Count} marks.";
        questionTxt.text = $"Lesson No {currentQuiz + 1} Completed!";
        resultTxt.text = remarks;
        allDone = true;
        if(percent> 80)
        {
            SoundsManager.instance.PlayClick(9);
            if (AppContext.instance.game.profile != null && AppContext.instance.game.profile.UserLessons != null && currentQuiz < AppContext.instance.game.profile.UserLessons.Count )
            {
                UserLesson lessson = AppContext.instance.game.profile.UserLessons[currentQuiz];
                lessson.hasClearedQuiz = true;
                lessson.hasClearedLesson = true;
                AppContext.instance.DB.UpdateLesson(currentQuiz, lessson);
                //AppContext.instance.game.profile.UserLessons.Add(lessson);
                
            }
            new PopupViewer().ShowRewardPopup(new PopupRequest()
            {
                Title = "Congrats! You have passed the quiz!",
                Body = AppContext.instance.game.Lessons[currentQuiz].secretCode
            });
            /*Dictionary<string,bool> data = new Dictionary<string,bool>();
            data.Add("isUnlocked", true);
            data.Add("hasSecretKey", true);
            AppContext.instance.DB.UpdateLesson(currentQuiz, data);*/
        }else
        {
            SoundsManager.instance.PlayClick(10);
            
        }
        SingleAnswer.SetActive(false);
        ChoiceAnswer.SetActive(false);
        ResultUI.SetActive(true);
        /*new PopupViewer().ShowSimplePopup(new PopupRequest()
        {
            Title = "Result",
            Body = remarks,
            OnOkAction = () =>
            {
                AppContext.instance.game.ShowLessons();
            }
        }) ;*/
    }

    public void BackButtonClicked()
    {
        SoundsManager.instance.PlayClick(2);
        AppContext.instance.game.ShowLessonDetail(currentQuiz);
    }
}
