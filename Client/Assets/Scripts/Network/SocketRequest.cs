using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class SocketRequest : MonoBehaviour
{
	public Socket m_Socket;

    public void Send(string msg) {
        if (TcpSocket.inst.IsConnected == false) {
            return;
        }

        int sendDataLength = Encoding.Default.GetByteCount(msg);
        byte[] header = BitConverter.GetBytes(sendDataLength);
        byte[] body = Encoding.Default.GetBytes(msg);
        byte[] totalSendBuffer = byte_merge(header, body);
        //Debug.Log("[SocketRequest.Send]: data length = " + totalSendBuffer.Length); 
        m_Socket.Send(totalSendBuffer, totalSendBuffer.Length, SocketFlags.None);
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

	public void Send(string method, string[] param) {
		StringBuilder sb = new StringBuilder();
		sb.Append(method);
		sb.Append("|");
		for (int count = 0; count < param.Length; count++) {
			sb.Append(param[count]);
			if (count != param.Length - 1) {
				sb.Append("*");
			}
		}
        Send(sb.ToString());
	}

	public void SendPosition(Vector3 position) {
		StringBuilder sb = new StringBuilder();
		sb.Append("1");
        Send(sb.ToString());
	}


    public void EnterRoom(string playerName) {
        Send("enterRoom" + "|" + playerName);
    }

    public void MovePlayer(int playerNum, Vector3 playerPos) {
        Send("move" + "|" + playerNum.ToString()
                    + "|" + playerPos.x
                    + "|" + playerPos.y
                    + "|" + playerPos.z);
    }
}
