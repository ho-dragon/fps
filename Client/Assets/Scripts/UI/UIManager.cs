using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviourInstance<UIManager> {
    public PlayerHUD hud;
    public Text waitingTime;
    public Text serverAlert;

    void Awake() {
        Assert.IsNotNull(this.hud);
        Assert.IsNotNull(this.waitingTime);
        Assert.IsNotNull(this.serverAlert);
    }

    public void UpdateWaitingPlayers(int joinedPlayerCount, int maxPlayerCount, int remianTimeToPlay) {
        this.waitingTime.text = string.Format("플레이어를 기다리고 있습니다...{0} 참여인원({1}/{2})", remianTimeToPlay, joinedPlayerCount, maxPlayerCount);
    }

    public void Alert(string msg) {
        this.serverAlert.text = msg;
    }

    public void EndGame() {
    }

}