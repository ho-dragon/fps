﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketRequest {
    private PacketManager packetManager;
    public PacketRequest(PacketManager packetManager) {
        this.packetManager = packetManager;
    }

    public void EnterRoom(string playerName, PacketManager.Response<EnterRoomModel> callback) {
        this.packetManager.Send<EnterRoomModel>(this.packetManager.CreateRequestFormat("enterRoom", "playerName", playerName), callback);
    }

    public void Attack(int attackPlayer, int damagedPlayer, int attackPosition, PacketManager.Response<DamageModel> callback) {
        this.packetManager.Send<DamageModel>(this.packetManager.CreateRequestFormat("attackPlayer", "attackPlayer", attackPlayer
                                      , "damagedPlayer", damagedPlayer
                                      , "attackPosition", attackPosition), callback);
    }

    public void MovePlayer(int playerNum, Vector3 playerPos) {
        this.packetManager.Send<PlayerMoveModel>(this.packetManager.CreateRequestFormat("movePlayer", "playerNum", playerNum
                                              , "playerPosX", playerPos.x
                                              , "playerPosY", playerPos.y
                                              , "playerPosZ", playerPos.z), null);
    }
}
