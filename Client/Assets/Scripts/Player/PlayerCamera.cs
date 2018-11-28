using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    private float rotationX = 0F;
    private float rotationY = 0F;
    private float lockPos = 0F;
    private Quaternion originalRotation;
    private Transform playerPivot;
    private Transform cameraPivot;
    private bool isAttachedPlayer = false;

    void Awake() {
		Assert.IsNotNull(this.GetComponent<Camera>());
        this.originalRotation = this.transform.localRotation;
    }

    public void AttatchCameraToPlayer(Transform playerPivot, Transform cameraPivot) {
        this.playerPivot = playerPivot;
        this.cameraPivot = cameraPivot;
        this.isAttachedPlayer = false;
    }

    public Camera GetCamera() {
        return this.GetComponent<Camera>();
    }

    void Update() {
        if (this.isAttachedPlayer == false) {
            return;
        }
        
        if (axes == RotationAxes.MouseXAndY) {
            this.rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            this.rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            this.rotationX = ClampAngle(rotationX, minimumX, maximumX);
            this.rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
            SetRotation(originalRotation * xQuaternion * yQuaternion);
        } else if (axes == RotationAxes.MouseX) {
            this.rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            this.rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            SetRotation(originalRotation * xQuaternion);
        } else {
            this.rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            this.rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            SetRotation(originalRotation * yQuaternion);
        }
        SetRotation(Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockPos));
    }

    private void SetRotation(Quaternion quaternion) {
        this.playerPivot.localRotation = quaternion;
        this.cameraPivot.localRotation = quaternion;
    }

    public static float ClampAngle(float angle, float min, float max) {
     if (angle < -360f)
         angle += 360F;

     if (angle > 360F)
         angle -= 360F;

     return Mathf.Clamp (angle, min, max);
 }

}
