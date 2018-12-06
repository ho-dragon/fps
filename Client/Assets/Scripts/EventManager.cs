using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {
    public delegate void UpdatePlayTimeHandler(int PlayTime);
    public event UpdatePlayTimeHandler OnUpdatePlayTime;
    public void UpdatePlayTime(int PlayTime) {
        if (OnUpdatePlayTime != null) {
            OnUpdatePlayTime(PlayTime);
        }
    }


    public delegate void UpdateScoreHandler(int scoreRed, int scoreBlue);
    public event UpdateScoreHandler OnUpdateScore;
    public void UpdateScore(int scoreRed, int scoreBlue) {
        if (OnUpdateScore != null) {
            OnUpdateScore(scoreRed, scoreBlue);
        }
    }
}
