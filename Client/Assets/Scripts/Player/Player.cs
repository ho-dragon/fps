using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {    
    public TextMesh textMesh;
    public Transform eyes;
    public Transform camDerection;
    public Transform muzzleTransform;
    public PlayerActionController actionController;
    public Weapon weapon;//테스트용으로 직접 붙여놓음
    public HpGage hpBar;
    private bool isLocalPlayer = false;
    private PlayerCamera playerCameara;
    private int teamCode = 0;
    private int number = 0;
    private string name = "";
    public int Number { get { return this.number; } }
    public PlayerActionController ActionController { get { return this.actionController; } }
    public bool IsLocalPlayer {
        get { return this.isLocalPlayer; }
        set {
            this.isLocalPlayer = value;
            this.actionController.SetLocalPlayer(value);
        }
    }

    void Awake() {
        Assert.IsNotNull(this.actionController);
        Assert.IsNotNull(this.camDerection);
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.muzzleTransform);
        Assert.IsNotNull(this.hpBar);
    }
    
    public void Init(int teamCode, int number, string name, float currentHP, float maxHP, System.Action<int, Vector3> moveCallback) {
        Logger.Debug("[Player] Init number = " + number + " / name = " + name);
        this.teamCode = teamCode;
        this.number = number;
        this.name = name;
        this.textMesh.text = name;
		this.actionController.Init(this.transform, number, moveCallback);
        SetHealth(currentHP, maxHP);
        SetWeapon(this.weapon, number);        
    }

    public void SetWeapon(Weapon weapon, int ownerPlayerNumber) {
        this.weapon = weapon;        
        this.weapon.Init(ownerPlayerNumber);
        this.actionController.SetWeapon(weapon);
    }

    public bool IsSameTeam(int teamCode) {
        return this.teamCode == teamCode;
    }

    public void SetHealth(float currentHP, float maxHP) {
        if (this.isLocalPlayer) {
            UIManager.inst.PlayerHP(currentHP, maxHP);
        }
        this.hpBar.SetHP(currentHP, maxHP);
    }

    public void SetCamera(PlayerCamera playerCameara) {
        if (this.playerCameara == null) {
            Logger.Debug("[Player.AddCamera]");
            this.playerCameara = playerCameara;
            this.playerCameara.target = this.gameObject;
            this.playerCameara.MoveChildTrans(this.eyes);
            this.playerCameara.Look(this.camDerection);
            this.actionController.SetCamera(playerCameara.transform);
			if (this.weapon != null){
				this.weapon.SetCamera(playerCameara);
			}
        }
    }
}


