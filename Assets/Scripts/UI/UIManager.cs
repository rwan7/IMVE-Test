using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI enemyCounterText; 
    public TextMeshProUGUI survivedTimeText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameStartText;

    [Header("UI List")] 
    public GameObject pauseUI;
    public GameObject settingsUI;
    public GameObject inGameUI; 
    public GameObject gameOverUI;
    public GameObject gameStartUI;

    [Header("Audio Toggles")]
    public Toggle soundToggle;
    public Toggle musicToggle;

    private CanvasGroup gameStartCanvasGroup;
    private CanvasGroup gameOverCanvasGroup;
    public float timeSurvived = 0f;
    public bool isAlive = true;

    public static UIManager Instance { get; private set; } 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverText != null)
        {
            gameOverCanvasGroup = gameOverText.GetComponent<CanvasGroup>();
            gameOverCanvasGroup.alpha = 0f; 
            gameOverCanvasGroup.gameObject.SetActive(false);
            StartCoroutine(FadeInText()); 
        }

        if (gameStartText != null)
        {
            gameStartCanvasGroup = gameStartText.GetComponent<CanvasGroup>();
            gameStartCanvasGroup.alpha = 1f; 
            StartCoroutine(FadeOutText());
        }

        soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        soundToggle.onValueChanged.AddListener(ToggleSound);
        musicToggle.onValueChanged.AddListener(ToggleMusic);
    }

    void Update()
    {
        if (isAlive)
        {
            timeSurvived += Time.deltaTime;
            UpdateSurvivedTime(timeSurvived);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
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
    
    public void UpdateEnemyCounter(int count)
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies : " + count;
        }
    }

    private void UpdateSurvivedTime(float time)
    {
        if (survivedTimeText != null)
        {
            timeSurvived = time;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            survivedTimeText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
    }

    public void PauseGame()
    {
        AudioManager.Instance.PlayPausedSFX();
        
        CursorUnlock();

        inGameUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        AudioManager.Instance.PlayClickSFX();
        
        CursorLock();

        inGameUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        AudioManager.Instance.PlayClickSFX();
        pauseUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void BackButton()
    {
        AudioManager.Instance.PlayClickSFX();
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void BackToMenuButton()
    {
        AudioManager.Instance.PlayClickSFX();

        CursorUnlock();
        
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    
    public void RestartButton()
    {
        AudioManager.Instance.PlayClickSFX();

        CursorUnlock();

        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void GameOver()
    {
        AudioManager.Instance.PlayGameOverSFX();
        isAlive = false;
        EnemyPool.Instance.StopSpawning();

        CursorUnlock();
        
        if (gameOverText != null)
        {
            inGameUI.SetActive(false);
            gameOverUI.SetActive(true);
            StartCoroutine(FadeInText());
        }
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

    private IEnumerator FadeInText()
    {
        float duration = 2f; 
        float elapsedTime = 0f;

        gameOverCanvasGroup.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            gameOverText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        gameOverCanvasGroup.alpha = 1f; 
    }

    private IEnumerator FadeOutText()
    {
        float duration = 2f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            gameStartText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }
        gameStartCanvasGroup.alpha = 0f; 
        gameStartUI.SetActive(false);
    }

}
