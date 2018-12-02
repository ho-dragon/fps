using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
    public HpGage hp;
    public Text playerName;
    public GameObject gunCross;
    public CanvasGroup hitCrossGroup;
    private IEnumerator coFadeOut;

    void Awake() {
        Assert.IsNotNull(this.hp);
        Assert.IsNotNull(this.playerName);
        Assert.IsNotNull(this.gunCross);
    }

    public void SetName(string name) {
        this.playerName.text = name;
    }

    public void SetHP(float currentHP, float maxHP) {
        this.hp.SetHP(currentHP, maxHP);
    }

    public void EnableGunCross(bool isEnable) {
        this.gunCross.SetActive(isEnable);        
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
        }
    }
}
