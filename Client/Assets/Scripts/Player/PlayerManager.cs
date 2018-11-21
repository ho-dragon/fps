using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviourInstance<PlayerManager> {
    public GameObject playerPrefab;
    public List<Player> remotePlayers;
    public Player localPlayer;
  
    protected override void _Awake() {
        Assert.IsNotNull(this.playerPrefab);
        this.remotePlayers = new List<Player>();
    }

    private void Start() {
        if (this.localPlayer != null) {//Test용     
            RayCastGun rayCastGun = localPlayer.weaponGameObject.AddComponent<RayCastGun>();
            //this.localPlayer.Init(0, rayCastGun, 1, "localTestPlayer", 100, 100, null);
            //this.localPlayer.EnableCamera(PlayerCamera.inst);
            //this.localPlayer.IsPlayable = true;
        }
    }

    public bool IsExsitPlayer(int playerNum) {
        return this.remotePlayers.Exists(x => x.Number == playerNum);
    }

    public void JoinedPlayer(PlayerModel player, bool isLocalPlayer) {
        if (player == null) {
            Logger.Error("[PlayerManager.JoinedPlayer] player is null");
            return;
        }

        GameObject clone = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (clone == null) {
            Logger.Error("[PlayerManager.JoinedPlayer] clone is null");
            return;
        }

        if (player.lastPosition != null) {
            clone.transform.position = new Vector3(player.lastPosition[0], player.lastPosition[1], player.lastPosition[2]);
        } else {
            clone.transform.position = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(10, 20), UnityEngine.Random.Range(0, 10));
        }
        
        Player newPlayer = clone.GetComponent<Player>();
        newPlayer.Init(player.teamCode
                                     ,  null
                                     , player.number
                                     , player.name
                                     , player.currentHP
                                     , player.maxHP
                                     , (number, movePos) => {
                                         TcpSocket.inst.client.MovePlayer(number, movePos);
                                     });

        if (isLocalPlayer) {
            this.localPlayer = newPlayer;
            this.localPlayer.EnableCamera(PlayerCamera.inst);
            this.localPlayer.IsPlayable = true;
            localPlayer.weaponGameObject.AddComponent<RayCastGun>();
            return;
        }

         if (this.remotePlayers.Exists(x => x.Number == player.number)) {
             Logger.Error("[Main.JoinPlayer] already exist player = " + player.number);
             return;
         }

        newPlayer.hpBar.facing.SetCamara(PlayerCamera.inst.camera);
        this.remotePlayers.Add(newPlayer);
    }

    public void MovePlayer(int playerNumb, Vector3 movePosition) {
        Player player = this.remotePlayers.Find(x => x.Number == playerNumb);
        if (player != null) {
            if (player.IsPlayable == false) {
                Logger.DebugHighlight("[PlayerManager.MovePlayer] playerNumb = " + playerNumb);
                player.ActionController.move.SetMovePosition(movePosition);
            }
        }
    }

    public void DamagedPlayer(DamageModel result) {
        if (this.localPlayer.Number == result.damagedPlayer) {
            Logger.Debug("내가 맞았다");
            this.localPlayer.SetHealth(result.currentHP, result.maxHP);
            return;
        }
        Player player = this.remotePlayers.Find(x => x.Number == result.damagedPlayer);
        if (player == null) {
            Logger.Error("[PlayerManager.SetDamage] player is null");
            return;
        }
        player.SetHealth(result.currentHP, result.maxHP);
    }
}