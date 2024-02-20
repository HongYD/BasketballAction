using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private int scoreData;

    private void Start()
    {
        scoreData = 0;
        scoreText.text = scoreData.ToString("00");
        EventManager<GameEventEvent>.instance.AddListener(GameEventEvent.OnScoreEvent, OnScoreEvent);
    }

    private void OnScoreEvent(object[] param)
    {
        int score = (int)param[0];
        scoreData += score;
        scoreText.text = scoreData.ToString("00");
    }
}
