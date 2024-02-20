using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public enum SoundType
    {
        BounceWire,
        BounceFloor,
        BounceBasket,
        PlayerMove,
    }

    private static Dictionary<SoundType, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<SoundType, float>();
        soundTimerDictionary[SoundType.BounceFloor] = 0f;
    }

    public static void PlaySound(SoundType soundType, Vector3 position)
    {
        if (CanPlaySound(soundType))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(soundType);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0;
            audioSource.Play();
            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public static void PlaySound(SoundType soundType)
    {
        if (CanPlaySound(soundType))
        {
            if(oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(soundType));
        }
    }

    private static bool CanPlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            default:
                return true;
            case SoundType.BounceFloor:
                if (soundTimerDictionary.ContainsKey(soundType))
                {
                    float lastTimePlayed =soundTimerDictionary[soundType];
                    float playerMoveTimeMax = 0.05f;
                    if (Time.time - lastTimePlayed > playerMoveTimeMax)
                    {
                        soundTimerDictionary[soundType]=Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }                      
                }
                else
                {
                    return true;
                }
        }
    }

    private static AudioClip GetAudioClip(SoundType soundType)
    {
        foreach(GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
        {
            if(soundAudioClip.SoundType == soundType)
            {
                return soundAudioClip.AudioClip;
            }
        }
        Debug.LogError("无法找到音频文件");
        return null;
    }
}
