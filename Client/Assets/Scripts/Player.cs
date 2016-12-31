using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public bool isPlayable = false;
    public int number = 0;
    public string name = "";
    public TextMesh textMesh;
    public Rigidbody rigidbody;
    public Transform eyes;
    public Transform camDerection;
    private PlayerCamera cam;
    private Vector3 targetPos;
    private bool isStartMove = false;
    private float speed = 3f;
    private float tilt = 1f;
    public System.Action<int, Vector3> moveCallback;
    void Awake() {
        Assert.IsNotNull(this.camDerection);
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.rigidbody);
    }

    public int Number { get { return this.number; } }
    public string Name { get { return this.name; } }

    public void Init(int number, string name, System.Action<int, Vector3> moveCallback) {
        Debug.Log("[Player] Init number = " + number + " / name = " + name);
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
        this.moveCallback = moveCallback;
    }

    public void AddCamera(PlayerCamera cam) {
        if (this.cam == null) {
            Debug.Log("[Player.AddCamera]");
            this.cam = cam;
            this.cam.target = this.gameObject;
            this.cam.MoveChildTrans(this.eyes);
            this.cam.Look(this.camDerection);
        }
    }

    public void SetMovePosition(Vector3 targetPos) {
        this.isStartMove = true;
        this.targetPos = targetPos;
    }

    void FixedUpdate() {
        if (isPlayable) {
            MoveInput();
        }
        else {
            if (isStartMove) {
                this.transform.position = Vector3.Lerp(this.transform.position, this.targetPos, Time.deltaTime * speed);
            }
        }
    }
    
    private void MoveInput() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        if((moveHorizontal > 0.05f || moveVertical > 0.05f) == false) {
            return;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        if (cam != null) {
            if (cam.target == this.gameObject) {
                movement = cam.transform.TransformDirection(movement);
                movement.y = 0;
            }
        }

        movement = movement * speed;
        this.rigidbody.MovePosition(this.rigidbody.position + movement * Time.deltaTime);
        this.rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tilt);

        if (moveCallback != null) {
            moveCallback(this.number, this.transform.position);
        }
    }
}


