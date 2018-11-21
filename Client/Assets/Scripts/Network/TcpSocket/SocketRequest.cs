using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class SocketRequest : MonoBehaviour {
	public Socket socket;
    public void Send(byte[] msg) {
        if (TcpSocket.inst.IsConnected == false) {
            return;
        }
        int sendDataLength = msg.Length;
        byte[] header = BitConverter.GetBytes(sendDataLength);
        byte[] body = msg;
        byte[] totalSendBuffer = byte_merge(header, body);

        if (socket.Connected == false) {
            Logger.Error("[SocketRequest] disconnected from server");
            return;
        }
        socket.Send(totalSendBuffer, totalSendBuffer.Length, SocketFlags.None);
    }

    private byte[] byte_merge(byte[] arg1, byte[] arg2) {
        byte[] tmp = new byte[arg1.Length + arg2.Length];
        for (int i = 0; i < arg1.Length; i++) {
            tmp[i] = arg1[i];
        }
        for (int j = 0; j < arg2.Length; j++) {
            tmp[arg1.Length + j] = arg2[j];
        }
        return tmp;
    }
}