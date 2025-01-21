using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Firebase;
using CBS;
public class FirebaseDatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    string userId;
    private string firebaseBaseUrl = "gs://app-and-web-firebase.appspot.com/";
    // Start is called before the first frame update
    void Start()
    {
        AppContext.instance.DB = this;
        InitFirebase();
       // LoadQuiz(0);
    }
    void InitFirebase()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void LoadUserData(string userId, Action<ProfileData> result, Action<string> error)
    {
        
        reference.Child("Users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            this.userId = userId;
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception.Message);
                error?.Invoke(task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                Debug.Log(json);
                try
                {
                    ProfileData data = null;
                    if (!string.IsNullOrEmpty(json))
                    {
                        //Debug.Log("Serializing...");
                        data = JsonConvert.DeserializeObject<ProfileData>(json);
                    }
                    else {
                        if (!CBSModule.Get<CBSAuthModule>().isAdmin && data == null)
                        {
                            
                            UserLesson lesson = new UserLesson() { hasClearedLesson = false, hasClearedQuiz = false, hasWatchVideo = false };
                            data = new ProfileData() { UserLessons = new List<UserLesson> { lesson } };
                            UpdateLesson(0, lesson);
                            
                        }else
                        data = new ProfileData() { UserLessons = new List<UserLesson>() }; 
                    }
                        result?.Invoke(data);
                    
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    error?.Invoke(ex.Message);
                }

            }
        });
    }
    public void LoadQuiz(int lesson,Action<List<QuizData>> result, Action<string> error)
    {
        reference.Child("Quiz").Child(lesson.ToString()).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception.Message);
                error?.Invoke(task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                Debug.Log(json);
                try
                {
                    Dictionary<string,List<QuizData>> data = JsonConvert.DeserializeObject<Dictionary<string, List<QuizData>>>(json);
                    //if (data == null || data.Count == 0) Debug.Log("Data count is zero");
                    //quizUI.SetupQuiz(data.ElementAt(0).Value);
                    result?.Invoke(data.ElementAt(0).Value);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    error?.Invoke(ex.Message);
                }
               
            }
        });
    }
    public void LoadLessons(Action<List<LessonData>> result, Action<string> error)
    {
        reference.Child("Lesson").GetValueAsync().ContinueWithOnMainThread(task => {
        if (task.IsFaulted)
        {
            Debug.Log(task.Exception.Message);
            error?.Invoke(task.Exception.Message);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            //Debug.Log(json);
            try
            {
                List<Dictionary<string, LessonData>> data = JsonConvert.DeserializeObject<List<Dictionary<string, LessonData>>>(json);

                //if (data == null || data.Count == 0) Debug.Log("Data count is zero");
                //quizUI.SetupQuiz(data.ElementAt(0).Value);
                List<LessonData> lessons = new List<LessonData>();
                    data.ForEach(lData =>
                    {
                        //Debug.Log(lData.ElementAt(0).Value.videoUrl);
                        if(lData.Count>0)
                        lessons.Add(lData.ElementAt(0).Value);
                    });
                    result?.Invoke(lessons);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    error?.Invoke(ex.Message);
                }

            }
        });
    }
    public void UpdateLesson (int lesson,UserLesson data)
    {
        //Dictionary<string,bool> data = new Dictionary<string,bool>();
        //data.Add("isUnlocked", true);
        if (string.IsNullOrEmpty(CBSModule.Get<CBSAuthModule>().userId) || CBSModule.Get<CBSAuthModule>().isAdmin) return;
            reference.Child("Users").Child(userId).Child("UserLessons").Child(""+lesson).SetRawJsonValueAsync(JsonConvert.SerializeObject(data));
    }

    public void LoadFileUrl(string filePath, Action<Uri> result, Action<string> error)
    {
        //string firebaseVideoUrl = firebaseBaseUrl + firebaseVideoPath;
        // Initialize Firebase storage reference
        //Debug.Log("loading File URL of " + filePath);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception.Message);
                error?.Invoke("Failed to initialize Firebase: " + task.Exception.Message);
                return;
            }

            // Create Firebase Storage reference
            Firebase.Storage.StorageReference storageReference = Firebase.Storage.FirebaseStorage.DefaultInstance.GetReferenceFromUrl(filePath);

           // Debug.Log("Getting streaming url");
            // Get download URL for streaming
            storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(downloadTask =>
            {
                if (downloadTask.Exception != null)
                {
                    Debug.LogError("Failed to get download URL: " + downloadTask.Exception.Message);
                    error?.Invoke($"Failed to get download URL: {downloadTask.Exception.Message}");
                    return;
                }
                //Debug.Log(downloadTask.Result.ToString());
                result?.Invoke(downloadTask.Result);
                // Play video using VideoPlayer
                //PlayVideo(downloadTask.Result);
            });
        });
    }

    internal void LoadFlashCards(int lesson, Action<FlashCardDataCollection> result,Action<string> error)
    {
        reference.Child("FlashCard").Child(lesson.ToString()).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception.Message);
                error?.Invoke(task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
               // Debug.Log(json);
                try
                {
                    if (string.IsNullOrEmpty(json))
                    {
                        FlashCardDataCollection flashCardDataCollection = new FlashCardDataCollection();
                        flashCardDataCollection.flashcards = new List<FlashCardData>();
                        result?.Invoke(flashCardDataCollection);
                    }
                    Dictionary<string,List<FlashCardData>> data = JsonConvert.DeserializeObject<Dictionary<string, List<FlashCardData>>>(json);
                    //if (data == null || data.Count == 0) Debug.Log("Data count is zero");
                    //quizUI.SetupQuiz(data.ElementAt(0).Value);
                    FlashCardDataCollection flashCardDataCollection2 = new FlashCardDataCollection();
                    flashCardDataCollection2.flashcards = data.ElementAt(0).Value;
                    result?.Invoke(flashCardDataCollection2);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    error?.Invoke(ex.Message);
                }

            }
        });
    }
}

