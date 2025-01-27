using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float detectionRange = 10f;
    public float chaseSpeed = 3.5f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isChasing = false;
    private bool hasGrowled = false; // for growl sound effect

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
       float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                if (!hasGrowled)
                {
                    AudioManager.Instance.PlayEnemyGrowlSFX();
                    hasGrowled = true; // Ensure growl sound is only played once
                }
            }
        }
        else
        {
            isChasing = false;
            hasGrowled = false;
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
            
            if (!AudioManager.Instance.SFXSource.isPlaying || 
                AudioManager.Instance.SFXSource.clip != AudioManager.Instance.enemyWalkSound)
            {
                AudioManager.Instance.PlayEnemyWalkSFX();
            }
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }    
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player playerScript))
            {
                playerScript.Die(); 
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);  
    }
}

