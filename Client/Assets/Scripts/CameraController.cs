using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviourInstance<CameraController> {
    public Transform pivot;
    public PlayerCamera playerCamera;
    private Transform playerCameraPivot;
    private bool isAttatchedPlayer = false;

    void Awake() {
        Assert.IsNotNull(this.playerCamera);
        Assert.IsNotNull(this.pivot);
    }

    public void AttatchCameraToPlayer(Transform playerCameraPivot) {
        this.playerCameraPivot = playerCameraPivot;
        this.playerCamera.AttatchCameraToPlayer(playerCameraPivot, this.pivot);
        this.isAttatchedPlayer = true;        
    }

    public void ZoomIn() {
        this.playerCamera.ZoomIn();
    }

    public void ZoomOut() {
        this.playerCamera.ZoomOut();
    }

    public void GunRecoil(float degree, float duration) {
        this.playerCamera.GunRecoil(degree , duration);
    }
}
