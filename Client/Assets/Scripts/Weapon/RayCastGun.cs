using UnityEngine;
using System.Collections;

public class RayCastGun : Weapon {
    private float distance = 100f;
    public override void Shoot() {
		Debug.Log("[RayCastGun] called Shoot");
		if(this.playerCam == null) {
			Debug.Log("[RayCastGun] playerCam is null");
			return;
		}
		Vector3 screenHitPoint = GetScreenForwardPoint(this.distance);
		Vector3 normalizedDirection = Vector3.Normalize(screenHitPoint - this.muzzleTransform.position);
		RaycastHit hit;
		Ray ray = new Ray(this.muzzleTransform.position, normalizedDirection);
		if (Physics.Raycast(ray, out hit, distance)) {
			Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);
			Debug.Log("[RayCastGun.Shoot] Yes! hit detected : Tag = " + hit.transform.tag.ToString());
            if(hit.transform.tag.Equals(playerTag)) {
                Player hitPlayer = hit.transform.GetComponent<Player>();
                TcpSocket.inst.client.Attack(this.playerNum, hitPlayer.Number, 0, (req, result) => {
                    PlayerManager.inst.DamagedPlayer(result);
                });
            }
		} else {
			Debug.Log("[RayCastGun.Shoot] No! hit not detected");
		}
    }

	private Vector3 GetScreenForwardPoint(float distance) {
		RaycastHit hit;
		Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
		Ray ray = this.playerCam.camera.ScreenPointToRay(screenCenter);
		if (Physics.Raycast(ray, out hit, distance)) {
			Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);
			Debug.Log("[RayCastGun.GetScreenForwardPoint] Yes! hit detected : Tag = " + hit.transform.tag.ToString());
			return hit.point;
		} else {
			Debug.Log("[RayCastGun.GetScreenForwardPoint] No! hit not detected");
			return Vector3.zero; 
		}
	}
}
