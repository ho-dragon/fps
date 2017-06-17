using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour {
    public Transform muzzleTransform;
    public string playerTag = "Player";
    public string groundTag = "Ground";
	public PlayerCamera playerCam;
	public bool isPlayable = false;
	public bool IsPlayable { set { this.isPlayable = value; } }
    public void Init(Transform muzzleTransform) {
        Debug.Log("[Weapon] Init");
		this.muzzleTransform = muzzleTransform;
    }

	public void SetCamera(PlayerCamera camera) {
		this.playerCam = camera;
	}

    public virtual void Shoot() {
        Debug.Log("[Weapon] shoot");
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }
}
