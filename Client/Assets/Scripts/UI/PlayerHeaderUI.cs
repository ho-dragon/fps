using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHeaderUI : MonoBehaviour {
    public HpGage hpBar;
    public Text nickName;
    public Canvas canvas;

    private void Awake() {
        Assert.IsNotNull(this.hpBar);
        Assert.IsNotNull(this.nickName);
        Assert.IsNotNull(this.canvas);
    }

    public void SetCamera(Camera camera) {
        this.canvas.worldCamera = camera;
    }

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
