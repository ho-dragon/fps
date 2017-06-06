using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerMove : MonoBehaviour {
    private int playerNumber = 0;
    private bool isPlayable = false;
    private System.Action<int, Vector3> moveCallback;
    private Transform cameraTransform;
    private bool isStartMove = false;
    private Vector3 targetPos;
    private float speed = 3f;
    private float tilt = 1f;
    public Rigidbody rigidbody;

    void Awake() {
        Assert.IsNotNull(this.rigidbody);
    }
    public void Init(int number, System.Action<int, Vector3> moveCallback) {
        this.playerNumber = number;
        this.moveCallback = moveCallback;
    }
    public bool IsPlayable { set { this.isPlayable = value; } }
    public Transform CameraTransform {set { this.cameraTransform = value; } }
    public void SetMovePosition(Vector3 targetPos) {
        this.isStartMove = true;
        this.targetPos = targetPos;
    }

   void FixedUpdate() {
        if (isPlayable) {
            MoveInput();
        } else {
            if (isStartMove) {//Todo. 선형보간 들어가야함
                this.transform.position = Vector3.Lerp(this.transform.position, this.targetPos, Time.deltaTime * speed);
            }
        }
    }

    private void MoveInput() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        if ((moveHorizontal > 0.05f || moveVertical > 0.05f) == false) {
            return;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        if (this.cameraTransform != null) {
            movement = this.cameraTransform.TransformDirection(movement);
            movement.y = 0;
        }

        movement = movement * speed;
        this.rigidbody.MovePosition(this.rigidbody.position + movement * Time.deltaTime);
        this.rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, this.rigidbody.velocity.x * -tilt);

        if (moveCallback != null) {
            moveCallback(this.playerNumber, this.transform.position);
        }
    }

    public void Jump() {

    }
}
