using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OgreController : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Enemy Sprites")]
    public Sprite defaultSprite;
    public Sprite attackSprite;

    [Header("Enemy States")]
    [SerializeField] private EnemyState currentState;
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
    private float attackTimer;
    private bool firstStrike;
    private Vector2 vx;
    private bool isAttacking;

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
        isAttacking = false;
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
            isAttacking = false;
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

        if (!isAttacking && Vector3.Distance(player.position, transform.position) <= attackRange)
        {
            isAttacking = true;
            StartCoroutine(DelayedAttack());
        }
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(1.5f);

        if (Vector3.Distance(player.position, transform.position) <= attackRange)
        {
            StartCoroutine(PlayAttackSprite());
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Ogre attacks the player!");
            attackTimer = attackCooldown;
            SoundManager.Instance.PlaySFX(3);

        }
        else
        {
            Debug.Log("Ogre attack canceled because player moved out of range");
        }

        isAttacking = false;
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
