using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviourInstance<UIManager> {
    public PlayerHUD hud;

    void Awake() {
        Assert.IsNotNull(this.hud);
    }
}
