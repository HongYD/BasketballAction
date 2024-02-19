using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BallState
{
    MoveFly=0,
    ShootFly,
    PassFly,
    ReceiveFly,
    FreeFly,
    PickUp,
}

//这个球完全受动画控制
public class BallController : MonoBehaviour
{
    [SerializeField]
    private BallState ballState;
    [SerializeField]
    private Vector2 ballMoveDir;
    private Animator ballAnimator;
    [SerializeField]
    private GameObject flyBall;
    [SerializeField]
    private GameObject ballFlyTargetF;
    [SerializeField]
    private GameObject ballFlyTargetB;
    [SerializeField]
    private List<Vector3> trajectory;
    [SerializeField]
    GameObject visualizedBall;
    [SerializeField]
    GameObject boneBall;
    private int curIndex;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        ballState = BallState.MoveFly;
        visualizedBall = transform.GetChild(0).gameObject;
        boneBall = transform.GetChild(1).gameObject;
        rb = flyBall.GetComponent<Rigidbody>();
        ballAnimator = GetComponent<Animator>();
        flyBall = GameObject.Find("basketball");
        curIndex = 0;
        flyBall.SetActive(false);

        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Move, OnPlayerMove);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.MoveCancled, OnPlayerMoveCancle);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Rececive, OnPlayerReceive);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Pass, OnPlayerPass);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Shoot, OnPlayerShoot);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.PickUpBallEvent, OnPickUpBall);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.PickUpBallEndEvent, OnPickUpBallEnd);
    }

    private void OnPickUpBallEnd(object[] param)
    {
        flyBall.SetActive(false);
        visualizedBall.SetActive(true);
    }

    private void OnPickUpBall(object[] param)
    {
        ballState = BallState.MoveFly;
        rb.isKinematic = true;
        ballAnimator.SetTrigger("PickUpBall");
    }

    private void OnPlayerShoot(object[] param)
    {
        rb.isKinematic = true;
        ballAnimator.SetTrigger("ShootBall");
        flyBall.transform.parent = null;
    }

    private void OnPlayerPass(object[] param)
    {
        ballState = BallState.PassFly;
    }

    private void OnPlayerReceive(object[] param)
    {
        ballState = BallState.ReceiveFly;
        flyBall.SetActive(false);
        visualizedBall.SetActive(true);
        ballAnimator.SetTrigger("ReceiveBall");
    }

    private void OnPlayerMoveCancle(object[] param)
    {
        ballMoveDir = Vector2.zero;
        ballAnimator.SetFloat("BallMoveX", ballMoveDir.x);
        ballAnimator.SetFloat("BallMoveY", ballMoveDir.y);
        ballAnimator.SetBool("IsJog", false);
    }

    private void OnPlayerMove(object[] param)
    {
        if (ballState != BallState.ShootFly)
        {
            ballState = BallState.MoveFly;
        }
        ballAnimator.SetBool("IsJog", true);
        ballMoveDir = (Vector2)param[0];
    }

    // Update is called once per frame
    void Update()
    {
        switch (ballState)
        {
            case BallState.MoveFly:
                Animate();
                break;
            case BallState.ShootFly:
                ShootFly();
                break;
            case BallState.PassFly:
                break;
            case BallState.ReceiveFly:
                ReceiveFly();
                break;
            case BallState.FreeFly:
                FreeFly();
                break;
        }
    }

    private void Animate()
    {
        ballAnimator.SetFloat("BallMoveX", ballMoveDir.x);
        ballAnimator.SetFloat("BallMoveY", ballMoveDir.y);
    }

    private void ShootFly()
    {
        if (curIndex < trajectory.Count - 1)
        {

            flyBall.transform.position = trajectory[curIndex];
            curIndex++;
        }
        else
        {
            ballState = BallState.FreeFly;
            curIndex = 0;
            trajectory.Clear();
        }
    }

    private void FreeFly()
    {
        if (ballState == BallState.FreeFly)
        {
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }
            EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.isNeedPickUpEvent);
        }
    }

    private void ReceiveFly()
    {
        ballState = BallState.MoveFly;
    }

    private void FixedUpdate()
    {

    }

    public void OnBallShooting()
    {
        ballState = BallState.ShootFly;
        flyBall.SetActive(true);
        visualizedBall.SetActive(false);
        flyBall.transform.position = boneBall.transform.position;
        trajectory.Clear();
        trajectory = BallTrajactoryManager.CalculateBallTrajactory(flyBall.transform.position, ballFlyTargetF.transform.position);
    }
}
