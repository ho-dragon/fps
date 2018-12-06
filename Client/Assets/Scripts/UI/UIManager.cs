﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviourInstance<UIManager> {
    public PlayerHUD hud;

    void Awake() {
        Assert.IsNotNull(this.hud);
    }

    public void SetHP(float currentHP, float maxHP) {
        this.hud.SetHP(currentHP, maxHP);
    }

    public void SetName(string name) {
        this.hud.SetName(name);
    }

    public void SetActiveGunCross(bool isOn) {
        this.hud.SetActiveGunCross(isOn);
    }

    public void HitEffect() {
        this.hud.HitEffect();
    }

    public void UpdateWaitingPlayers(int joinedPlayerCount, int maxPlayerCount,) {

    }
}
