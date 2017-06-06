using UnityEngine;
using System.Collections;

public class RayCastGun : Weapon {
    private float distance = 100f;
    public override void Init(Transform muzzleTransform) {
        Debug.Log("[RayCastGun] Init");
        this.muzzleTransform = muzzleTransform;
    }

    public override void Shoot() {
        Debug.Log("[RayCastGun] called Shoot");
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
       // Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
       // Vector3 normalizedVector = 
        Ray ray = new Ray(this.muzzleTransform.position, this.muzzleTransform.forward);
        if (Physics.Raycast(ray, out hit, distance)) {
            Debug.Log("[RayCastGun] Yes! hit detected");
            if(hit.transform.tag.Equals(this.playerTag)){
                Debug.Log("[RayCastGun] Yes! hit detected : Player");
            }
        } else {
            Debug.Log("[RayCastGun] No! hit not detected");
        }
    }
}
