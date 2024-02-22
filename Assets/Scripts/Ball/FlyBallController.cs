using UnityEngine;

public class FlyBallController : MonoBehaviour
{
    [SerializeField]
    private bool isNeedPickUp;
    [SerializeField]
    private bool hasPickUp;
    [SerializeField]
    private GameObject playerObj;
    [SerializeField]
    private bool isOnFloor;

    private void Start()
    {
        isNeedPickUp = false;
        hasPickUp = false;
        isOnFloor = false;
        playerObj = GameObject.Find("Player");
        EventManager<AnimationEvent>.instance.AddListener(AnimationEvent.isNeedPickUpEvent, IsNeedPickUp);
    }

    private void Update()
    {
        if (!hasPickUp && isNeedPickUp && isOnFloor)
        {
            float distance = Vector2.Distance(playerObj.transform.position.ToVector2(), this.transform.position.ToVector2());
            if(distance <= 1.0f)
            {
                isNeedPickUp = false;
                hasPickUp = true;
                isOnFloor = false;
                EventManager<AnimationEvent>.instance.TriggerEvent(AnimationEvent.PickUpBallEvent, playerObj.gameObject.name);
            }
        }
    }

    private void IsNeedPickUp(object[] obj)
    {
        isNeedPickUp = true;
        hasPickUp = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isOnFloor = true;
            SoundManager.PlaySound(SoundManager.SoundType.BounceFloor,this.transform.position);
        }
    }
}
