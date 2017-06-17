using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {
    private bool isPlayable = false;
    public bool IsPlayable { set { this.isPlayable = value; } }
    private Weapon weapon;
    public void Init(Weapon weapon) {
        this.weapon = weapon;
    }
    public void SetWeapon(Weapon weapon) {
        this.weapon = weapon;
    }
}
