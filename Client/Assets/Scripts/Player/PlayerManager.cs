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

    public bool IsExsitPlayer(int playerNum) {
        return this.remotePlayers.Exists(x => x.Number == playerNum);
    }

    public void JoinedPlayer(PlayerModel player, bool isLocalPlayer) {
        if (player == null) {
            Logger.Error("[PlayerManager.JoinedPlayer] player is null");
            return;
        }

        GameObject clone = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        if (player.lastPosition != null) {
            clone.transform.position = new Vector3(player.lastPosition[0], player.lastPosition[1], player.lastPosition[2]);
        } else {
            clone.transform.position = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(10, 20), UnityEngine.Random.Range(0, 10));
        }        

        Player newPlayer = clone.GetComponent<Player>();
        //newPlayer.hpBar.facing.SetCamara(PlayerCamera.inst.camera);//HP카메라 보는 부분 일단 주석

        if (isLocalPlayer) {
            this.localPlayer = newPlayer;
            this.localPlayer.AttachCamera();
            this.localPlayer.IsLocalPlayer = true;
            Logger.DebugHighlight("[PlayerManager.JoinedPlayer] added local player / name  = {0} / number = {1}", player.name, player.number);
        } else {
            if (this.remotePlayers.Exists(x => x.Number == player.number)) {
                Logger.Error("[Main.JoinPlayer] already exist player = " + player.number);
                return;
            }
            this.remotePlayers.Add(newPlayer);
            Logger.DebugHighlight("[PlayerManager.JoinedPlayer] added remote player / name  = {0} / number = {1}", player.name, player.number);
        }

        newPlayer.Init(player.teamCode
             , player.number
             , player.name
             , player.currentHP
             , player.maxHP
             , (number, movePos, roationY) => {
                 TcpSocket.inst.Request.MovePlayer(number, movePos, roationY);
             });        
    }

    public void OnMove(int playerNumb, Vector3 movePosition, float yaw) {
        Player player = this.remotePlayers.Find(x => x.Number == playerNumb);
        if (player != null) {
            if (player.IsLocalPlayer == false) {                
                player.ActionController.OnMove(movePosition, yaw);
            }
        }
    }

    public void OnACtion(int playerNumb, PLAYER_ACTION_TYPE actionType) {
        Player player = this.remotePlayers.Find(x => x.Number == playerNumb);
        if (player != null) {
            if (player.IsLocalPlayer == false) {
                player.ActionController.OnAction(actionType);
            }
        }
    }

    public void OnDamaged(DamageModel result) {
        Logger.DebugHighlight("[PlayerManager.DamagedPlayer]--------result / damagedPlayerNumb = " + result.damagedPlayer);
        if (this.localPlayer.Number == result.damagedPlayer) {
            Logger.DebugHighlight("[PlayerManager.DamagedPlayer]--------SetLocalHP / damagedPlayerNumb = " + result.damagedPlayer);
            this.localPlayer.SetHealth(result.currentHP, result.maxHP);
            return;
        }

        Player player = this.remotePlayers.Find(x => x.Number == result.damagedPlayer);
        if (player == null) {
            Logger.Error("[PlayerManager.SetDamage] player is null");
            return;
        }
        Logger.DebugHighlight("[PlayerManager.DamagedPlayer]--------SetRemoteHP / damagedPlayerNumb = " + result.damagedPlayer);
        player.SetHealth(result.currentHP, result.maxHP);
    }
}