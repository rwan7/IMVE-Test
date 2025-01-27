using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 100;
    
    public Transform spawnAreaCenter; // The center of the spawn area
    public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f); // The range (width, height, depth) of the spawn area

    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    void Start()
    {
        // Initialize the pool with enemies
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); // Start as inactive
            enemyPool.Enqueue(enemy);
        }

        // Start spawning enemies every 5 seconds
        StartCoroutine(SpawnEnemy());
    }

    // Get an enemy from the pool or expand the pool if empty
    private GameObject Activate()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            // If pool is empty, instantiate a new enemy
            GameObject enemy = Instantiate(enemyPrefab);
            return enemy;
        }
    }

    // Deactivate enemy and return it to the pool
    private void Deactivate(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    // Spawn an enemy every 5 seconds within the spawn area
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds

            GameObject enemy = Activate();

            // Get random position within the spawn area
            Vector3 randomPosition = GetRandomPositionInArea();
            enemy.transform.position = randomPosition; // Set the random position within the spawn area
            enemy.transform.rotation = Quaternion.identity; // Reset rotation or add randomized rotation if needed

            // Optional: Additional logic or spawn effects could go here (animations, etc.)
        }
    }

    // Get a random position within the defined spawn area
    private Vector3 GetRandomPositionInArea()
    {
        float randomX = Random.Range(spawnAreaCenter.position.x - spawnAreaSize.x / 2, spawnAreaCenter.position.x + spawnAreaSize.x / 2);
        float randomY = spawnAreaCenter.position.y; // You can adjust this if you want to vary Y as well
        float randomZ = Random.Range(spawnAreaCenter.position.z - spawnAreaSize.z / 2, spawnAreaCenter.position.z + spawnAreaSize.z / 2);

        return new Vector3(randomX, randomY, randomZ);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
