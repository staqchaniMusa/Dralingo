using CBS;
using CBS.Scriptable;
using System;
using System.Linq;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;
using CBS.UI;
using UnityEngine;

namespace CBS
{
    public class CBSAuthModule : CBSModule, IAuth
    {
        /// <summary>
        /// An event that reports a successful user login
        /// </summary>
        public event Action OnLoginEvent;

        /// <summary>
        /// An event that reports when the user logged out
        /// </summary>
        public event Action OnLogoutEvent;

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        public bool IsLogiledIn =>false;

        private AuthData AuthData { get; set; }

        const string appUrl = "https://dralingo.com/";

        protected Firebase.Auth.FirebaseAuth auth;

        public bool isAdmin;
        public string userId;
        public ProfileData ProfileData { get; private set; }
        protected override void Init()
        {
            AuthData = CBSScriptable.Get<AuthData>();
            InitializeFirebase();
        }
        // Handle initialization of the necessary firebase modules:
        protected void InitializeFirebase()
        {
            //Debug.Log("Setting up Firebase Auth");
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
           
        }

       

        /// <summary>
        /// Authorization using login and password. No automatic registration. Before login, you need to register
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        public async void LoginWithMailAndPassword(string email, string password)
        {
            Credential credential = EmailAuthProvider.GetCredential(email, password);
            try
            {
                await auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        DisplayError("Login Canceled");

                        return;
                    }
                    //AuthResult result = task.Result;

                    if (task.IsFaulted)
                    {
                        DisplayError("Someting went wrong, try again later");
                        return;
                    }
                    isAdmin = email.ToLower() == "admin@test.com" && password.ToLower() == "admin1234";
                    userId = task.Result.User.UserId;
                    LoginPostProcess();
                });
            } catch (Exception e)
            {
                DisplayError("User not found");
            }
        }
      
        public void RegisterAccount()
        {
            Application.OpenURL(appUrl);
        }

        public void ForgotPassword()
        {
            Application.OpenURL(appUrl);
        }
        void DisplayError(string error)
        {
            
            new PopupViewer().ShowSimplePopup(new PopupRequest()
            {
                Title = "Error",
                Body = "Login Failed"
            });
            new PopupViewer().HideLoadingPopup();
        }
        /// <summary>
        /// Sign Out. Stops running and executing CBS scripts. Clears all cached information.
        /// </summary>
        /// <param name="result"></param>
        public void Logout()
        {
            
            LogoutProcces();
          
        }

        protected override void OnLogout()
        {
            OnLogoutEvent?.Invoke();
        }
        // other tools
        private void LoginPostProcess()
        {
           
            OnLoginEvent?.Invoke();
        }

        public void FreeTrial()
        {
            LoginPostProcess();
        }
    }
}
