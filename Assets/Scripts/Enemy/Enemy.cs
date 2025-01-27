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
    private bool isDead = false;

    private enum EnemyState {Idle, Chasing, Attacking, Dying}
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
        agent.stoppingDistance = attackRange;
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
    }

    private void CheckState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distance <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Idle;
        }   
    }

    private void Idle()
    {
        animator.SetBool("IsIdle", true);
        animator.SetBool("IsWalking", false);   
        CheckState();
    }

    private void Chase()
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsIdle", false);
        agent.SetDestination(player.position);

        CheckState();
    }

    private void Attack()
    {
        if (canAttack)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("IsWalking", false);

            StartCoroutine(PerformAttack());
        }    
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;

        if (player.TryGetComponent(out Player playerScript))
        {
            playerScript.TakeDamage(damage);
        }

        currentState = EnemyState.Idle;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        agent.isStopped = false;

        CheckState();
    }   

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        Debug.Log("Enemy took damage. Current health: " + health);

        if (health <= 0)
        { 
            Die(); 
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = EnemyState.Dying;
        animator.SetTrigger("Die");
        agent.isStopped = true;
        Debug.Log("Enemy died.");

        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }   
}
