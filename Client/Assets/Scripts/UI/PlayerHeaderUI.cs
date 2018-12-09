using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHeaderUI : MonoBehaviour {
    public HpGage hpBar;
    public Text nickName;
    public BillboardCamera billboardCamera;

    private void Awake() {
        Assert.IsNotNull(this.hpBar);
        Assert.IsNotNull(this.nickName);
        Assert.IsNotNull(this.billboardCamera);
    }

    public void SetCamera(Camera camera) {
        this.billboardCamera.SetCamera(camera);
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
