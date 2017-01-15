﻿using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class SocketResponse : MonoBehaviour
{
    private DataResolver resolver;
    void Awake() {
        this.resolver = new DataResolver();
    }

    public void GetRecevieBuffer(byte[] buffer) {
        //Debug.Log("[SocketResponse.GetRecevieBuffer] buffer size = " + buffer.Length);
        //for (int i = 0; i < buffer.Length; i++) {
        //    Debug.Log("buffer[{" + i + "}] = " + buffer[i]);
        //}
        this.resolver.on_receive(buffer, 0, buffer.Length, CallbackRecevieBuffer);
    }

    public void CallbackRecevieBuffer(byte[] data) {
        //Debug.Log("[SocketResponse.CallbackRecevieBuffer] data size = " + data.Length);
        byte[] header = new byte[4];
        Array.Copy(data, header, 4);
        int dataSize = get_body_size(header);
        if (dataSize < 1) {
            return;
        }
        byte[] body = new byte[dataSize];
        Array.Copy(data, 4, body, 0, dataSize);
        //Debug.Log("[SocketResponse.callback] data size = " + dataSize + " / data = " + Encoding.UTF8.GetString(body));
        Receiver(Encoding.UTF8.GetString(body));
    }

    private int get_body_size(byte[] data) {
        Type type = Defines.HEADERSIZE.GetType();
        return BitConverter.ToInt32(data, 0);
    }

    public void Receiver(string stringData) {
        string[] msgs = stringData.Split('|');
        switch(msgs[0]) {
            case "joinPlayer" :
                JoinPlayer(msgs);
                break;
            case "move":
                MovePlayer(msgs);
                break;
            case "enterRoomResult":
                enterRoomResult(msgs);
                break;
        }
    }

    private void enterRoomResult(string[] msgs) {
        int playerNum = System.Convert.ToInt32(msgs[1]);
        string playerName = msgs[2];
        PlayerManager.inst.player.Init(playerNum
                                     , playerName
                                     , (number, movePos) => {
                                        TcpSocket.inst.sender.MovePlayer(number, movePos);
                                     });
    }

    private void JoinPlayer(string[] msgs) {
        int playerNum = System.Convert.ToInt32(msgs[1]);
        string playerName = msgs[2];
        PlayerManager.inst.JoinedPlayer(playerNum, playerName);
    }

    private void MovePlayer(string[] msgs) {
        int playerNum = System.Convert.ToInt32(msgs[1]);
        Vector3 movePos = new Vector3(System.Convert.ToSingle(msgs[2])
                                    , System.Convert.ToSingle(msgs[3])
                                    , System.Convert.ToSingle(msgs[4]));
        PlayerManager.inst.MovePlayer(playerNum, movePos);
    }
}
