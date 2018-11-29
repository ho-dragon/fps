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

        if (System.Math.Abs(moveHorizontal) > 0f == false && System.Math.Abs(moveVertical) > 0f == false) {
            PlayAnimation(PLAYER_ACTION_TYPE.Idle);
            return;
        }        
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        if (this.cameraTransform != null) {
            movement = this.cameraTransform.TransformDirection(movement);
            movement.y = 0;
        }
        bool isRun = Input.GetKey(KeyCode.LeftShift);
        if (isRun) {
            PlayAnimation(PLAYER_ACTION_TYPE.Run);
            movement = movement * runSpeed;
        } else {
            PlayAnimation(PLAYER_ACTION_TYPE.Walk);
            movement = movement * walkSpeed;
        }

        this.playerRigidbody.MovePosition(this.playerRigidbody.position + movement * Time.deltaTime);
        this.playerRigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, this.playerRigidbody.velocity.x * -tilt);

        if (moveCallback != null) {
            moveCallback(this.playerNumber, this.playerTrans.position, this.transform.localRotation.y);
        }
    }

    private void PlayAnimation(PLAYER_ACTION_TYPE actionType) {
        if (this.currentActionType != actionType) {
            this.currentActionType = actionType;
            this.animationController.OnAcion(actionType);
        }
    }
}
