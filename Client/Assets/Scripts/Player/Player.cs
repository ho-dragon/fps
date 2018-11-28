using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public TextMesh textMesh;
    public Transform cameraPivot;
    public Transform muzzleTransform;
    public PlayerActionController actionController;
    public PlayerAnimationController animationController;
    public Weapon weapon;//테스트용으로 직접 붙여놓음
    public HpGage hpBar;
    private bool isLocalPlayer = false;
    private int teamCode = 0;
    private int number = 0;
    private string name = "";
    public Transform rightGunBone;
    public Transform leftGunBone;
    public WeaponModel[] weaponModels;
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
        Assert.IsNotNull(this.textMesh);
        Assert.IsNotNull(this.muzzleTransform);
        Assert.IsNotNull(this.hpBar);
        InitWeapon("Rifle");//최초 라이플을 들고있도록
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

    private void InitWeapon(string weaponName) {
        foreach (WeaponModel hand in weaponModels) {
            if (hand.name == weaponName) {
                if (rightGunBone.childCount > 0)
                    Destroy(rightGunBone.GetChild(0).gameObject);
                if (leftGunBone.childCount > 0)
                    Destroy(leftGunBone.GetChild(0).gameObject);
                if (hand.rightGun != null) {
                    GameObject newRightGun = (GameObject)Instantiate(hand.rightGun);
                    newRightGun.transform.parent = rightGunBone;
                    newRightGun.transform.localPosition = Vector3.zero;
                    newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                if (hand.leftGun != null) {
                    GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
                    newLeftGun.transform.parent = leftGunBone;
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                this.animationController.GetAnimator.runtimeAnimatorController = hand.controller;
                return;
            }
        }
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

    public void AttachCamera() {
            Logger.Debug("[Player.AddCamera]");
            CameraController.inst.AttatchCameraToPlayer(this.transform);
            this.actionController.SetCamera(CameraController.inst.playerCamera.transform);
			if (this.weapon != null){
				this.weapon.SetCamera(CameraController.inst.playerCamera);
			}
    }
}


