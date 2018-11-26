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

    public void Init(Transform playerTras, int PlayerNmber, System.Action<int, Vector3> moveCallback) {
        this.move.Init(playerTras, PlayerNmber, moveCallback);
    }

    public void SetCamera(Transform playerCamera) {
        this.move.SetCamera(playerCamera);
    }

    public void SetLocalPlayer(bool isLocalPlayer) {
        this.move.IsLocalPlayer = isLocalPlayer;
    }

    public void MoveTo(Vector3 toPosition) {
        this.move.MoveTo(toPosition);
    }

    public void SetWeapon(Weapon weapon) {
        this.shoot.Init(weapon);
    }    

    public void Jump() {
        this.move.Jump();
    }

    void Update() {
        if (this.shoot.IsShootable == false) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            this.shoot.Shoot();
        }
    }
}
