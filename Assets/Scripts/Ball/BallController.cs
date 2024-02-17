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
        ballState = BallState.Animate;
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
    }

    private void OnPlayerShoot(object[] param)
    {
        rb.isKinematic = true;
        ballAnimator.SetBool("ShootBall", true);
    }

    private void OnPlayerPass(object[] param)
    {
        
    }

    private void OnPlayerReceive(object[] param)
    {
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
        ballAnimator.SetBool("IsJog", true);
        ballMoveDir = (Vector2)param[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(ballState == BallState.FreeFly)
        {
            rb.isKinematic = false;
        }
    }

    private void FixedUpdate()
    {
        ballAnimator.SetFloat("BallMoveX", ballMoveDir.x);
        ballAnimator.SetFloat("BallMoveY", ballMoveDir.y);
        if (curIndex < trajectory.Count - 1)
        {

            flyBall.transform.position = trajectory[curIndex];
            curIndex++;
        }
        else
        {
            ballState = BallState.FreeFly;
            trajectory.Clear();
        }
    }

    public void OnBallShooting()
    {
        flyBall.SetActive(true);
        visualizedBall.SetActive(false);
        flyBall.transform.position = boneBall.transform.position;
        trajectory.Clear();
        trajectory = BallTrajactoryManager.CalculateBallTrajactory(flyBall.transform.position, ballFlyTargetF.transform.position);
    }
}
