using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
public class SocketRequest : MonoBehaviour
{
	public Socket m_Socket;
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
    public void Send(string msg) {
        if(TcpSocket.inst.IsConnected == false) {
            return;
        }
        int SenddataLength = Encoding.Default.GetByteCount(msg);
        byte[] Sendbyte = Encoding.Default.GetBytes(msg);
        m_Socket.Send(Sendbyte, Sendbyte.Length, SocketFlags.None);
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
