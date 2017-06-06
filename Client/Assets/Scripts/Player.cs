using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    private bool isPlayable = false;
    public int number = 0;
    public string name = "";
    public TextMesh textMesh;
    public Transform eyes;
    public Transform camDerection;
    public Transform muzzleTransform;
    public GameObject weaponGameObject;
    private PlayerCamera cam;
    private Weapon weapon;
    public PlayerActionController actionController;

    void Awake() {
        Assert.IsNotNull(this.actionController);
        Assert.IsNotNull(this.camDerection);
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.muzzleTransform);
        Assert.IsNotNull(this.weaponGameObject);
    }

    public bool IsPlayable {
        get { return this.isPlayable; }
        set { this.isPlayable = value;
            this.actionController.move.IsPlayable = value;
        }
    }
    public int Number { get { return this.number; } }
    public string Name { get { return this.name; } }
    public PlayerActionController ActionController { get { return this.actionController; } }
    public void Init(Weapon weapon, int number, string name, System.Action<int, Vector3> moveCallback) {
        Debug.Log("[Player] Init number = " + number + " / name = " + name);
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
        if (weapon != null) {
            this.weapon = weapon;
            this.weapon.Init(this.muzzleTransform);
        }
        this.actionController.move.Init(number, moveCallback);
        //this.actionController.shoot.Init(weapon);
    }

    public void EnableCamera(PlayerCamera cam) {
        if (this.cam == null) {
            Debug.Log("[Player.AddCamera]");
            this.cam = cam;
            this.cam.target = this.gameObject;
            this.cam.MoveChildTrans(this.eyes);
            this.cam.Look(this.camDerection);
            this.actionController.move.CameraTransform = this.cam.transform;
        }
    }
}


