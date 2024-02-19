using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBallController : MonoBehaviour
{
    [SerializeField]
    private bool isNeedPickUp = false;
    private void Start()
    {
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.isNeedPickUpEvent, IsNeedPickUp);
    }

    private void IsNeedPickUp(object[] obj)
    {
        isNeedPickUp = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isNeedPickUp)
        {
            isNeedPickUp = true;
            EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.PickUpBallEvent, other.gameObject.name);
        }
    }
}
