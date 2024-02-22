using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallScoreType
{
    One = 1,
    Two,
    Three,
}

public enum BallState
{
    MoveFly=0,
    ShootFly,
    PassFly,
    ReceiveFly,
    FreeFly,
    PickUp,
    LayUp,
}

public enum BallHitResult
{
    Hit,
    Miss,
}

public enum ShootPoseType
{
    NormalShoot,
    SlamDunk,
    LayUp,
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
    private GameObject missTargetFA;
    [SerializeField]
    private GameObject missTargetFB;
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
    [SerializeField]
    private BallHitResult ballHitResult;
    [SerializeField]
    private float forceStrength;
    [SerializeField]
    private float forceUpOffset;
    [SerializeField]
    private float flyballDist;
    [SerializeField]
    private BallScoreType ballScoreType;
    [SerializeField]
    private Transform playerRightHandTransform;


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
        ballHitResult = BallHitResult.Miss;

        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Move, OnPlayerMove);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.MoveCancled, OnPlayerMoveCancle);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Rececive, OnPlayerReceive);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Pass, OnPlayerPass);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.PickUpBallEvent, OnPickUpBall);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.PickUpBallEndEvent, OnPickUpBallEnd);
        EventManager<PlayerAbilityEvent>.instance.AddListener(PlayerAbilityEvent.ShootAbility, OnPlayerShootHitDecide);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.ShootBallEvent, OnShoot);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.LayUpShootEvent, OnLayUpShoot);
    }

    private void OnShoot(object[] param)
    {
        ShootPoseType shootPoseType = (ShootPoseType)param[0];
        if(shootPoseType == ShootPoseType.NormalShoot)
        {
            rb.isKinematic = true;
            ballAnimator.SetTrigger("ShootBall");
            flyBall.transform.parent = null;
        }
        else if(shootPoseType == ShootPoseType.LayUp)
        {
            ballState = BallState.LayUp;
            ballAnimator.SetTrigger("LayUp");
        }
    }

    private void OnPlayerShootHitDecide(object[] param)
    {
        float playerShootAbility = (float)param[0];
        float hitProbability = RandomNumberGenerator.GenerateRandomNumber();
        if (hitProbability < playerShootAbility)
        {
            ballHitResult = BallHitResult.Hit;
            Debug.Log($"<color=blue>这球进了!</color>");
        }
        else
        {
            ballHitResult = BallHitResult.Miss;
            Debug.Log($"<color=red>这球没进!</color>");
        }
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
            case BallState.LayUp:
                LayUp();
                break;
        }
    }

    private void LayUp()
    {
        flyBall.SetActive(true);
        visualizedBall.SetActive(false);
        flyBall.transform.position = playerRightHandTransform.position;
        flyBall.transform.SetParent(playerRightHandTransform, true);
    }

    private void OnLayUpShoot(object[] param)
    {
        trajectory.Clear();
        ballHitResult = BallHitResult.Hit;
        float muzzleV = TrajectoryMuzzleV.CloseShootV;
        trajectory = BallTrajactoryManager.CalculateBallTrajactory(flyBall.transform.position, ballFlyTargetF.transform.position, muzzleV, true);
        flyBall.transform.parent = null;
        ballState = BallState.ShootFly;
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
            rb.isKinematic = false;
            if (ballHitResult == BallHitResult.Miss)
            {
                Vector3 dir = (trajectory[0] - trajectory[trajectory.Count - 1]).normalized;
                rb.AddForce((dir + new Vector3(0, forceUpOffset, 0)) * forceStrength, ForceMode.Impulse);
                SoundManager.PlaySound(SoundManager.SoundType.BounceBasket, rb.transform.position);
            }
            else
            {
                SoundManager.PlaySound(SoundManager.SoundType.BounceWire, rb.transform.position);
                if (flyballDist >= HallDataStruct.ThreePointLineDist)
                {
                    EventManager<GameEventEvent>.instance.TriggerEvent(GameEventEvent.OnScoreEvent, (int)BallScoreType.Three);
                }
                else
                {
                    EventManager<GameEventEvent>.instance.TriggerEvent(GameEventEvent.OnScoreEvent, (int)BallScoreType.Two);
                }
            }
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
        flyballDist = Vector2.Distance(this.transform.position.ToVector2(), ballFlyTargetF.transform.position.ToVector2());
        float muzzleV=0;
        if (flyballDist > 0 && flyballDist < HallDataStruct.LayUpDist)
        {
            muzzleV = TrajectoryMuzzleV.CloseShootV;
        }
        else if (flyballDist > HallDataStruct.LayUpDist && flyballDist < HallDataStruct.MiddleShootDist)
        {
            muzzleV = TrajectoryMuzzleV.MiddleShootV;
        }
        else if (flyballDist > HallDataStruct.MiddleShootDist)
        {
            muzzleV = TrajectoryMuzzleV.LongShootV;
        }
        Transform targetTrans = null;
        switch (ballHitResult)
        {
            case BallHitResult.Hit:
                targetTrans = ballFlyTargetF.transform;
                break;
            case BallHitResult.Miss:
                float ranNum = RandomNumberGenerator.GenerateRandomNumber();
                if(ranNum > 0.5f)
                {
                    targetTrans = missTargetFA.transform;
                }
                else
                {
                    targetTrans = missTargetFB.transform;
                }
                break;
        }
        trajectory = BallTrajactoryManager.CalculateBallTrajactory(flyBall.transform.position, targetTrans.transform.position, muzzleV,false);
    }

    public void OnBallAnimTouchGround()
    {
        SoundManager.PlaySound(SoundManager.SoundType.BounceFloor,this.transform.position);
    }
}
