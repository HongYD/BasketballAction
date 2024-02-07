using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public const float Jog = 2.0f;
    public const float Nomal = 3.0f;
}

public struct PlayerSpeedDeadZone
{
    public const float JogDeadZone = 0.1f;
    public const float NormalDeadZone = 0.3f;
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
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Rececive, OnPlayerReceive);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Pass, OnPlayerPass);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Shoot, OnPlayerShoot);
    }

    private void OnPlayerShoot(object[] param)
    {
        state = PlayerState.Shoot;
        playerAnimator.SetBool("ShootBall", true);
    }

    private void OnPlayerPass(object[] param)
    {

    }

    private void OnPlayerReceive(object[] param)
    {
        state = PlayerState.Rececive;
        playerAnimator.SetTrigger("ReceiveBall");
    }

    private void OnPlayerMoveCancle(object[] param)
    {
        moveDir = Vector2.zero;
        playerAnimator.SetFloat("MoveX", moveDir.x);
        playerAnimator.SetFloat("MoveY", moveDir.y);
        playerAnimator.SetBool("IsJogWithBall", false);
    }

    private void OnPlayerMove(object[] param)
    {
        state = PlayerState.Move;
        moveDir = (Vector2)param[0];
        playerAnimator.SetBool("IsJogWithBall", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(moveDir.x) > PlayerSpeedDeadZone.JogDeadZone)
        {
            playerSpeed = PlayerSpeedLevel.Jog;
        }
        else if (Mathf.Abs(moveDir.y) > PlayerSpeedDeadZone.NormalDeadZone)
        {
            playerSpeed = PlayerSpeedLevel.Nomal;
        }
        else
        {
            playerSpeed = PlayerSpeedLevel.Idle;
        }
        Vector3 movement = new Vector3(moveDir.x, 0.0f, moveDir.y) * playerSpeed * Time.deltaTime;
        playerAnimator.SetFloat("MoveX", moveDir.x);
        playerAnimator.SetFloat("MoveY", moveDir.y);
        transform.Translate(movement, Space.World);
    }
}