using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize enemy settings if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Implement enemy behavior here (e.g., patrolling, chasing the player)
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy took damage. Current health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}