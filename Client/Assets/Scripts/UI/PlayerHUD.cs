using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
    public GameObject parentStatus;
    public GameObject parentScore;
    public GameObject parentTime;
    public GameObject gunCross;
    public CanvasGroup hitCrossGroup;
    public HpGage hp;
    public Text playerName;
    public Text killDeath;    
    public Text scoreRed;
    public Text scoreBlue;
    public Text scoreGoal;
    public Text remainTime;
    public Text playerCount;

    private IEnumerator coFadeOut;

    void Awake() {
        Assert.IsNotNull(this.parentStatus);
        Assert.IsNotNull(this.parentScore);
        Assert.IsNotNull(this.parentTime);
        Assert.IsNotNull(this.hitCrossGroup);
        Assert.IsNotNull(this.hp);
        Assert.IsNotNull(this.playerName);
        Assert.IsNotNull(this.gunCross);
        Assert.IsNotNull(this.killDeath);
    }


    public void AddEvents(EventManager eventManager) {
        eventManager.OnUpdateRemainTime += UpdateRemainTime;
        eventManager.OnUpdateScore += UpdateScore;
        eventManager.OnUpdatePlayerCount += UpdatePlayerCount;
    }

    public void EnablePlayerStatus() {
       this.parentStatus.SetActive(true);
    }

    public void SetScoreGoal(int goal) {
        this.parentScore.SetActive(true);
        this.scoreGoal.text = goal.ToString();
    }

    public void UpdateScore(int scoreRed, int scoreBlue) {
        this.parentScore.SetActive(true);
        this.scoreRed.text = scoreRed.ToString();
        this.scoreBlue.text = scoreBlue.ToString();
    }

    public void SetName(string name) {
        if (this.playerName.gameObject.activeSelf == false) {
            this.playerName.gameObject.SetActive(true);
        }        
        this.playerName.text = name;
    }

    public void UpdateHP(float currentHP, float maxHP) {
        if (this.hp.gameObject.activeSelf == false) {
            this.hp.gameObject.SetActive(true);
        }        
        this.hp.SetHP(currentHP, maxHP);
    }

    public void SetKillDeath(int kill, int death) {
        this.killDeath.text = string.Format("Kill {0} Death {1}", kill, death);
    }

    public void SetWaitingPlayerCount(int playerCount) {
        this.playerCount.text = string.Format("joined player {0}", playerCount.ToString());
    }

    public void UpdatePlayerCount(int playerRed, int playerBlue) {
        this.playerCount.text = string.Format("red {0} blue {1}", playerRed, playerBlue);
    }

    public void SetActiveGunCross(bool isOn) {
        this.gunCross.SetActive(isOn);        
    }

    public void UpdateRemainTime(int remainTime) {
        this.parentTime.SetActive(true);
        this.remainTime.text = string.Format("{0}s", remainTime);
    }

    public void HitEffect() {
        if (this.coFadeOut != null) {
            StopCoroutine(this.coFadeOut);
        }
        this.coFadeOut = FadeOut(this.hitCrossGroup, 1f);
        StartCoroutine(this.coFadeOut);
    }

    IEnumerator FadeOut(CanvasGroup group, float duration) {
        float alpha = 1f;
        float remainDuraion = duration;
        group.alpha = 1f;
        while (alpha > 0) {            
            yield return null;
            remainDuraion -= Time.deltaTime;
            alpha -= 0.05f;
            if (alpha < 0) {
                alpha = 0f;
            }
            group.alpha = alpha;
        }
    }
}
