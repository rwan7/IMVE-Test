using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI enemyCounterText; 
    public TextMeshProUGUI survivedTimeText; 
    public GameObject pauseUI;
    public GameObject settingsUI;
    public GameObject inGameUI;
    public float timeSurvived = 0f;
    public bool isAlive = true;

    public static UIManager Instance { get; private set; } 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inGameUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inGameUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        pauseUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void BackButton()
    {
        pauseUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void BackToMenuButton()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        isAlive = false;
        EnemyPool.Instance.StopSpawning();
    }
}
