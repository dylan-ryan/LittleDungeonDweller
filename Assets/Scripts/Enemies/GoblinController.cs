using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GoblinController : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Enemy Sprites")]
    public Sprite defaultSprite;
    public Sprite attackSprite;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Enemy States")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private float chaseRange = 10f;

    [Header("Enemy Attack Values")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float projectileSpeed = 5f;

    [Header("Coin Enemy Drops")]
    public GameObject coin;

    private HealthSystem healthSystem;
    private NavMeshAgent agent;
    private Transform player;
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
                Idle();
                anim.SetBool("Walking", false);
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
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            waveManager.OnEnemyDeath(this.gameObject);
            Vector3 enemyLocation = transform.position;
            Destroy(gameObject);
            RaycastHit hit;
            Vector3 spawnPosition = enemyLocation;

            if (Physics.Raycast(enemyLocation + Vector3.up * 10, Vector3.down, out hit, 20f))
            {
                spawnPosition = hit.point;
            }

            Instantiate(coin, spawnPosition, Quaternion.identity);

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

    private void Idle()
    {
        vx = Vector2.zero;
        agent.SetDestination(transform.position);
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
            FireArrow();
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
            firstStrike = true;
            SoundManager.Instance.PlaySFX(2);
        }
        if (attackTimer <= 0f)
        {
            StartCoroutine(PlayAttackSprite());
            FireArrow();
            Debug.Log("Enemy attacks the player!");
            attackTimer = attackCooldown;
            SoundManager.Instance.PlaySFX(2);

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

    private void FireArrow()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject arrow = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = (player.position - firePoint.position).normalized;

            ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
            if (arrowScript != null)
            {
                arrowScript.Initialize(direction);
            }

            Destroy(arrow, 5f);
        }
    }

}
