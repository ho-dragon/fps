using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
    public Camera camera;
    //void Awake() {
    //    UnityEngine.Assertions.Assert.IsNotNull(this.camera);
    //}

    public void SetCamara(Camera camera) {
        this.camera = camera;
    }

    void Update() {
        if (camera == null) {
            return;
        }
        this.transform.LookAt(this.transform.position + camera.transform.rotation * Vector3.forward,
            camera.transform.rotation * Vector3.up);
    }
}