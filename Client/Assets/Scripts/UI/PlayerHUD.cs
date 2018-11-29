using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
    public HpGage hp;
    public Text playerName;
    void Awake() {
        Assert.IsNotNull(this.hp);
        Assert.IsNotNull(this.playerName);
    }

    public void SetName(string name) {
        this.playerName.text = name;
    }

    public void SetHP(float currentHP, float maxHP) {
        this.hp.SetHP(currentHP, maxHP);
    }
}
