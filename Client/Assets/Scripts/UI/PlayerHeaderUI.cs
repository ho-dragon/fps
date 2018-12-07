using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHeaderUI : MonoBehaviour {
    public HpGage hpBar;
    public Text nickName;

    public void SetNickName(string nickName) {
        this.nickName.text = nickName;
    }

    public void SetHealth(float currentHP, float maxHP) {
        this.hpBar.SetHP(currentHP, maxHP);
    }

    public void SetDead(bool isDead) {
       //Todo. DeadMark
    }
}
