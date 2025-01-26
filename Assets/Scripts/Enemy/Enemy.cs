using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 100f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    private enum EnemyState { Idle, Chasing, Attacking, Dying }
    private EnemyState currentState = EnemyState.Idle;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canAttack = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
            case EnemyState.Dying:
                break;
        }

        if (health <= 0 && currentState != EnemyState.Dying)
        {
            Die();
        }
    }

    private void Idle()
    {
        animator.SetBool("IsWalking", false);
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else if (distance <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    private void Chase()
    {
        animator.SetBool("IsWalking", true);
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distance > detectionRange)
        {
            currentState = EnemyState.Idle;
        }
    }

    private void Attack()
    {
        if (canAttack)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("IsWalking", false); // Ensure the enemy stays idle during attack
            agent.SetDestination(transform.position);  // Stop moving

            StartCoroutine(PerformAttack());
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange) // Only transition to chasing if the player is out of attack range
        {
            currentState = EnemyState.Chasing;
        }
        else if (distance <= attackRange && currentState != EnemyState.Attacking) // Remain idle if still in attack range
        {
            currentState = EnemyState.Attacking; // Stay in attacking state
        }
    }


    private IEnumerator PerformAttack()
    {
        canAttack = false;
        Debug.Log("Enemy attacks player!");

        if (player.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
        }

        currentState = EnemyState.Idle;  
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
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
        currentState = EnemyState.Dying;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        Debug.Log("Enemy died.");
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }   
}
