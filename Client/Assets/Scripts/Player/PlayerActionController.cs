using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerActionController : MonoBehaviour {
    public PlayerMove move;
    public PlayerShoot shoot;
    private PlayerAnimationController animationController;
    private bool isLocalPayer = false;
    void Awake() {
        Assert.IsNotNull(this.move);
        Assert.IsNotNull(this.shoot);
    }

    public void Init(PlayerAnimationController animationController, Transform playerTras, int playerNmber, System.Action<int, Vector3, float> moveCallback) {
        this.animationController = animationController;
        this.move.Init(animationController, playerTras, playerNmber, moveCallback);
        this.animationController.Init(playerNmber);
    }

    public void SetCamera(Transform playerCamera) {
        this.move.SetCamera(playerCamera);
    }

    public void SetLocalPlayer(bool isLocalPlayer) {
        this.isLocalPayer = isLocalPlayer;
        this.move.SetLocalPlayer(isLocalPlayer);
        this.animationController.SetLocalPlayer(isLocalPlayer);
    }

    public void OnMove(Vector3 toPosition, float yaw) {
        this.move.OnMoveTo(toPosition, yaw);
    }

    public void OnAction(PLAYER_ACTION_TYPE actionType) {
        this.animationController.OnAcion(actionType);
    }

    public void SetWeapon(Weapon weapon) {
        this.shoot.Init(weapon);
    }

    void Update() {
        if(this.isLocalPayer == false) {
            return;
        }

        if (this.shoot.IsShootable == false) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            this.shoot.Shoot();
            this.animationController.OnAcion(PLAYER_ACTION_TYPE.Attack);
        }

        if (Input.GetMouseButton(1)) {
            this.animationController.OnAcion(PLAYER_ACTION_TYPE.Aiming);
            CameraController.inst.ZoomIn();
        } else {
            CameraController.inst.ZoomOut();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            this.animationController.OnAcion(PLAYER_ACTION_TYPE.Jump);
        }
    }
}
