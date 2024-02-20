using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;
    public static GameAssets Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameObject>("AssetsLoader/AssetsLoader").GetComponent<GameAssets>());
                return _instance;
            }
            return _instance;
        }
    }

    public SoundAudioClip[] soundAudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.SoundType SoundType;
        public AudioClip AudioClip;
    }
}


