using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CBS;
public class SettingUI : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;

    private string musicPrefs = "music";    
    // Start is called before the first frame update
    void Start()
    {
        musicToggle.isOn = PlayerPrefs.GetInt(musicPrefs, 1) == 1;
    }

    

    public void ToggleMusic(bool isOn)
    {
        SoundsManager.instance.PlayClick();
        PlayerPrefs.SetInt(musicPrefs, isOn ? 1 : 0);
    }

    public void ResetPassword()
    {
        CBSModule.Get<CBSAuthModule>().ForgotPassword();
    }

    public void Logout()
    {
        CBSModule.Get<CBSAuthModule>().OnLogoutEvent += OnLogout;
        CBSModule.Get<CBSAuthModule>().Logout();
    }

    public void Back()
    {
        AppContext.instance.game.ShowGameContext();
    }
    public void OnLogout()
    {
        CBSModule.Get<CBSAuthModule>().OnLogoutEvent -= OnLogout;
        SceneManager.LoadScene(0);
    }
}
