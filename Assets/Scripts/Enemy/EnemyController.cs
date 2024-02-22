using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public struct EnemyBehaviorData
    {
        public const float ChasePlayer = 2.0f;
        public const float defenceDist = 1.5f;
    }
    public enum EnemyState
    {
        Idle,
        Chase,
        ReadyToDefence,
        Defence,
    }
    [SerializeField]
    private GameObject player;
    public EnemyState state;
    [SerializeField]
    private Vector3 chaseTarget;
    [SerializeField]
    private float chaseSpeed = 1.5f;
    [SerializeField]
    private float angleSpeed = 5f;
    private Animator enemyAnimator;
    private Vector3 faceDir;

    // Start is called before the first frame update
    void Start()
    {
        state = EnemyState.Idle;
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.ReadyToDefence:
                ReadyToDefense();
                break;
            case EnemyState.Defence:
                Defense();
                break;
        }
    }

    private void ReadyToDefense()
    {
        
    }

    private void Defense()
    {

    }

    private void Chase()
    {
        chaseTarget = player.transform.position + player.transform.forward * EnemyBehaviorData.defenceDist;
        Vector3 dir = (chaseTarget - this.transform.position).normalized;
        faceDir = dir;
        Vector3 movement = new Vector3(dir.x,0,dir.z) * chaseSpeed * Time.deltaTime;
        transform.Translate(movement);
        RotatePlayer();
        enemyAnimator.SetBool("isJog",true);
        if(Vector2.Distance(this.transform.position.ToVector2(), chaseTarget.ToVector2())< 0.1f)
        {
            enemyAnimator.SetBool("isJog", false);
            state = EnemyState.ReadyToDefence;
        }
    }

    private void RotatePlayer()
    {
        Vector3 lookDir = new Vector3(faceDir.x, 0, faceDir.z);
        Quaternion rot = Quaternion.LookRotation(lookDir, transform.up);
        float anglediff = Mathf.Abs(rot.eulerAngles.y - transform.eulerAngles.y);
        if (anglediff > 30.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, angleSpeed * Time.deltaTime);
        }
    }

    private void Idle() 
    {
        float distToPlayer = Vector2.Distance(this.transform.position.ToVector2(), player.transform.position.ToVector2());
        if(distToPlayer > EnemyBehaviorData.ChasePlayer)
        {
            state = EnemyState.Chase;
        }
    }
}
