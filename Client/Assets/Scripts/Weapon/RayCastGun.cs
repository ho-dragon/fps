using UnityEngine;
using System.Collections;

public class RayCastGun : Weapon {
    private float distance = 1000f;
    private void Awake() {
        base.fireRate = 1f;
    }

    public override void Shoot() {
        base.Shoot();

        SoundManager.inst.PlayFx(SoundFxType.LazerShoot, this.gameObject);
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
			Logger.Debug("[RayCastGun.Shoot] Yes! hit detected : Layer = " + hit.transform.gameObject.layer.ToString());
            if (hit.transform.tag.Equals(GameLayers.Player)) {
                Player hitPlayer = hit.transform.GetComponent<Player>();
                SoundManager.inst.PlayFx(SoundFxType.HitPlayer, hitPlayer.gameObject);

                if (hitPlayer.IsDead) {
                    Logger.DebugHighlight("[RayCastGun.Shoot] hitPlayer is already dead.");
                    return;
                }

                if (this.ownerPlayerNumber == hitPlayer.Number) {
                    Logger.DebugHighlight("[RayCastGun.Shoot] shoot my body this.ownerPlayerNumber  = {0} hitNumber = {1}", this.ownerPlayerNumber, hitPlayer.Number);
                    return;
                }

                if (hitPlayer.IsSameTeam(GetTeamCdoe())) {
                    Logger.DebugHighlight("[RayCastGun.Shoot] hitPlayer is same team.");
                    return;
                }

                UIManager.inst.hud.HitEffect();
                TcpSocket.inst.Request.Attack(this.ownerPlayerNumber, hitPlayer.Number, 0, (req, result) => {
                    PlayerManager.inst.UpdateHP(result.damagedPlayer, result.currentHP, result.maxHP);
                });
            } else if (hit.transform.gameObject.layer.Equals(GameLayers.Rock)) {
                SoundManager.inst.PlayFx(SoundFxType.HitRock, this.gameObject);
            } else if (hit.transform.gameObject.layer.Equals(GameLayers.Ground)) {
                SoundManager.inst.PlayFx(SoundFxType.HitRock, this.gameObject);
            }
            EffectManager.inst.OnBulletTail(this.muzzleTransform.position, hit.point, 2f);
        } else {
			Logger.Debug("[RayCastGun.Shoot] No! hit not detected");
            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            EffectManager.inst.OnBulletTail(this.muzzleTransform.position, screenHitPoint, 2f);
        }
    }

	private Vector3 GetScreenForwardPoint(float distance) {
		RaycastHit hit;
		Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
		Ray ray = this.playerCam.GetCamera().ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out hit, distance)) {
			Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);
			Logger.Debug("[RayCastGun.GetScreenForwardPoint] Yes! hit detected : Layer = " + hit.transform.gameObject.layer.ToString());
			return hit.point;
		} else {
			Logger.Debug("[RayCastGun.GetScreenForwardPoint] No! hit not detected");
            return ray.GetPoint(distance);
		}
	}
}
