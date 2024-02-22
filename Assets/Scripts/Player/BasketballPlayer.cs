using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum PlayerState
{
    Idle=0,
    Move,
    Shoot,
    Pass,
    Rececive,
    PickUp,
    LayUp,
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

public struct PlayerAnimationData
{
    public const int shootEvent = 16;
    public const int layUpLength = 90;
    public const float playerHeight = 2.0f;
    public const float playerLayUpAnimMaxHeight = 0.42f;
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
    private float angleSpeed;
    [SerializeField]
    private float JumpAngleSpeed;
    [SerializeField]
    private Vector2 moveDir;
    [SerializeField]
    private Vector2 faceDir;
    [SerializeField]
    private GameObject hoopF;
    [SerializeField]
    private GameObject hoopB;

    private float shootAngleDiff;

    private Vector3 layUpPosDiff;
    private float layUpJumpDist;
    private float layUpSpeed;

    [SerializeField]
    private Rig rig;
    [SerializeField]
    private Transform rightHandTarget;
    [SerializeField]
    private Transform rightHandTransform;
    [Range(0f, 1f)]
    private float targetWeight;
    [SerializeField]
    private GameObject flyBall;
    [SerializeField]
    private float shootAbility;
    [SerializeField]
    private float hoopPlayerDist;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.Idle;
        playerSpeed = PlayerSpeedLevel.Idle;
        playerAnimator = this.GetComponent<Animator>();
        rig = this.GetComponentInChildren<Rig>();
        targetWeight = 0;

        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Move, OnPlayerMove);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.MoveCancled, OnPlayerMoveCancle);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Rececive, OnPlayerReceive);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Pass, OnPlayerPass);
        EventManager<PlayerInputEvent>.instance.AddListener(PlayerInputEvent.Shoot, OnPlayerShoot);
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.PickUpBallEvent, OnPickUpBall);
    }

    private void OnPickUpBall(object[] param)
    {
        string name = (string)param[0];
        if (name == gameObject.name)
        {
            state = PlayerState.PickUp;
            playerAnimator.SetTrigger("PickUpBall");
        }
    }

    private void OnPlayerShoot(object[] param)
    {
        state = PlayerState.Shoot;
        hoopPlayerDist = Vector2.Distance(transform.position.ToVector2(), hoopF.transform.position.ToVector2());
        if (hoopPlayerDist < HallDataStruct.LayUpDist)
        {
            state = PlayerState.LayUp;
            playerAnimator.SetTrigger("LayUp");
            EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.ShootBallEvent, ShootPoseType.LayUp);
        }
        else
        {
            playerAnimator.SetTrigger("ShootBall");
            EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.ShootBallEvent, ShootPoseType.NormalShoot);
        }
        EventManager<PlayerAbilityEvent>.instance.TriggerEvent(PlayerAbilityEvent.ShootAbility, shootAbility);
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
        playerAnimator.SetBool("IsJog", false);
    }

    private void OnPlayerMove(object[] param)
    {
        state = PlayerState.Move;
        moveDir = (Vector2)param[0];
        if (moveDir != Vector2.zero)
        {
            faceDir = moveDir;
        }
        playerAnimator.SetBool("IsJog", true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Move:
                Move();
                break;
            case PlayerState.Shoot:
                Shoot();
                break;
            case PlayerState.LayUp:
                LayUp();
                break;
            case PlayerState.Pass:
                Pass();
                break;
            case PlayerState.Rececive:
                Rececive();
                break;
        }
    }

    void FixedUpdate()
    {

    }

    private void Idle()
    {

    }

    private void Move()
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
        RotatePlayer();
    }

    private void Shoot()
    {
        Vector2 hookDir = (hoopF.transform.position - transform.position).ToVector2().normalized;
        float rotY = Quaternion.LookRotation(new Vector3(hookDir.x, 0, hookDir.y), transform.up).eulerAngles.y;
        float curY = this.transform.rotation.eulerAngles.y;
        shootAngleDiff = rotY - curY;
        StartCoroutine(FixPlayerTransformOnShoot());
        state = PlayerState.Idle;
    }

    private void LayUp()
    {
        Vector2 hookDir = (hoopF.transform.position - transform.position).ToVector2().normalized;
        float rotY = Quaternion.LookRotation(new Vector3(hookDir.x, 0, hookDir.y), transform.up).eulerAngles.y;
        float curY = this.transform.rotation.eulerAngles.y;
        shootAngleDiff = rotY - curY;
        StartCoroutine(FixPlayerTransformOnShoot());
        flyBall.transform.position = Vector3.zero;
        flyBall.transform.SetParent(rightHandTransform,true);

        float heightDiff = HallDataStruct.HoopWireHeight - PlayerAnimationData.playerHeight - PlayerAnimationData.playerLayUpAnimMaxHeight;
        Vector3 hoopUpVector3 = hoopF.transform.position - new Vector3(hoopF.transform.position.x, heightDiff, hoopF.transform.position.z);
        Vector3 playerForwardVector3 = new Vector3(hoopF.transform.position.x, 0, hoopF.transform.position.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
        layUpPosDiff = hoopUpVector3 + playerForwardVector3;
        layUpJumpDist = layUpPosDiff.magnitude;
        layUpSpeed = layUpJumpDist / PlayerAnimationData.layUpLength;
        StartCoroutine(FixPlayerPosOnLayUp());
        state = PlayerState.Idle;
    }

    private void Pass()
    {

    }

    private void Rececive()
    {
        
    }

    private void RotatePlayer()
    {
        Vector3 lookDir = new Vector3(faceDir.x, 0, faceDir.y);
        Quaternion rot = Quaternion.LookRotation(lookDir, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, angleSpeed * Time.deltaTime);
    }

    private IEnumerator FixPlayerPosOnLayUp()
    {
        while (layUpJumpDist > 0)
        {
            Vector3 dir = layUpPosDiff.normalized;
            Vector3 movement = new Vector3(dir.x, dir.y, dir.z) * layUpSpeed*Time.deltaTime;
            this.transform.Translate(movement);
            layUpJumpDist -= layUpSpeed;
            yield return null;
        }
        StopCoroutine(FixPlayerPosOnLayUp());
    }

    private IEnumerator FixPlayerTransformOnShoot()
    {
        float absDiff = Mathf.Abs(shootAngleDiff);
        if (absDiff > 180.0f)
        {
            if (shootAngleDiff > 0)
            {
                shootAngleDiff = (shootAngleDiff-360.0f);
                absDiff = Mathf.Abs(shootAngleDiff);
            }
            else
            {
                shootAngleDiff = (360.0f - Mathf.Abs(shootAngleDiff));
                absDiff = Mathf.Abs(shootAngleDiff);
            }
        }
        while (absDiff > 0)
        {
            float rotSpeed = shootAngleDiff / (float)PlayerAnimationData.shootEvent;
            transform.Rotate(new Vector3(0, rotSpeed, 0));
            absDiff -= Mathf.Abs(rotSpeed);
            yield return null;
        }
        shootAngleDiff = 0;
        StopCoroutine(FixPlayerTransformOnShoot());
    }

    public void OnPickUpEvent()
    {
        targetWeight = 1f;
        rig.weight = targetWeight;
        flyBall.transform.position = rightHandTransform.position;
        flyBall.transform.SetParent(rightHandTransform, true);
    }

    public void OnPickUpEndEvent()
    {
        EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.PickUpBallEndEvent);
    }

    public void OnLayUpShoot()
    {
        EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.LayUpShootEvent);
    }
}
