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

public class BasketballPlayer : PlayerAgent
{
    [SerializeField]
    private PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.Idle
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
