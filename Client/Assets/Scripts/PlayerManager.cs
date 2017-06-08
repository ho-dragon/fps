using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviourInstance<PlayerManager> {
    public List<Player> remotePlayers;
    public Player localPlayer;

    protected override void _Awake() {
        this.remotePlayers = new List<Player>();
    }

    private void Start() {
        if (this.localPlayer != null) {            
            RayCastGun rayCastGun = localPlayer.weaponGameObject.AddComponent<RayCastGun>();
            this.localPlayer.Init(rayCastGun, 1, "localTestPlayer", null);
			this.localPlayer.EnableCamera(PlayerCamera.inst);
			this.localPlayer.IsPlayable = true;
        }
    }

    public bool IsExsitPlayer(int playerNum) {
        return this.remotePlayers.Exists(x => x.Number == playerNum);
    }

    public void JoinedPlayer(int playerNum, string playerName) {
        if (this.remotePlayers.Exists(x => x.Number == playerNum)) {
            Debug.LogError("[Main.JoinPlayer] already exist player = " + playerNum);
            return;
        }

        GameObject clone = Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity) as GameObject;
        clone.transform.position = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(10, 20), UnityEngine.Random.Range(0, 10));
        Player newPlayer = clone.GetComponent<Player>();
        newPlayer.Init(null//Todo. set weapon from server
                      , playerNum
                      , playerName
                      , null);

        this.remotePlayers.Add(newPlayer);
    }

    public void MovePlayer(int playerNumb, Vector3 movePosition) {
        Player player = this.remotePlayers.Find(x => x.Number == playerNumb);
        if(player != null) {
            if (player.IsPlayable == false) {
                player.ActionController.move.SetMovePosition(movePosition);
            }
        }
    }
}
