using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBallController : MonoBehaviour
{
    [SerializeField]
    private bool isNeedPickUp;
    [SerializeField]
    private bool hasPickUp;
    [SerializeField]
    private GameObject playerObj;

    private void Start()
    {
        isNeedPickUp = false;
        hasPickUp = false;
        playerObj = GameObject.Find("Player");
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.isNeedPickUpEvent, IsNeedPickUp);
    }

    private void Update()
    {
        if (!hasPickUp && isNeedPickUp)
        {
            float distance = Vector2.Distance(playerObj.transform.position.ToVector2(), this.transform.position.ToVector2());
            if(distance <= 1.0f)
            {
                isNeedPickUp = false;
                hasPickUp = true;
                EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.PickUpBallEvent, playerObj.gameObject.name);
            }
        }
    }

    private void IsNeedPickUp(object[] obj)
    {
        isNeedPickUp = true;
        hasPickUp = false;
    }
}
