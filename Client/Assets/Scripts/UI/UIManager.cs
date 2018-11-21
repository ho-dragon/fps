using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviourInstance<UIManager> {
    public PlayerHUD hud;

    void Awake() {
        Assert.IsNotNull(this.hud);
    }
    public void PlayerHP(float currentHP, float maxHP) {
        Logger.DebugHighlight("[UIManager.PlayerHP] Main UI ----");
        this.hud.hp.SetHP(currentHP, maxHP);
    }
}
