using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallState
{
    Animate,
    ShootFly,
    PassFly,
    ReceiveFly,
    FreeFly,
}

public class BallController : MonoBehaviour
{
    [SerializeField]
    private BallState ballState;
    [SerializeField]
    private Vector2 ballMoveDir;
    private Animator ballAnimator;
    private GameObject flyBall;

    // Start is called before the first frame update
    void Start()
    {
        ballState = BallState.Animate;
        ballAnimator = GetComponent<Animator>();
        flyBall = GameObject.Find("basketball");

        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Move, OnPlayerMove);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.MoveCancled, OnPlayerMoveCancle);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Rececive, OnPlayerReceive);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Pass, OnPlayerPass);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Shoot, OnPlayerShoot);
    }

    private void OnPlayerShoot(object[] param)
    {
        ballAnimator.SetBool("ShootBall", true);
    }

    private void OnPlayerPass(object[] param)
    {
        
    }

    private void OnPlayerReceive(object[] param)
    {
        ballAnimator.SetTrigger("ReceiveBall");
    }

    private void OnPlayerMoveCancle(object[] param)
    {
        ballMoveDir = Vector2.zero;
        ballAnimator.SetFloat("BallMoveX", ballMoveDir.x);
        ballAnimator.SetFloat("BallMoveY", ballMoveDir.y);
        ballAnimator.SetBool("IsJogWithBall", false);
    }

    private void OnPlayerMove(object[] param)
    {
        ballAnimator.SetBool("IsJogWithBall", true);
        ballMoveDir = (Vector2)param[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ballAnimator.SetFloat("BallMoveX", ballMoveDir.x);
        ballAnimator.SetFloat("BallMoveY", ballMoveDir.y);
    }
}
