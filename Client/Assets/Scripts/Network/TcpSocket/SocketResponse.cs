using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

public class SocketResponse : MonoBehaviour {
    private DataResolver resolver;
    public IngameClient ingameClient;

    void Awake() {
        this.resolver = new DataResolver();
    }

    public void GetRecevieBuffer(byte[] buffer) {
        //Logger.Debug("[SocketResponse.GetRecevieBuffer] buffer size = " + buffer.Length);
        //for (int i = 0; i < buffer.Length; i++) {
        //    Logger.Debug("buffer[{" + i + "}] = " + buffer[i]);
        //}
        this.resolver.on_receive(buffer, 0, buffer.Length, CallbackRecevieBuffer);
    }

    public void CallbackRecevieBuffer(byte[] data) {
        //Logger.Debug("[SocketResponse.CallbackRecevieBuffer] data size = " + data.Length);
        byte[] header = new byte[4];
        Array.Copy(data, header, 4);
        int dataSize = get_body_size(header);
        if (dataSize < 1) {
            return;
        }
        byte[] body = new byte[dataSize];
        Array.Copy(data, 4, body, 0, dataSize);
        //Logger.Debug("[SocketResponse.callback] data size = " + dataSize + " / data = " + Encoding.UTF8.GetString(body));
        Receiver(Encoding.UTF8.GetString(body), body);
    }

    private int get_body_size(byte[] data) {
        return BitConverter.ToInt32(data, 0);
    }

    public void Receiver(string stringData, byte[] buffer) {
        ingameClient.OnMessage(buffer);
    }

    public void RecevieNotification(SocketRequestFormat result) {        
        switch(result.method){
            case "damagedPlayer":
                DamagedPlayer(TcpSocket.inst.Deserializaer<DamageModel>(result.bytes));
                break;
            case "joinPlayer":
                JoinPlayer(TcpSocket.inst.Deserializaer<EnterRoomModel>(result.bytes));
                break;
            case "movePlayer":
                MovePlayer(TcpSocket.inst.Deserializaer<PlayerMoveModel>(result.bytes));
                break;
        }
    }

    public void DamagedPlayer(DamageModel result) {
        Logger.DebugHighlight("[RecevieNtotication.DamagedPlayer]");
        PlayerManager.inst.DamagedPlayer(result);
    }

    public void JoinPlayer(EnterRoomModel result) {
        Logger.DebugHighlight("[RecevieNtotication.JoinPlayer]");
        PlayerManager.inst.JoinedPlayer(result.player, false);
    }

    private void MovePlayer(PlayerMoveModel result) {
        //Logger.DebugHighlight("[RecevieNtotication.MovePlayer]  = " + result.playerNum + " / movePosition X : {0} Y : {1} : Z : {2}" , result.playerPosX, result.playerPosY, result.playerPosZ);
        PlayerManager.inst.MovePlayer(result.playerNum, new Vector3(result.playerPosX
                                                                    , result.playerPosY
                                                                    , result.playerPosZ));
    }
}