using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

public class PacketNotification {
    private const string notiMovePlayer = "movePlayer";
    private const string notiDemagedPlayer = "damagedPlayer";
    private const string notiJoinPlayer = "joinPlayer";
    private const string notiActionPlayer = "actionPlayer";

    public void RecevieNotification(ResponseFormat result) {        
        switch(result.method){
            case notiDemagedPlayer:
                DamagedPlayer(BsonSerializer.Deserialize<DamageModel>(result.bytes));
                break;
            case notiJoinPlayer:
                JoinPlayer(BsonSerializer.Deserialize<EnterRoomModel>(result.bytes));
                break;
            case notiMovePlayer:
                MovePlayer(BsonSerializer.Deserialize<PlayerMoveModel>(result.bytes));
                break;
            case notiActionPlayer:
                ActionPlayer(BsonSerializer.Deserialize<PLayerActionModel>(result.bytes));
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