using System;
using System.Collections;
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
    public const int pickUpEvent = 90;
    public const int layUpLength = 250;
    public const int layUpEventLength = 38;
    public const float playerHeight = 2.0f;
    public const float playerLayUpAnimMaxHeight = 0.42f;
    public const float playerHalfHeight = 1.0f;
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
    private float pickUpAngleDiff;

    private Vector2 layUpPosDiff;
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
            case PlayerState.PickUp:
                PickUp();
                break;
        }
    }

    void FixedUpdate()
    {

    }

    private void Idle()
    {

    }

    private void PickUp()
    {
        Vector2 flyBallDir = (flyBall.transform.position - transform.position).ToVector2().normalized;
        float rotY = Quaternion.LookRotation(new Vector3(flyBallDir.x, 0, flyBallDir.y), transform.up).eulerAngles.y;
        float curY = this.transform.rotation.eulerAngles.y;
        pickUpAngleDiff = rotY - curY;
        StartCoroutine(FixPlayerTransformOnPickUp());
        state = PlayerState.Move;
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

        layUpPosDiff = (hoopF.transform.position.ToVector2() - this.transform.position.ToVector2());
        layUpJumpDist = layUpPosDiff.magnitude;
        layUpSpeed = layUpJumpDist / (float)PlayerAnimationData.layUpLength;
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

    private IEnumerator FixPlayerTransformOnPickUp()
    {
        float absDiff = Mathf.Abs(pickUpAngleDiff);
        if (absDiff > 180.0f)
        {
            if (pickUpAngleDiff > 0)
            {
                pickUpAngleDiff = (pickUpAngleDiff - 360.0f);
                absDiff = Mathf.Abs(pickUpAngleDiff);
            }
            else
            {
                pickUpAngleDiff = (360.0f - Mathf.Abs(pickUpAngleDiff));
                absDiff = Mathf.Abs(pickUpAngleDiff);
            }
        }
        while (absDiff > 0)
        {
            float rotSpeed = pickUpAngleDiff / (float)PlayerAnimationData.pickUpEvent;
            transform.Rotate(new Vector3(0, rotSpeed, 0));
            absDiff -= Mathf.Abs(rotSpeed);
            yield return null;
        }
        pickUpAngleDiff = 0;
        StopCoroutine(FixPlayerTransformOnPickUp());

    }

    private IEnumerator FixPlayerPosOnLayUp()
    {
        //Debug.Break();
        while (layUpJumpDist > 0)
        {
            Vector2 dir = layUpPosDiff.normalized;
            this.transform.Translate(new Vector3(dir.x,0,dir.y) * layUpSpeed);
            layUpJumpDist -= layUpSpeed;
            //Debug.Log($"¾àÀë»¹Ê£{layUpJumpDist}");
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

    public void OnLayUpEnd()
    {
        this.transform.position = new Vector3(this.transform.position.x,0,this.transform.position.z);
    }
}


















public class A
{

}