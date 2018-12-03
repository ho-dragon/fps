using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {
    private Weapon weapon;
    private bool isShootable = false;
    public bool IsShootable {
        get { return this.isShootable && this.weapon.IsShootable(); }
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
