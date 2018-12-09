using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCamera : MonoBehaviour {
    public Camera targetCamera;
	
    public void SetCamera(Camera camera) {
        this.targetCamera = camera;
    }

    private void Update() {
        if (this.targetCamera != null) {
            this.transform.LookAt(this.transform.position + this.targetCamera.transform.rotation * Vector3.back
            , this.targetCamera.transform.rotation * Vector3.up);
        }
    }

}
