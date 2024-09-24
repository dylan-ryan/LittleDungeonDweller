using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContoller : MonoBehaviour
{
    private CharacterController characterController;
    public int attackDamage = 1;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float swordRadius = 1.5f;
    [SerializeField] private float swordRange = 2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Vector3 velocity;
    private Vector3 lastMoveDirection;
    private Vector3 gizmoSwipeCenter;
    private float gizmoSwipeRadius;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (move != Vector3.zero)
        {
            lastMoveDirection = move.normalized;
        }

        characterController.Move(move * (Time.fixedDeltaTime * _moveSpeed));

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
