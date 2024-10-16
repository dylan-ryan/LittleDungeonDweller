using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }

    [Header("Enemy States")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float chaseRange = 10f;

    [Header("Enemy Attack Values")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 1;

    [Header("Coin Enemy Drops")]
    public GameObject coin;

    private HealthSystem healthSystem;
    private NavMeshAgent agent;
    private Transform player;
    private int currentPatrolIndex;
    private float attackTimer;
    private bool firstStrike;

    private void Start()
    {
        
        healthSystem = gameObject.GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrol;
        attackTimer = attackCooldown;
        firstStrike = false;
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

        if (healthSystem.health <= 0)
        {
            Vector3 enemyLocation = transform.position;
            Destroy(gameObject);
            Instantiate(coin, enemyLocation, Quaternion.identity);
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
        if(firstStrike == false)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
            firstStrike = true;
        }
        if (attackTimer <= 0f)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
        }
    }
}
