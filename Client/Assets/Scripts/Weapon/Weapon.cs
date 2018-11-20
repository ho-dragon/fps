using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour {
    public int playerNum;
    public Transform muzzleTransform;
    public string playerTag = "Player";
    public string groundTag = "Ground";
	public PlayerCamera playerCam;
	public bool isPlayable = false;
	public bool IsPlayable { set { this.isPlayable = value; } }
    public void Init(int playerNum, Transform muzzleTransform) {
        Logger.Debug("[Weapon] Init");
		this.muzzleTransform = muzzleTransform;
        this.playerNum = playerNum;
    }

	public void SetCamera(PlayerCamera camera) {
		this.playerCam = camera;
	}

    public virtual void Shoot() {
        Logger.Debug("[Weapon] shoot");
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }
}
