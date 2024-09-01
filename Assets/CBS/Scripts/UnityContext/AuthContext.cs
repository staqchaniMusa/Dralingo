
using CBS.Scriptable;
using CBS.UI;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace CBS.Context
{
    public class AuthContext : MonoBehaviour
    {
        [SerializeField]
        private string mainScene;
        [SerializeField] private AudioMixer mixer;
        private AuthPrefabs AuthUIData { get; set; }
        private LoginForm LoginForm { get; set; }
        private AuthData AuthData { get; set; }
        private IAuth Auth { get; set; }

        private void Start()
        {
            AuthUIData = CBSScriptable.Get<AuthPrefabs>();
            AuthData = CBSScriptable.Get<AuthData>();
            Auth = CBSModule.Get<CBSAuthModule>();
            
            Init();
        }
        string videoLocation = "Lessons/Lesson1/Lesson 1.mp4";
        private void Init()
        {
           
            string url = "https://firebasestorage.googleapis.com/v0/b/app-and-web-firebase.appspot.com/o/" + System.Uri.EscapeDataString(videoLocation);
            //Debug.Log(url);
            // show background
            var backgroundPrefab = AuthUIData.Background;
            UIView.ShowWindow(backgroundPrefab);
            ShowLoginScreen();
        }

        private void ShowLoginScreen()
        {
            // show login screen
            var loginPrefab = AuthUIData.LoginForm;
           // Debug.Log(loginPrefab.name);
            var loginWindow = UIView.ShowWindow(loginPrefab);
            LoginForm = loginWindow.GetComponent<LoginForm>();
            // subscribe to success login
            LoginForm.OnLogined += OnLoginComplete;
        }

        private void OnLoginComplete()
        {
            
            LoginForm.OnLogined -= OnLoginComplete;
            SceneManager.LoadScene(mainScene);
            /*if (result.IsSuccess)
            {
                if (LoginForm != null)
                {
                    LoginForm.OnLogined -= OnLoginComplete;
                }
                SceneManager.LoadScene(LobbyScene);
            }*/
        }
    }
}
