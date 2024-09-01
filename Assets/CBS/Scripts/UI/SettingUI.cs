using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CBS;
using UnityEngine.Audio;
public class SettingUI : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;

    private string musicPrefs = "music";

    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        musicToggle.isOn = PlayerPrefs.GetInt(musicPrefs, 1) == 1;
        
    }

    

    public void ToggleMusic(bool isOn)
    {
        Debug.Log(isOn);
        audioMixer.SetFloat("Music", isOn ? 0f : -80f);
        SoundsManager.instance.PlayClick();
        PlayerPrefs.SetInt(musicPrefs, isOn ? 1 : 0);
    }

    public void ResetPassword()
    {
        CBSModule.Get<CBSAuthModule>().ForgotPassword();
    }

    public void Logout()
    {
        SoundsManager.instance.PlayClick();
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
