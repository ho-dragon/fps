using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour {
    protected int ownerPlayerNumber;
    protected TeamCode ownerTeamCode;
    public Transform muzzleTransform;
    public ParticleSystem gunFireEffect;
	protected PlayerCamera playerCam;
    protected float fireRate = 0f;
    private float nextFire = 0f;

    private void Awake() {
        Assert.IsNotNull(this.muzzleTransform);
    }

    public void Init(int ownerPlayerNumber, TeamCode teamCode) {
        Logger.Debug("[Weapon] Init : ownerPlayerNumber = " + ownerPlayerNumber);
        this.ownerPlayerNumber = ownerPlayerNumber;
        this.ownerTeamCode = teamCode;
    }

    public void SetTeamCode(TeamCode teamCode) {
        this.ownerTeamCode = teamCode;
    }

    protected TeamCode GetTeamCdoe() {
        return this.ownerTeamCode;
    }

	public void SetCamera(PlayerCamera camera) {
		this.playerCam = camera;
	}
   
    public virtual void Shoot() {
        Logger.Debug("[Weapon] shoot");
        this.nextFire = Time.time + fireRate;
        if (this.gunFireEffect != null) {
            this.gunFireEffect.Play();
        }
    }

    public bool IsShootable() {
        return Time.time > this.nextFire;
    }
}
