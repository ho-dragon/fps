using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerMove : MonoBehaviour {
    public PlayerAnimationController animationController;
    public Rigidbody playerRigidbody;
    private int playerNumber = 0;    
    private System.Action<int, Vector3, float> moveCallback;
    private Transform cameraTransform;
    private Transform playerTrans;
    private bool isStartMove = false;
    private Vector3 toPosition;
    private float aimingSpeed = 1f;
    private float walkSpeed = 3f;
    private float runSpeed = 6f;
    private float tilt = 1f;
    private bool isLocalPlayer = false;
    private PLAYER_ACTION_TYPE currentActionType = PLAYER_ACTION_TYPE.Idle;

    void Awake() {
        Assert.IsNotNull(this.playerRigidbody);
    }

    public void Init(PlayerAnimationController animationController, Transform playerTrans, int number, System.Action<int, Vector3, float> moveCallback) {
        Logger.DebugHighlight("[PlayerMove] Init");
        this.animationController = animationController;
        this.playerTrans = playerTrans;
        this.playerNumber = number;
        this.moveCallback = moveCallback;
    }

    public void SetLocalPlayer(bool isLocalPlayer) {
        this.isLocalPlayer = isLocalPlayer;
    }

    public void SetCamera(Transform camearaTrans) {
        this.cameraTransform = camearaTrans;
    }

    public void OnMoveTo(Vector3 toPosition, float yaw) {
        this.isStartMove = true;
        this.toPosition = toPosition;
        this.playerTrans.localRotation = Quaternion.Euler(this.playerTrans.localRotation.eulerAngles.x, yaw, this.playerTrans.localRotation.eulerAngles.z);
    }

    void FixedUpdate() {
        if (this.isLocalPlayer) {
            MoveInput();
        } else {
            if (isStartMove) {
                this.playerTrans.position = Vector3.Lerp(this.playerTrans.position, this.toPosition, Time.deltaTime * walkSpeed);
            }
        }
    }

    private void MoveInput() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (IsChangeRotationY(this.playerTrans.rotation.eulerAngles.y)) {
            if (moveCallback != null) {
                moveCallback(this.playerNumber, this.playerTrans.position, this.playerTrans.rotation.eulerAngles.y);
            }
        }

        if (System.Math.Abs(moveHorizontal) > 0f == false && System.Math.Abs(moveVertical) > 0f == false) {
            PlayAnimation(PLAYER_ACTION_TYPE.Idle);
            return;
        }        
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        if (this.cameraTransform != null) {
            movement = this.cameraTransform.TransformDirection(movement);
            movement.y = 0;
        }

        if (this.animationController.IsAiming()) {
            movement = movement * GetAccelerationSpped(this.currentSpeed, this.aimingSpeed, 1f);
        } else {
            bool isRun = Input.GetKey(KeyCode.LeftShift);
            if (isRun) {
                PlayAnimation(PLAYER_ACTION_TYPE.Run);
                movement = movement * GetAccelerationSpped(this.currentSpeed, this.runSpeed, 1f);
            } else {
                PlayAnimation(PLAYER_ACTION_TYPE.Walk);
                movement = movement * GetAccelerationSpped(this.currentSpeed, this.walkSpeed, 1f);
            }
        }        

        this.playerRigidbody.MovePosition(this.playerRigidbody.position + movement * Time.deltaTime);
        this.playerRigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, this.playerRigidbody.velocity.x * -tilt);
        this.lastRotationY = this.playerTrans.rotation.eulerAngles.y;

        if (moveCallback != null) {
            moveCallback(this.playerNumber, this.playerTrans.position, this.playerTrans.rotation.eulerAngles.y);
        }
    }

    private float currentSpeed = 0f;
    private float GetAccelerationSpped(float currentSpeed, float targetSpeed, float accelerationTime) {
        this.currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationTime * Time.deltaTime);
        return this.currentSpeed;
    }

    private float lastRotationY = 0f;
    private const float MinimumRotationY = 3f;
    private bool IsChangeRotationY(float currentRotationY) {
        if (Mathf.Abs(this.lastRotationY - currentRotationY) > MinimumRotationY) {
            this.lastRotationY = currentRotationY;
            return true;
        } else {
            return false;
        }
    }

    private void PlayAnimation(PLAYER_ACTION_TYPE actionType) {
        if (this.currentActionType != actionType) {
            this.currentActionType = actionType;
            this.animationController.OnAcion(actionType);
        }
    }
}
