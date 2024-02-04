using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputActionsAsset playerMovement;
    
    Vector2 move;
    public float speed = 10;

    private void Awake()
    {
        playerMovement = new InputActionsAsset();
        playerMovement.PlayerControls.Move.performed += OnMove;
        playerMovement.PlayerControls.Move.canceled += context=>move = Vector2.zero;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        Debug.Log($"按下方向键{move}" + context);
    }

    private void OnEnable()
    {
        playerMovement.PlayerControls.Enable();

    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(move.x, 0.0f, move.y) * speed * Time.deltaTime;
        transform.Translate(movement,Space.World);
    }

    private void OnDisable()
    {
        playerMovement.PlayerControls.Disable();
    }
}