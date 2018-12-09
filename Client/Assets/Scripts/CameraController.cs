using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviourInstance<CameraController> {
    public Transform pivot;
    public PlayerCamera playerCamera;
    private Transform playerCameraPivot;

    void Awake() {
        Assert.IsNotNull(this.playerCamera);
        Assert.IsNotNull(this.pivot);
    }

    public void AttatchCameraToPlayer(Transform playerCameraPivot) {
        this.playerCameraPivot = playerCameraPivot;
        this.playerCamera.AttatchCameraToPlayer(playerCameraPivot, this.pivot);     
    }

    public void Stop() {
        this.playerCamera.Stop();
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
