using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public int poolSize = 100;
    
    public Transform spawnAreaCenter; 
    public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);
    public float minSpawnDistance = 3f;
    private Vector3 nextSpawnPosition;
    private bool isSpawning = false;

    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    private int activeEnemyCount = 0;  

    public static EnemyPool Instance { get; private set; } 

    void Awake()
    {
        Instance = this; 
    }

    void Update()
    {
        if (!isSpawning && UIManager.Instance.isAlive) 
        {
            StartCoroutine(SpawnEnemy());
            isSpawning = true;
        }
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); 
            enemyPool.Enqueue(enemy);
        }

        UIManager.Instance.UpdateEnemyCounter(activeEnemyCount);
    }

    private GameObject Activate()
    {
        GameObject enemy;
        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Dequeue();
        }
        else
        {
            enemy = Instantiate(enemyPrefab);
        }
        
        enemy.SetActive(true);
        activeEnemyCount++;  
        UIManager.Instance.UpdateEnemyCounter(activeEnemyCount);

        return enemy;
    }

    public void Deactivate(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
        activeEnemyCount--;  
        UIManager.Instance.UpdateEnemyCounter(activeEnemyCount);
    }

    private IEnumerator SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomPositionInArea();

        if (Vector3.Distance(spawnPos, player.position) >= minSpawnDistance)
        {
            GameObject enemy = Activate();
            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.identity;
        }
            
        yield return new WaitForSeconds(5f);
        isSpawning = false;
    }

    internal void StopSpawning()
    {
        isSpawning = false;
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 randomPosition;
        int maxAttempts = 10;  
        int attempts = 0;

        do
        {
            float randomX = Random.Range(spawnAreaCenter.position.x - spawnAreaSize.x / 2, spawnAreaCenter.position.x + spawnAreaSize.x / 2);
            float randomY = spawnAreaCenter.position.y; 
            float randomZ = Random.Range(spawnAreaCenter.position.z - spawnAreaSize.z / 2, spawnAreaCenter.position.z + spawnAreaSize.z / 2);

            randomPosition = new Vector3(randomX, randomY, randomZ);
            attempts++;
        }
        while (Vector3.Distance(randomPosition, player.position) < minSpawnDistance && attempts < maxAttempts);

        return randomPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter.position, spawnAreaSize);
    }
}
