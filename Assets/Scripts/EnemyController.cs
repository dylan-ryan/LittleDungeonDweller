using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }
    [SerializeField] private EnemyState currentState;

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private int currentPatrolIndex;
    private float attackTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrol;
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (currentState == EnemyState.Patrol && distanceToPlayer <= chaseRange)
        {
            currentState = EnemyState.Chase;
        }
        else if (currentState == EnemyState.Chase && distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (currentState == EnemyState.Attack && distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
        }
        else if (currentState == EnemyState.Chase && distanceToPlayer > chaseRange)
        {
            currentState = EnemyState.Patrol;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
        }
    }
}
