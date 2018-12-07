using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {
    public delegate void UpdateRemainTimeHandler(int remainTime);
    public event UpdateRemainTimeHandler OnUpdateRemainTime;
    public void UpdateRemainTime(int remainTime) {
        if (OnUpdateRemainTime != null) {
            OnUpdateRemainTime(remainTime);
        }
    }

    public delegate void UpdateScoreHandler(int scoreRed, int scoreBlue);
    public event UpdateScoreHandler OnUpdateScore;
    public void UpdateScore(int scoreRed, int scoreBlue) {
        if (OnUpdateScore != null) {
            OnUpdateScore(scoreRed, scoreBlue);
        }
    }

    public delegate void UpdatePlayerHandler(int playerRed, int playerBlue);
    public event UpdatePlayerHandler OnUpdatePlayerCount;
    public void UpdatePlayerCount(int playerRed, int playerBlue) {
        if (OnUpdatePlayerCount != null) {
            OnUpdatePlayerCount(playerRed, playerBlue);
        }
    }
}
