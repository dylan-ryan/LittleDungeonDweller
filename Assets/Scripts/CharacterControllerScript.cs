using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
    private CharacterController characterController;
    [HideInInspector] public Renderer objRenderer;

    [Header("Player Movement")]
    [SerializeField] public float moveSpeed = 10f;

    [Header("Player Attack Values")]
    [SerializeField] public float swordRadius = 1.5f;
    [SerializeField] public float swordRange = 2f;
    public int attackDamage = 1;

    [Header("Unity Layer Enemies Are On")]
    [SerializeField] private LayerMask enemyLayer;
    
    private float gravity = -10f;
    private float groundCheckDistance = 0.1f;
    private Vector3 velocity;
    private Vector3 lastMoveDirection;
    private Vector3 gizmoSwipeCenter;
    private float gizmoSwipeRadius;

    public static CharacterControllerScript manager;
    void Awake()
    {

        //If GameManager doesnt exist set this as the manager and dont destruction on load
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        //If GameManager already exists destroy the dupe
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
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (move != Vector3.zero)
        {
            lastMoveDirection = move.normalized;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    public void Attack()
    {
        Vector3 swipeCenter = transform.position + lastMoveDirection * swordRange;

        gizmoSwipeCenter = swipeCenter;
        gizmoSwipeRadius = swordRadius;

        Debug.DrawRay(transform.position, lastMoveDirection * swordRange, Color.red, 2f);
        Debug.DrawLine(transform.position, swipeCenter, Color.blue, 2f);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoSwipeCenter, gizmoSwipeRadius);
    }
}
