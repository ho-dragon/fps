using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

public class PacketNotification {
    private const string _movePlayer = "movePlayer";
    private const string _damagedPlayer = "damagedPlayer";
    private const string _joinPlayer = "joinPlayer";
    private const string _actionPlayer = "actionPlayer";
    private const string _startGame = "startGame";
    private const string _waitingPlayer = "waitingPlayer";
    private const string _updateGameTime = "updateGameTime";
    private const string _endGame = "_endGame";

    public void RecevieNotification(ResponseFormat result) {        
        switch(result.method){
            case _damagedPlayer:
                DamagedPlayer(BsonSerializer.Deserialize<DamageModel>(result.bytes));
                break;
            case _joinPlayer:
                JoinPlayer(BsonSerializer.Deserialize<EnterRoomModel>(result.bytes));
                break;
            case _movePlayer:
                MovePlayer(BsonSerializer.Deserialize<PlayerMoveModel>(result.bytes));
                break;
            case _actionPlayer:
                ActionPlayer(BsonSerializer.Deserialize<PLayerActionModel>(result.bytes));
                break;
            case _startGame:

                break;
            case _waitingPlayer:

                break;
            case _updateGameTime:

                break;

            case _endGame:
                break;

        }
    }

    public void DamagedPlayer(DamageModel result) {
        Logger.DebugHighlight("[RecevieNtotication.DamagedPlayer]");
        PlayerManager.inst.OnDamaged(result);
    }

    public void JoinPlayer(EnterRoomModel result) {
        Logger.DebugHighlight("[RecevieNtotication.JoinPlayer]");
        PlayerManager.inst.JoinedPlayer(result.player, false);
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
}