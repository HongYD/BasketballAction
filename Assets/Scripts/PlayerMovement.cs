using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    InputActionsAsset playerMovement;
    
    Vector2 move;
    public float speed = 5;

    Animator playerAnimator;
    Animator ballAnimator;


    private void Awake()
    {
        playerAnimator = this.GetComponent<Animator>();
        ballAnimator = this.transform.Find("Ball").GetComponent<Animator>();
        playerMovement = new InputActionsAsset();
        playerMovement.PlayerControls.Move.performed += OnMove;
        playerMovement.PlayerControls.Move.canceled += OnMoveCanceled;
        playerMovement.PlayerControls.Shoot.performed += OnShoot;
        playerMovement.PlayerControls.Pass.performed += OnPass;
        playerMovement.PlayerControls.Receive.performed += OnReceive;
    }

    private void OnReceive(InputAction.CallbackContext context)
    {
        playerAnimator.SetTrigger("ReceiveBall");
        ballAnimator.SetTrigger("ReceiveBall");
        Debug.Log($"按下接球键K" + context);
    }

    private void OnPass(InputAction.CallbackContext context)
    {
        Debug.Log($"按下传球键K" + context);
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        playerAnimator.SetBool("ShootBall",true);
        ballAnimator.SetBool("ShootBall", true);
        Debug.Log($"按下投篮键J" + context);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        move = Vector2.zero;
        playerAnimator.SetFloat("MoveX", move.x);
        playerAnimator.SetFloat("MoveY", move.y);
        playerAnimator.SetBool("IsJogWithBall", false);

        ballAnimator.SetFloat("BallMoveX", move.x);
        ballAnimator.SetFloat("BallMoveY", move.y);
        ballAnimator.SetBool("IsJogWithBall", false);

        Debug.Log($"<color=red>取消按下方向键{move}</color>" + context);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        playerAnimator.SetBool("IsJogWithBall",true);
        ballAnimator.SetBool("IsJogWithBall", true);
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
        playerAnimator.SetFloat("MoveX", move.x);
        playerAnimator.SetFloat("MoveY", move.y);

        ballAnimator.SetFloat("BallMoveX", move.x);
        ballAnimator.SetFloat("BallMoveY", move.y);
        transform.Translate(movement,Space.World);
    }

    private void OnDisable()
    {
        playerMovement.PlayerControls.Disable();
    }
}