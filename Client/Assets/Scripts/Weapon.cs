using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Weapon : MonoBehaviour {
    public Transform muzzleTransform;
    public string playerTag = "Player";
    public string groundTag = "Ground";
    public virtual void Init(Transform muzzleTransform) {
        Debug.Log("[Weapon] Init");
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
