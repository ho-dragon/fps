using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

public class HpGage : MonoBehaviour {
    public Image image;
    void Awake() {
        Assert.IsNotNull(this.image);
    }

    public void SetGage(float value) {
        this.image.fillAmount = value;
    }
}
