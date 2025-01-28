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
    private Player playerScript;


    private bool isChasing = false;
    private bool isPlayingWalkSFX = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (playerScript == null || playerScript.enabled == false) // to check if player is dead
        {
            StopChasing(); 
            return;
        }

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        float detectionRangeSqr = detectionRange * detectionRange;

        if (sqrDistance <= detectionRangeSqr)
        {
            if (!isChasing)
            {
                isChasing = true;
                agent.speed = chaseSpeed;
            }

            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("IsWalking", true);

        if (!isPlayingWalkSFX)
        {
            Debug.Log("Playing enemy walk sound");
            AudioManager.Instance.PlayEnemyWalkSFX();
            isPlayingWalkSFX = true;
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        animator.SetBool("IsWalking", false);

        if (isPlayingWalkSFX)
        {
            Debug.Log("Stopping enemy walk sound");
            AudioManager.Instance.StopLoopingSFX();
            isPlayingWalkSFX = false;
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
