using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public int number = 0;
    public string name = "";
    public TextMesh textMesh;
    public Rigidbody rigidbody;
    private float speed = 3f;
    private float tilt = 1f;

    void Awake() {
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.rigidbody);
    }

    public int Number { get { return this.number; } }
    public string Name { get { return this.name; } }

    public void Init(int number, string name) {
        Debug.Log("[Player] Init number = " + number + " / name = " + name);
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
    }


    void FixedUpdate() {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement = movement * speed;
        rigidbody.MovePosition(this.rigidbody.position + movement * Time.deltaTime);
        rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tilt);
    }
}


