using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class PlayerHUD : MonoBehaviour {
    public HpGage hp;
    void Awake() {
        Assert.IsNotNull(this.hp);
    }
}
