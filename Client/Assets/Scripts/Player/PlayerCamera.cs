using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class PlayerCamera : MonoBehaviourInstance<PlayerCamera> {
    public GameObject target;
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

    void Awake() {
		Assert.IsNotNull(this.GetComponent<Camera>());
        this.originalRotation = this.transform.localRotation;
    }

    public Camera GetCamera() {
        return this.GetComponent<Camera>();
    }

    public void Look(Transform targetRotation) {
        this.transform.LookAt(targetRotation);
    }

    public void MoveChildTrans(Transform parent) {
       this.transform.parent = parent;
       this.transform.localPosition = Vector3.zero;
    }

    void Update() {
        if (axes == RotationAxes.MouseXAndY) {
            this.rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            this.rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            this.rotationX = ClampAngle(rotationX, minimumX, maximumX);
            this.rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
            this.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX) {
            this.rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            this.rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            this.transform.localRotation = originalRotation * xQuaternion;
        }
        else {
            this.rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            this.rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            this.transform.localRotation = originalRotation * yQuaternion;
        }
        this.transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockPos);
    }

    public static float ClampAngle(float angle, float min, float max) {
     if (angle < -360f)
         angle += 360F;

     if (angle > 360F)
         angle -= 360F;

     return Mathf.Clamp (angle, min, max);
 }

}
