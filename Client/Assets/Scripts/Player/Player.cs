using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    private bool isPlayable = false;
    public int number = 0;
    private int teamCode = 0;
    public string name = "";
    public TextMesh textMesh;
    public Transform eyes;
    public Transform camDerection;
    public Transform muzzleTransform;
    private PlayerCamera cam;
    public Weapon weapon;
    public PlayerActionController actionController;
    private PlayerStatus status;
    public HpGage hpBar;

    void Awake() {
        Assert.IsNotNull(this.actionController);
        Assert.IsNotNull(this.camDerection);
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.muzzleTransform);
        Assert.IsNotNull(this.hpBar);
    }
    public bool IsSameTeam(int teamCode) {
        return this.teamCode == teamCode;
    }

    public bool IsPlayable {
        get { return this.isPlayable; }
        set { this.isPlayable = value;
            this.actionController.move.IsPlayable = value;
			if(this.weapon != null) {
				this.weapon.IsPlayable = value;
			}			
        }
    }
    public int Number { get { return this.number; } }
    public string Name { get { return this.name; } }
    public PlayerActionController ActionController { get { return this.actionController; } }
    public void Init(int teamCode, int number, string name, float currentHP, float maxHP, System.Action<int, Vector3> moveCallback) {
        Logger.Debug("[Player] Init number = " + number + " / name = " + name);
        this.teamCode = teamCode;
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
		this.actionController.move.Init(this.transform, number, moveCallback);
        this.status = new PlayerStatus(currentHP, maxHP);
        SetHealth(currentHP, maxHP);

        if (weapon != null) {
            this.weapon.Init(this.number, this.muzzleTransform);
        }
        //this.actionController.shoot.Init(weapon);
    }

    public void SetHealth(float currentHP, float maxHP) {
        if(this.isPlayable) {
            UIManager.inst.hud.hp.SetHP(currentHP, maxHP);
        }
        this.hpBar.SetHP(currentHP, maxHP);
    }

    public void EnableCamera(PlayerCamera cam) {
        if (this.cam == null) {
            Logger.Debug("[Player.AddCamera]");
            this.cam = cam;
            this.cam.target = this.gameObject;
            this.cam.MoveChildTrans(this.eyes);
            this.cam.Look(this.camDerection);
            this.actionController.move.CameraTransform = this.cam.transform;
			if(this.weapon != null){
				this.weapon.SetCamera(cam);
			}
        }
    }
}


