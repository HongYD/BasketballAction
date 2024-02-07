using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle=0,
    Move,
    Shoot,
    Pass,
    Rececive,
}

public struct PlayerSpeedLevel
{
    public const float Idle = 0;
    public const float Jog = 1.0f;
    public const float Nomal = 3.0f;
}

public class BasketballPlayer : PlayerAgent
{
    [SerializeField]
    private PlayerState state;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private Vector2 moveDir;


    // Start is called before the first frame update

    void Start()
    {
        state = PlayerState.Idle;
        playerSpeed = PlayerSpeedLevel.Idle;
        playerAnimator = this.GetComponent<Animator>();

        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Move, OnPlayerMove);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.MoveCancled, OnPlayerMoveCancle);
    }

    private void OnPlayerMoveCancle(object[] param)
    {
        InputAction.CallbackContext context = (InputAction.CallbackContext)param[0];
        moveDir = Vector2.zero;
        playerAnimator.SetFloat("MoveX", moveDir.x);
        playerAnimator.SetFloat("MoveY", moveDir.y);
        playerAnimator.SetBool("IsJogWithBall", false);
    }

    private void OnPlayerMove(object[] param)
    {
        state = PlayerState.Move;
        InputAction.CallbackContext context = (InputAction.CallbackContext)param[0];
        playerAnimator.SetBool("IsJogWithBall", true);
        moveDir = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
