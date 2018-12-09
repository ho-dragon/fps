using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {
    public PlayerActionController actionController;
    public PlayerAnimationController animationController;
    public PlayerHeaderUI headerUI;
    private Weapon weapon;
    private bool isLocalPlayer = false;
    private TeamCode teamCode;
    private int number = 0;
    private string nickName = "";
    private bool isDead =false;
    private int killCount = 0;
    private int deadCount = 0;
    public Transform rightGunBone;
    public Transform leftGunBone;
    public WeaponModel[] weaponModels;

    public int Number { get { return this.number; } }
    public string NickName { get { return this.nickName; } }
    public PlayerActionController ActionController { get { return this.actionController; } }

    public bool IsLocalPlayer {
        get { return this.isLocalPlayer; }
        set {
            this.isLocalPlayer = value;
            this.actionController.SetLocalPlayer(value);
        }
    }

    public bool IsDead {
        get {return this.isDead; }
        set {this.isDead = value;
            this.actionController.UpdatePlayerDead(value);
            if (value) {
                animationController.OnAcion(PlayerActionType.Death);
            }
        }
    }

    public int KillCount {
        get { return this.killCount; }
        set { this.killCount = value; }
    }

    public int DeadCount {
        get { return this.deadCount; }
        set { this.killCount = value; }
    }

    public TeamCode GetTeamCode() {
        return this.teamCode;
    }

    void Awake() {
        Assert.IsNotNull(this.actionController);
        Assert.IsNotNull(this.headerUI);        
    }
        
    public void Init(bool isLocalPlayer, TeamCode teamCode, int number, string nickName, float currentHP, float maxHP, bool isDead, int killCount, int deadCount, System.Action<int, Vector3, float> moveCallback) {
        Logger.Debug("[Player] Init number = " + number + " / name = " + nickName);
        this.isLocalPlayer = isLocalPlayer;
        this.teamCode = teamCode;
        this.number = number;
        this.nickName = nickName;
        this.isDead = isDead;
        this.killCount = killCount;
        this.deadCount = deadCount;
        if (this.isLocalPlayer) {
            this.headerUI.gameObject.SetActive(false);
            UIManager.inst.hud.SetName(nickName);
        } else {
            this.headerUI.SetCamera(CameraController.inst.playerCamera.GetCamera());
            this.headerUI.gameObject.SetActive(true);
            this.headerUI.SetNickName(nickName);
            this.headerUI.SetTeamCode(teamCode);
        }
        UpdateHP(currentHP, maxHP);
        AttachWeapon(number, "Rifle");//최초 라이플을 들고있도록
        this.actionController.Init(this.animationController, this.transform, number, isLocalPlayer, isDead, moveCallback);
        this.actionController.SetLocalPlayer(isLocalPlayer);
    }

    public void Respawn() {
        this.transform.position = MapInfo.inst.GetRespawnZone(this.teamCode);
        TcpSocket.inst.Request.MovePlayer(this.number, this.transform.position, this.transform.rotation.eulerAngles.y);
        animationController.OnAcion(PlayerActionType.Idle);
    }

    public void SetWeapon(Weapon weapon, int ownerPlayerNumber) {
        this.weapon = weapon;        
        this.weapon.Init(ownerPlayerNumber, this.teamCode);
        this.actionController.SetWeapon(weapon); 
    }

    public void AssignTeamCode(bool isLocalPlayer, TeamCode teamCode) {
        this.teamCode = teamCode;
        if (this.weapon != null) {
            this.weapon.SetTeamCode(teamCode);
        }

        if (isLocalPlayer == false) {
            this.headerUI.SetTeamCode(teamCode);
        }
    }

    public bool IsSameTeam(TeamCode teamCode) {
        return this.teamCode == teamCode;
    }

    public void UpdateHP(float currentHP, float maxHP) {
        if (this.isLocalPlayer) {
            UIManager.inst.hud.UpdateHP(currentHP, maxHP);
        } else {
            this.headerUI.SetHealth(currentHP, maxHP);
        }        
    }

    public void AttachCamera() {
        CameraController.inst.AttatchCameraToPlayer(this.transform);
        this.actionController.SetCamera(CameraController.inst.playerCamera.transform);
		if (this.weapon != null){
			this.weapon.SetCamera(CameraController.inst.playerCamera);
		}
    }

    private void AttachWeapon(int ownerNumber, string weaponName) {
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
                    SetWeapon(newRightGun.GetComponent<Weapon>(), ownerNumber);
                }
                if (hand.leftGun != null) {
                    GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
                    newLeftGun.transform.parent = leftGunBone;
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    SetWeapon(newLeftGun.GetComponent<Weapon>(), ownerNumber);
                }
                this.animationController.animator.runtimeAnimatorController = hand.controller;
                return;
            }
        }
    }
}


