﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour {
    public int ownerPlayerNumber;
    public Transform muzzleTransform;
    public string playerTag = "Player";
    public string groundTag = "Ground";
	protected PlayerCamera playerCam;

    private void Awake() {
        Assert.IsNotNull(this.muzzleTransform);
    }

    public void Init(int ownerPlayerNumber) {
        Logger.Debug("[Weapon] Init");
        this.ownerPlayerNumber = ownerPlayerNumber;
    }

	public void SetCamera(PlayerCamera camera) {
		this.playerCam = camera;
	}
   
    public virtual void Shoot() {
        Logger.Debug("[Weapon] shoot");
    }

    void Update() {
        if (this.playerCam != null) {
            this.transform.forward = -this.playerCam.transform.forward;
        }
    }
}
