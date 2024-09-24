using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterContoller : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float swordRadius = 1.5f;
    [SerializeField] private float swordRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

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
