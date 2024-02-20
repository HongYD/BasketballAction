using System;
using System.Collections;
using System.Collections.Generic;

public enum GameEventEvent
{
    OnScoreEvent,
}

public enum AnimationEvent
{
    PickUpBallEvent,
    PickUpBallEndEvent,
    isNeedPickUpEvent,
}

public enum PlayerInputEvent
{
    Move=0,
    MoveCancled,
    Shoot,
    Pass,
    Rececive,
}

public enum PlayerAbilityEvent
{
    ShootAbility=0,
}

public class EventManager<T>
{
    private static EventManager<T> _instance;
    public static EventManager<T> instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventManager<T>();
            }
            return _instance;
        }
    }

    private Dictionary<T, Action<object[]>> eventDict;

    private EventManager()
    {
        eventDict = new Dictionary<T, Action<object[]>>();
    }

    public void TriggerEvent(T eventName, params object[] param)
    {
        if (eventDict.ContainsKey(eventName))
        {
            eventDict[eventName](param);
        }
    }

    public void AddListener(T eventName, Action<object[]> cb)
    {
        if (eventDict.ContainsKey(eventName))
        {
            eventDict[eventName] += cb;
        }
        else
        {
            eventDict[eventName] = cb;
        }

    }
    public void RemoveListener(T eventName, Action<object[]> cb)
    {
        if (eventDict.ContainsKey(eventName))
        {
            eventDict[eventName] -= cb;
        }
    }
}