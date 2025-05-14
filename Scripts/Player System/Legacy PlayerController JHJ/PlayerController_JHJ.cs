using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

// 요구사항
[RequireComponent(typeof(CharacterController))]

public class PlayerController_JHJ : MonoBehaviour
{
    
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        IsGrounded();
        
        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            Debug.Log("점프함!");
            playerVelocity.y += Mathf.Sqrt(jumpHeight * 3.0f * gravityValue);
        }

        GetGravity();
    }

    // 땅에 닿아있으면 y 벨로시티 1로 고정
    void IsGrounded()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0f)
        {
            playerVelocity.y = 0f;
        }
    }

    
    // 중력 적용
    void GetGravity()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}