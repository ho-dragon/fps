using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

public class PacketNotification {
    private const string _MovePlayer = "movePlayer";
    private const string _DamagedPlayer = "damagedPlayer";
    private const string _JoinPlayer = "joinPlayer";
    private const string _ActionPlayer = "actionPlayer";
    private const string _StartGame = "startGame";
    private const string _WaitingPlayer = "waitingPlayer";
    private const string _UpdateGameTime = "updateGameTime";
    private const string _EndGame = "endGame";
    private const string _deadPlayer = "deadPlayer";
    private const string _respawn = "respawn";

    public void RecevieNotification(ResponseFormat result) {        
        switch(result.method){
            case _DamagedPlayer:
                DamagedPlayer(BsonSerializer.Deserialize<DamageModel>(result.bytes));
                break;
            case _JoinPlayer:
                JoinPlayer(BsonSerializer.Deserialize<EnterRoomModel>(result.bytes));
                break;
            case _MovePlayer:
                MovePlayer(BsonSerializer.Deserialize<PlayerMoveModel>(result.bytes));
                break;
            case _ActionPlayer:
                ActionPlayer(BsonSerializer.Deserialize<PLayerActionModel>(result.bytes));
                break;
            case _StartGame:
                StartGame(BsonSerializer.Deserialize<GameContextModel>(result.bytes));
                break;
            case _WaitingPlayer:
                WaitingPlayer(BsonSerializer.Deserialize<WaitingStatusModel>(result.bytes));
                break;
            case _UpdateGameTime:
                UpdateGameTime(BsonSerializer.Deserialize<GameTimeModel>(result.bytes));
                break;                
            case _EndGame:
                EndGame(BsonSerializer.Deserialize<GameContextModel>(result.bytes));
                break;
            case _deadPlayer:
                DeadPlayer(BsonSerializer.Deserialize<DeadPlayerModel>(result.bytes));
                break;
            case _respawn:
                Respawn(BsonSerializer.Deserialize<RespawnModel>(result.bytes));
                break;

        }
    }

    public void DamagedPlayer(DamageModel result) {
        PlayerManager.inst.UpdateHP(result.damagedPlayer, result.currentHP, result.maxHP);
    }

    public void JoinPlayer(EnterRoomModel result) {
        PlayerManager.inst.JoinPlayer(result.player, false);
    }

    private void MovePlayer(PlayerMoveModel result) {
        PlayerManager.inst.UpdatePosition(result.playerNum, new Vector3(result.playerPosX
                                                                    , result.playerPosY
                                                                    , result.playerPosZ)
                                                                    , result.playerYaw);
    }

    private void ActionPlayer(PLayerActionModel result) {
        PlayerManager.inst.UpdateAction(result.playerNum, result.actionType);
    }
    
    public void StartGame(GameContextModel result) {
        Main.inst.StartGame(false, result);
    }

    public void WaitingPlayer(WaitingStatusModel result) {
        UIManager.inst.UpdateWaitingPlayers(result.joinedPlayerCount, result.maxPlayerCount, result.remianTimeToPlay);
    }

    public void UpdateGameTime(GameTimeModel result) {
        if (Main.inst.context != null) {
            Main.inst.context.UpdateRemainTime(result.remainTime);
        }                   
    }

    public void DeadPlayer(DeadPlayerModel result) {
        Player killer = PlayerManager.inst.GetPlayer(result.lastDamageInfo.attackPlayer);
        killer.KillCount = result.killerKillCount;
        if (killer.IsLocalPlayer) {
            UIManager.inst.hud.SetKillDeath(killer.KillCount, killer.DeadCount);
        }

        Player deader = PlayerManager.inst.GetPlayer(result.lastDamageInfo.damagedPlayer);
        deader.IsDead = true;
        deader.DeadCount = result.deaderDeadCount;
        deader.UpdateHP(result.lastDamageInfo.currentHP, result.lastDamageInfo.maxHP);
        
        if (deader.IsLocalPlayer) {
            UIManager.inst.hud.SetKillDeath(deader.KillCount, deader.DeadCount);
            UIManager.inst.ShowToastMessgae(string.Format("{0}에 의해 죽었습니다. 5초 후 부활합니다.", killer.NickName, deader.NickName), 5f);
        } else {
            UIManager.inst.ShowToastMessgae(string.Format("{0}가 {1}를 죽였습니다.", killer.NickName, deader.NickName), 3f);
        }
        Main.inst.context.UpdateScore(result.scoreRed, result.scoreBlue);
    }

    public void Respawn(RespawnModel result) {
        Player player = PlayerManager.inst.GetPlayer(result.playerNumber);
        UIManager.inst.ShowToastMessgae(string.Format("{0}팀 {1}이(가) 부활했습니다.", player.GetTeamCode().GetTeamName(), player.NickName), 3f);
        player.UpdateHP(result.currentHP, result.maxHP);
        player.IsDead = false;
        if (player.IsLocalPlayer) {
            player.Respawn();
        }
    }

    public void EndGame(GameContextModel result) {
        Main.inst.EndGame(result);
    }
}