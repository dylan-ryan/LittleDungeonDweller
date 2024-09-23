using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterContoller : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private float _moveSpeed = 10f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    private void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        characterController.Move(move * (Time.deltaTime * _moveSpeed));
    }
}
