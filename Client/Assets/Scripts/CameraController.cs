using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviourInstance<CameraController> {
    public Transform pivot;
    public Transform position;
    public PlayerCamera playerCamera;
    private Transform playerCameraPivot;
    public bool isAttatchedPlayer = false;

    void Awake() {
        Assert.IsNotNull(this.playerCamera);
        Assert.IsNotNull(this.pivot);
        Assert.IsNotNull(this.position);
    }

    public void AttatchCameraToPlayer(Transform playerCameraPivot) {
        this.playerCameraPivot = playerCameraPivot;
        this.playerCamera.AttatchCameraToPlayer(playerCameraPivot, this.pivot);
        this.isAttatchedPlayer = true;        
    }

    void Update() {
        if (this.isAttatchedPlayer == false) {
            return;
        }
        this.pivot = this.playerCameraPivot;
    }

}
