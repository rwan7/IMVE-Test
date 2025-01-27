using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("UI List")] 
    public GameObject settingsUI;
    public GameObject MainMenuUI;

    [Header("Audio Toggles")]
    public Toggle soundToggle;
    public Toggle musicToggle;

    [Header("UI Elements")]
    public TextMeshProUGUI highestTimeText; 

    private SaveManager saveManager;

    void Start()
    {
        saveManager = new SaveManager();

        soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        soundToggle.onValueChanged.AddListener(ToggleSound);
        musicToggle.onValueChanged.AddListener(ToggleMusic);

        HighestScore();
    }

    public void ToggleSound(bool isOn)
    {
        AudioManager.Instance.SetSoundEnabled(isOn);
        PlayerPrefs.SetInt("SoundEnabled", isOn ? 1 : 0);
    }

    public void ToggleMusic(bool isOn)
    {
        AudioManager.Instance.SetMusicEnabled(isOn);
        PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
    }

    public void PlayGame()
    {
        AudioManager.Instance.PlayClickSFX();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

        CursorLock();
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlayClickSFX();
        Application.Quit();
    }

    public void OpenSettings()
    {
        AudioManager.Instance.PlayClickSFX();
        settingsUI.SetActive(true);
        MainMenuUI.SetActive(false);
    }

    public void BackButton()
    {
        AudioManager.Instance.PlayClickSFX();
        settingsUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }

    private void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CursorUnlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HighestScore()
    {
        float highestTime = saveManager.LoadHighestTimeSurvived();
        highestTimeText.text = "Highest Time Survived : " + highestTime.ToString("F2") + " seconds";
    }

}
