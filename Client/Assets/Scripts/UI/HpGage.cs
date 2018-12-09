using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class HpGage : MonoBehaviour {
    public Image image;
    public Text text;

    void Awake() {
        Assert.IsNotNull(this.image);
    }

    public void SetHP(float currentHP, float maxHP) {
        Logger.DebugHighlight("[HpGage.SetHP] currentHP = {0} / maxHP = {1}", currentHP, maxHP);
        if (this.text != null) {
            if (currentHP <= 0f) {
                this.text.text = "DEAD";
            } else {
                this.text.text = string.Format("{0} / {1}", currentHP, maxHP);
            }            
        }
        this.image.fillAmount = currentHP / maxHP;
    }
}
