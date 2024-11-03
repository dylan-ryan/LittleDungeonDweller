using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Enemy Sprites")]
    public Sprite defaultSprite;
    public Sprite attackSprite;

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
    private Vector2 vx;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
                anim.SetBool("Walking", true);
                break;
            case EnemyState.Chase:
                Chase();
                anim.SetBool("Walking", true);
                break;
            case EnemyState.Attack:
                Attack();
                anim.SetBool("Walking", false);
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

    void LateUpdate()
    {
        if (vx.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (vx.x < 0)
        {
            spriteRenderer.flipX = true;
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

        vx = agent.velocity.normalized;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);

        vx = agent.velocity.normalized;
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        attackTimer -= Time.deltaTime;
        if (firstStrike == false)
        {
            StartCoroutine(PlayAttackSprite());
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
            firstStrike = true;
        }
        if (attackTimer <= 0f)
        {
            StartCoroutine(PlayAttackSprite());
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
        }
    }

    private IEnumerator PlayAttackSprite()
    {
        anim.enabled = false;

        spriteRenderer.sprite = attackSprite;

        yield return new WaitForSeconds(0.5f);

        spriteRenderer.sprite = defaultSprite;
        anim.enabled = true;
    }

}
