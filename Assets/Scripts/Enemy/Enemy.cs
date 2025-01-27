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
            isChasing = true;
            agent.speed = chaseSpeed;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
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

