using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBallController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.PickUpBallEvent, other.gameObject.name);
        }
    }
}
