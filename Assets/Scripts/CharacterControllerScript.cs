using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterControllerScript : MonoBehaviour
{
    private CharacterController characterController;
    [HideInInspector] public Renderer objRenderer;
    public bool controlsEnabled;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Arrow UI Element")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Camera mainCamera;

    [Header("Player Movement")]
    [SerializeField] public float moveSpeed = 10f;

    [Header("Player Attack Values")]
    [SerializeField] public float swordRadius = 1.5f;
    [SerializeField] public float swordRange = 2f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite swordSwipe;
    [SerializeField] private Sprite defaultSprite; 

    [Header("Unity Layer Enemies Are On")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("UI Cooldown Indicator")]
    [SerializeField] private Image cooldownImage;

    private float gravity = -10f;
    private float groundCheckDistance = 0.1f;
    private Vector3 velocity;
    private Vector3 lastMoveDirection;
    private Vector3 gizmoSwipeCenter;
    private float gizmoSwipeRadius;
    private float cooldownTimer = 0f;

    public static CharacterControllerScript manager;
    private HealthSystem healthSystem;

    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        objRenderer = gameObject.GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        if (controlsEnabled)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (move != Vector3.zero)
            {
                anim.SetBool("Walking", true);
                lastMoveDirection = move.normalized;

                if (move.x > 0)
                    spriteRenderer.flipX = true;
                else if (move.x < 0)
                    spriteRenderer.flipX = false;
            }
            else
            {
                anim.SetBool("Walking", false);
            }

            characterController.Move(move * (Time.fixedDeltaTime * moveSpeed));

            if (characterController.isGrounded)
            {
                velocity.y = -groundCheckDistance;
            }
            else
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }
            characterController.Move(velocity * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (controlsEnabled && healthSystem.enabled == true)
        {
            cooldownTimer -= Time.deltaTime;

            cooldownImage.fillAmount = cooldownTimer / attackCooldown;

            if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0)
            {
                Attack();
                cooldownTimer = attackCooldown;
                SoundManager.Instance.PlaySFX(0);
            }
        }

        if (controlsEnabled && healthSystem.enabled == true)
        {
            UpdateArrow();
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = cooldownTimer / attackCooldown;

            if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0)
            {
                Attack();
                cooldownTimer = attackCooldown;
            }
        }
    }

    public void Attack()
    {
        anim.enabled = false;

        spriteRenderer.sprite = attackSprite;

        Invoke("ResetSprite", 0.5f);

        Vector3 swipeCenter = transform.position + lastMoveDirection * swordRange;
        gizmoSwipeCenter = swipeCenter;
        gizmoSwipeRadius = swordRadius;

        Debug.DrawRay(transform.position, lastMoveDirection * swordRange, Color.red, 2f);
        Debug.DrawLine(transform.position, swipeCenter, Color.blue, 2f);

        InstantiateAttackSprite(swipeCenter);

        Collider[] hitColliders = Physics.OverlapSphere(swipeCenter, swordRadius, enemyLayer);

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                Debug.Log("Hit " + hitCollider.name);
                HealthSystem enemyHealth = hitCollider.GetComponent<HealthSystem>();
                enemyHealth.TakeDamage(attackDamage);
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }

    private void InstantiateAttackSprite(Vector3 position)
    {
        GameObject attackVisual = new GameObject("AttackSprite");
        SpriteRenderer attackRenderer = attackVisual.AddComponent<SpriteRenderer>();
        attackRenderer.sprite = swordSwipe;
        attackRenderer.sortingOrder = 1;

        attackVisual.transform.position = position;
        float angle = Mathf.Atan2(lastMoveDirection.z, lastMoveDirection.x) * Mathf.Rad2Deg;
        attackVisual.transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(attackVisual, 0.2f);
    }



    private void ResetSprite()
    {
        spriteRenderer.sprite = defaultSprite;
        anim.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoSwipeCenter, gizmoSwipeRadius);
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }

    private void UpdateArrow()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 100f, enemyLayer);

        if (enemies.Length == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        Collider nearestEnemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
        Vector3 enemyPosition = nearestEnemy.transform.position;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(enemyPosition);
        bool isOnScreen = viewportPoint.z > 0 &&
                          viewportPoint.x > 0 && viewportPoint.x < 1 &&
                          viewportPoint.y > 0 && viewportPoint.y < 1;

        if (isOnScreen)
        {
            arrow.gameObject.SetActive(false);
        }
        else
        {
            arrow.gameObject.SetActive(true);

            Vector3 direction = (enemyPosition - transform.position).normalized;
            float radius = 4f;
            Vector3 circlePosition = transform.position + direction * radius;

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(circlePosition);

            arrow.position = screenPosition;

            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0, 0, angle);
        }
    }



}
