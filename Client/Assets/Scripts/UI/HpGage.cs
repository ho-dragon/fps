using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class HpGage : MonoBehaviour {
    public Image image;
    public Text text;
    public CameraFacingBillboard facing;

    void Awake() {
        Assert.IsNotNull(this.image);
    }

    public void SetHP(float currentHP, float maxHP) {
        if (this.text != null) {
            this.text.text = string.Format("{0}/{1}", currentHP, maxHP);
        }
        this.image.fillAmount = currentHP / maxHP;
    }
}
