using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerActionController : MonoBehaviour {
    public PlayerMove move;
    public PlayerShoot shoot;
    void Awake() {
        Assert.IsNotNull(this.move);
        Assert.IsNotNull(this.shoot);
    }
}
