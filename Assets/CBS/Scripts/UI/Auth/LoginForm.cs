
using CBS.Scriptable;
using CBS.Utils;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CBS.UI
{
    public class LoginForm : MonoBehaviour
    {
        [SerializeField]
        private InputField MailInput;
        [SerializeField]
        private InputField PasswordInput;
        [SerializeField]
        private InputField CustomIDInput;

        private IAuth Auth { get; set; }
        private AuthPrefabs AuthUIData { get; set; }

        public Action OnLogined;
        private void Start()
        {
            Auth = CBSModule.Get<CBSAuthModule>();
            AuthUIData = CBSScriptable.Get<AuthPrefabs>();
            LoginFromCache();
        }

        private void OnDisable()
        {
            Clear();
        }

        public void OnRegisterAccount()
        {
            SoundsManager.instance.PlayClick();
            Auth.RegisterAccount();
        }

        private void LoginFromCache()
        {
            string email = PlayerPrefs.GetString("email");
            if ((email) != "")
            {
                new PopupViewer().ShowLoadingPopup();
                string password = PlayerPrefs.GetString("password");
                Auth.OnLoginEvent += OnUserLogined;
                MailInput.text = email;
                PasswordInput.text = password;
                //Debug.Log($"Logging in with email {MailInput.text} and password {PasswordInput.text}");
                Auth.LoginWithMailAndPassword(email, password);
            }
        }
        // button click
        public void OnLoginWithMail()
        {
            SoundsManager.instance.PlayClick();
            if (InputValid())
            {
               
                // login request
                new PopupViewer().ShowLoadingPopup();
                Auth.OnLoginEvent += OnUserLogined;
                //Debug.Log($"Logging in with email {MailInput.text} and password {PasswordInput.text}");
                Auth.LoginWithMailAndPassword(MailInput.text, PasswordInput.text);
                //LoginWithMailAndPassword(MailInput.text, PasswordInput.text);
            }
            else
            {
                // show error message
                new PopupViewer().ShowSimplePopup(new PopupRequest
                {
                    Title = AuthTXTHandler.ErrorTitle,
                    Body = AuthTXTHandler.InvalidInput
                });
            }
        }


        public void OnFreeTrial()
        {
            Auth.OnLoginEvent += OnUserLogined;
            Auth.FreeTrial();
        }
        private void OnUserLogined()
        {
            new PopupViewer().HideLoadingPopup();
            OnLogined?.Invoke();
            Auth.OnLoginEvent -= OnUserLogined;
        }

        public void OnFogotPassword()
        {
            SoundsManager.instance.PlayClick();
            Auth.ForgotPassword();
        }

        // check if inputs is not null
        private bool InputValid()
        {
            bool mailValid = !string.IsNullOrEmpty(MailInput.text);
            bool passwordValid = !string.IsNullOrEmpty(PasswordInput.text);
            return mailValid && passwordValid;
        }

        // reset view
        private void Clear()
        {
            MailInput.text = string.Empty;
            PasswordInput.text = string.Empty;
        }

        private void HideWindow()
        {
            gameObject.SetActive(false);
        }
    }
}
