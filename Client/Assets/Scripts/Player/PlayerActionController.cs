using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerActionController : MonoBehaviour {
    public PlayerMove move;
    public PlayerShoot shoot;
    private PlayerAnimationController animationController;
    private bool isLocalPlayer = false;
    private bool isZoomOut = false;
    private bool isDead = false;
    void Awake() {
        Assert.IsNotNull(this.move);
        Assert.IsNotNull(this.shoot);
    }

    public void Init(PlayerAnimationController animationController, Transform playerTras, int playerNmber, bool isLocalPlayer, bool isDead, System.Action<int, Vector3, float> moveCallback) {
        this.isLocalPlayer = isLocalPlayer;
        this.isDead = isDead;
        this.move.Init(animationController, playerTras, playerNmber, isDead, moveCallback);
        this.animationController = animationController;
        this.animationController.Init(playerNmber);
    }

    public void SetCamera(Transform playerCamera) {
        this.move.SetCamera(playerCamera);
    }

    public void SetLocalPlayer(bool isLocalPlayer) {
        this.isLocalPlayer = isLocalPlayer;
        this.move.SetLocalPlayer(isLocalPlayer);
        this.animationController.SetLocalPlayer(isLocalPlayer);
    }

    public void UpdatePlayerDead(bool isDead) {
        this.isDead = isDead;
        this.move.UpdatePlayerDead(isDead);
    }

    public void OnMove(Vector3 toPosition, float yaw) {
        this.move.OnMoveTo(toPosition, yaw);
    }

    public void OnAction(PlayerActionType actionType, bool isFromServer) {
        this.animationController.OnAcion(actionType, isFromServer);
    }

    public void SetWeapon(Weapon weapon) {
        this.shoot.Init(weapon);
    }

    void Update() {
        if (this.isLocalPlayer == false) {
            return;
        }

        if (this.isDead) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && this.isZoomOut) {
            this.animationController.OnAcion(PlayerActionType.Jump);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)) {
            if (this.animationController.IsAiming() && this.shoot.IsShootable) {
                CameraController.inst.GunRecoil(2f, 0.2f);
                this.shoot.Shoot();
                this.animationController.OnAcion(PlayerActionType.Attack);
            }
        }

        if (Input.GetMouseButton(1)) {
            this.isZoomOut = false;
            this.animationController.OnAcion(PlayerActionType.Aiming);
            CameraController.inst.ZoomIn();
        } else {
            if(this.isZoomOut == false) {
                this.animationController.OnAcion(PlayerActionType.Idle);
                CameraController.inst.ZoomOut();
                this.isZoomOut = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            this.animationController.OnAcion(PlayerActionType.Sitting);
        }
    }
}
