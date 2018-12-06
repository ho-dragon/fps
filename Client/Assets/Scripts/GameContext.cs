using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext {
    public EventManager eventManager;
    public int maxPlayTime;
    public int playTime;
    public int scoreRed;
    public int scoreBlue;
    public int joinedPlayerCount;

    public GameContext(EventManager eventManager, int maxPlayTime, int playTime, int scoreRed, int scoreBlue, int joinedPlayerCount) {
        this.eventManager = eventManager;
        this.maxPlayTime = maxPlayTime;
        this.playTime = playTime;
        this.scoreRed = scoreRed;
        this.scoreBlue = scoreBlue;
        this.joinedPlayerCount = joinedPlayerCount;
    }

    public void UpdatePlayTime(int playTime) {
        this.playTime = playTime;
        this.eventManager.UpdatePlayTime(playTime);
    }
}