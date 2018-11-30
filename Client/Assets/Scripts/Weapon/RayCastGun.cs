﻿using UnityEngine;
using System.Collections;

public class RayCastGun : Weapon {
    private float distance = 1000f;

    public override void Shoot() {
		Logger.Debug("[RayCastGun] called Shoot");
		if(this.playerCam == null) {
			Logger.Debug("[RayCastGun] playerCam is null");
			return;
		}
		Vector3 screenHitPoint = GetScreenForwardPoint(this.distance);
		Vector3 normalizedDirection = Vector3.Normalize(screenHitPoint - this.muzzleTransform.position);
		RaycastHit hit;
		Ray ray = new Ray(this.muzzleTransform.position, normalizedDirection);
		if (Physics.Raycast(ray, out hit, distance)) {
            Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);
			Logger.Debug("[RayCastGun.Shoot] Yes! hit detected : Tag = " + hit.transform.tag.ToString());
            if (hit.transform.tag.Equals(playerTag)) {
                Player hitPlayer = hit.transform.GetComponent<Player>();
                if(this.ownerPlayerNumber == hitPlayer.Number) {
                    Logger.DebugHighlight("[RayCastGun.Shoot] shoot my body");
                    return;
                }
                TcpSocket.inst.Request.Attack(this.ownerPlayerNumber, hitPlayer.Number, 0, (req, result) => {
                    PlayerManager.inst.OnDamaged(result);
                });
            }
            EffectManager.inst.OnBulletTail(this.muzzleTransform.position, hit.point, 2f);
        } else {
            EffectManager.inst.OnBulletTail(this.muzzleTransform.position, normalizedDirection * distance, 2f);
            Logger.Debug("[RayCastGun.Shoot] No! hit not detected");
		}
    }

	private Vector3 GetScreenForwardPoint(float distance) {
		RaycastHit hit;
		Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
		Ray ray = this.playerCam.GetCamera().ScreenPointToRay(screenCenter);
		if (Physics.Raycast(ray, out hit, distance)) {
			Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);
			Logger.Debug("[RayCastGun.GetScreenForwardPoint] Yes! hit detected : Tag = " + hit.transform.tag.ToString());
			return hit.point;
		} else {
			Logger.Debug("[RayCastGun.GetScreenForwardPoint] No! hit not detected");
			return Vector3.zero; 
		}
	}
}
