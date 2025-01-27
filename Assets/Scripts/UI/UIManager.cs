using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI enemyCounterText; 
    public TextMeshProUGUI survivedTimeText; 
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
    }


    public void UpdateEnemyCounter(int count)
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies: " + count;
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

    public void StopGame()
    {
        isAlive = false;
        EnemyPool.Instance.StopSpawning();
    }
}
