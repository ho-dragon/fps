using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
public class SocketRequest : MonoBehaviour
{
	public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0) {
	    return new Rect(_width * raw, _height * column, _width, _height);
	}

	void OnGUI() {
	   if (GUI.Button(GetRectPos(0, 1, 200, 100), "EnterRoom")) {
           string testerName = "";
           EnterRoom(testerName);
	   }
	}

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
        int SenddataLength = Encoding.Default.GetByteCount(msg);
        byte[] Sendbyte = Encoding.Default.GetBytes(msg);
        m_Socket.Send(Sendbyte, Sendbyte.Length, SocketFlags.None);
    }


    public void EnterRoom(string playerName) {
        Send("enterRoom" + "|" + playerName);
    }
}
