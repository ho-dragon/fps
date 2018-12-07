using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext {
    public EventManager eventManager;
    public int remainTime;
    public int scoreRed;
    public int scoreBlue;
    public int playerCountRed;
    public int playerCountBlue;

    public GameContext(EventManager eventManager, int remainTime, int scoreRed, int scoreBlue, int playerCountRed, int playerCountBlue) {
        this.eventManager = eventManager;
        UpdatePlayTime(remainTime);
        UpdateScore(scoreRed, scoreBlue);
        UpdatePlayerCount(playerCountRed, playerCountBlue);        
    }

    public void UpdatePlayerCount(int playerRed, int playerBlue) {
        this.playerCountRed = playerRed;
        this.playerCountBlue = playerBlue;
        this.eventManager.UpdatePlayerCount(playerRed, playerBlue);
    }

    public void UpdatePlayTime(int remainTime) {
        this.remainTime = remainTime;
        this.eventManager.UpdateRemainTime(remainTime);
    }

    public void UpdateScore(int scoreRed, int scoreBlue) {
        this.scoreRed = scoreRed;
        this.scoreBlue = scoreBlue;
        this.eventManager.UpdateScore(scoreRed, scoreBlue);
    }
}