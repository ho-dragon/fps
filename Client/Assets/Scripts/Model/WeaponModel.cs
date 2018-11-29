using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponModel {
    public string name;
    public GameObject rightGun;
    public GameObject leftGun;
    public RuntimeAnimatorController controller;
}
