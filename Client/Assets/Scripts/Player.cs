using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public int number = 0;
    public string name = "";
    public TextMesh textMesh;

    void Awake() {
        Assert.IsNotNull(this.textMesh);
    }

    public int Number { get { return this.number; } }
    public string Name { get { return this.name; } }

    public void Init(int number, string name) {
        Debug.Log("[Player] Init number = " + number + " / name = " + name);
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
    }
}
