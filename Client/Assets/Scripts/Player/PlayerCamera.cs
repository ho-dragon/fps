using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    private float sensitivityX = 2f;
    private float sensitivityY = 2f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -60f;
    public float maximumY = 60f;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float lockPos = 0f;
    private Quaternion originalRotation;
    private Transform playerPivot;
    private Transform cameraPivot;
    private bool isAttachedPlayer = false;
    private bool isDoingZoomIn = false;
    private bool isDoneZoomIn = false;
    private bool isDoneZoomOut = false;
    private float zoomTime = 2f;
         
    void Awake() {
		Assert.IsNotNull(this.GetComponent<Camera>());
        this.originalRotation = this.transform.localRotation;
    }

    public void AttatchCameraToPlayer(Transform playerPivot, Transform cameraPivot) {
        this.playerPivot = playerPivot;
        this.cameraPivot = cameraPivot;
        this.isAttachedPlayer = true;
    }

    public Camera GetCamera() {
        return this.GetComponent<Camera>();
    }

    public void Stop() {
        this.isAttachedPlayer = false;
    }

    public void ZoomIn() {
        this.isDoingZoomIn = true;
        this.isDoneZoomOut = false;
    }

    public void ZoomOut() {
        this.isDoingZoomIn = false;
        this.isDoneZoomIn = false;
    }


    private bool isShaking = false;
    private float durationRecoil = 0f;
    private float currentDurationRecoil = 0f;
    private float degreeRecoil = 0f;
    private float originRotationX = 0f;
    public void GunRecoil(float degree, float duration) {
        this.currentDurationRecoil = 0f;
        this.durationRecoil = duration;
        this.degreeRecoil = 0.1f;
        this.isShaking = true;
    }

    
    private void UpdateRecoil(float duration) {
        this.currentDurationRecoil += Time.deltaTime;
        this.rotationX += this.degreeRecoil;
        this.rotationX = ClampAngle(rotationX, minimumX, maximumX);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        SetRotation(originalRotation * xQuaternion);
        if (this.currentDurationRecoil > this.durationRecoil) {
            this.isShaking = false;
        }
    }

    void Update() {
        if (this.isAttachedPlayer == false) {
            return;
        }

        ZooInOut();

        this.cameraPivot.position = this.playerPivot.position;
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

    private float duratuonTime = 0f;
    private float accelerationTime = 0f;
    private void ZooInOut() {
        if (this.isDoingZoomIn) {
            if (this.isDoneZoomIn == false) {
                this.accelerationTime = Mathf.Lerp(0.1f, 0.5f, duratuonTime);
                duratuonTime += Time.deltaTime + this.accelerationTime / zoomTime;
                this.GetComponent<Camera>().fieldOfView = Mathf.Lerp(60, 40, duratuonTime);
                if (this.GetComponent<Camera>().fieldOfView <= 40) {
                    this.isDoneZoomIn = true;
                    UIManager.inst.hud.SetActiveGunCross(true);
                }
            }
        } else {
            if (this.isDoneZoomOut == false) {
                UIManager.inst.hud.SetActiveGunCross(false);
                this.accelerationTime = Mathf.Lerp(0.5f, 0.1f, duratuonTime);
                duratuonTime -= Time.deltaTime + this.accelerationTime / zoomTime;
                this.GetComponent<Camera>().fieldOfView = Mathf.Lerp(60, 40, duratuonTime);
                if (this.GetComponent<Camera>().fieldOfView >= 60f) {
                    this.isDoneZoomOut = true;
                }
            }
        }
    }

    private void SetRotation(Quaternion quaternion) {
        this.playerPivot.localRotation = Quaternion.Euler(this.playerPivot.localRotation.eulerAngles.x, quaternion.eulerAngles.y, this.playerPivot.localRotation.eulerAngles.z);
        this.cameraPivot.localRotation = quaternion;
    }

    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360f) {
            angle += 360F;
        }
         
        if (angle > 360f) {
            angle -= 360f;
        }        
        return Mathf.Clamp (angle, min, max);
    }
}
