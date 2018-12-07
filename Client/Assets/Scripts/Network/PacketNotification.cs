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
                EndGame(BsonSerializer.Deserialize<UpdateScoreModel>(result.bytes));
                break;
            case _deadPlayer:
                DeadPlayer(BsonSerializer.Deserialize<DeadPlayerModel>(result.bytes));
                break;

        }
    }

    public void DamagedPlayer(DamageModel result) {
        Logger.DebugHighlight("[PacketNotification.DamagedPlayer]");
        PlayerManager.inst.OnDamaged(result);
    }

    public void JoinPlayer(EnterRoomModel result) {
        Logger.DebugHighlight("[PacketNotification.JoinPlayer]");
        PlayerManager.inst.JoinPlayer(result.player, false);
    }

    private void MovePlayer(PlayerMoveModel result) {
        //Logger.DebugHighlight("[RecevieNtotication.MovePlayer]  = " + result.playerNum + " / movePosition X : {0} Y : {1} : Z : {2}" , result.playerPosX, result.playerPosY, result.playerPosZ);
        PlayerManager.inst.OnMove(result.playerNum, new Vector3(result.playerPosX
                                                                    , result.playerPosY
                                                                    , result.playerPosZ)
                                                                    , result.playerYaw);
    }

    private void ActionPlayer(PLayerActionModel result) {
        Logger.DebugHighlight("[PacketNotification.ActionPlayer] actioType = " + result.actionType);
        PlayerManager.inst.OnACtion(result.playerNum, result.actionType);
    }
    
    public void StartGame(GameContextModel result) {
        Logger.DebugHighlight("[PacketNotification.StatGame]");
        Main.inst.StartGame(result);
    }

    public void WaitingPlayer(WaitingStatusModel result) {
        Logger.DebugHighlight("[PacketNotification.WaitingPlayer]");
        UIManager.inst.UpdateWaitingPlayers(result.joinedPlayerCount, result.maxPlayerCount, result.remianTimeToPlay);
    }

    public void UpdateGameTime(GameTimeModel result) {
        Logger.DebugHighlight("[PackketNotification.UpdateGameTime");
        Main.inst.context.UpdatePlayTime(result.playTime);
    }

    public void EndGame(UpdateScoreModel result) {
        Logger.DebugHighlight("[PacketNotification.EndGame");
        UIManager.inst.Alert(string.Format("게임 종료 RED {0} : BLUE {1}", result.scoreRed, result.scoreBlue));
    }

    public void DeadPlayer(DeadPlayerModel result) {
        Logger.DebugHighlight("[PacketNotification.DeadPlayer");

        Player killer = PlayerManager.inst.GetPlayer(result.killerNumber);
        killer.KillCount = result.killerKillCount;
        if (killer.IsLocalPlayer) {
            UIManager.inst.hud.SetKillDeath(killer.KillCount, killer.DeadCount);
        }

        Player deader = PlayerManager.inst.GetPlayer(result.deaderNumber);
        deader.DeadCount = result.deaderDeadCount;
        if (deader.IsLocalPlayer) {
            UIManager.inst.hud.SetKillDeath(deader.KillCount, deader.DeadCount);
        }        
        UIManager.inst.Alert(string.Format("{0}가 {1}를 죽였습니다.", killer.name , deader.name));
        Main.inst.context.UpdateScore(result.scoreRed, result.scoreBlue);
    }
}