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

    Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        playerMovement = new InputActionsAsset();
        playerMovement.PlayerControls.Move.performed += OnMove;
        playerMovement.PlayerControls.Move.canceled += OnMoveCanceled;
        playerMovement.PlayerControls.Shoot.performed += OnShoot;
        playerMovement.PlayerControls.Pass.performed += OnPass;
    }

    private void OnPass(InputAction.CallbackContext context)
    {
        Debug.Log($"按下传球键K" + context);
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log($"按下投篮键J" + context);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        move = Vector2.zero;
        Debug.Log($"<color=red>取消按下方向键{move}</color>" + context);
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
        animator.SetFloat("ForwardLeft45", move.x);
        Vector3 movement = new Vector3(move.x, 0.0f, move.y) * speed * Time.deltaTime;
        transform.Translate(movement,Space.World);
    }

    private void OnDisable()
    {
        playerMovement.PlayerControls.Disable();
    }
}