using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {
    public AudioSource audioSource;
    private Weapon weapon;
    private bool isShootable = false;
    public bool IsShootable {
        get { return this.isShootable; }
    }

    public void Init(Weapon weapon) {
        this.weapon = weapon;
        this.isShootable = true;
    }
    
    public void SetWeapon(Weapon weapon) {
        this.weapon = weapon;
        this.isShootable = true;
    }

    public void RemoveWeapon() {
        this.isShootable = false;
        if (this.weapon != null) {
            this.weapon = null;
        }        
    }

    public void Shoot() {
        if (this.weapon != null) {
            this.weapon.Shoot();
        }        
    }
}
