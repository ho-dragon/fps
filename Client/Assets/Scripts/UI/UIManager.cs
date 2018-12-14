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

    public void ShowToastMessgae(string messgae, float duration, System.Action callback = null) {
        StopCoroutine("ToastMessage");
        StartCoroutine(ToastMessage(this.waitingTime, messgae, duration, callback));
    }

    public void Alert(string msg) {
        this.serverAlert.text = msg;
    }

    public void AlertCountDown(int count, string message, System.Action callback = null) {
        StopCoroutine("CountDown");
        StartCoroutine(CountDown(this.serverAlert, count, message, callback));
    }

    IEnumerator CountDown(Text label, int count, string message, System.Action callback) {
        for (int i = 0; i < count; i++) {
            label.text = string.Format(message, count - i);
            yield return new WaitForSeconds(1f);
        }
        label.text = "";
        if (callback != null) {
            callback();
        }
    }

    IEnumerator ToastMessage(Text label, string message, float duration, System.Action callback) {
        label.text = message;
        yield return new WaitForSeconds(duration);
        label.text = "";
        if (callback != null) {
            callback();
        }
    }

}