using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviourInstance<Main>
{
    #region OnGUI
    public string userName = "PlayerName";
    public string ip = "127.0.0.1";
    public string port = "8107";

    public Rect GetRectPos(int raw, int column, float _width = 0, float _height = 0)
    {
        return new Rect(_width * raw, _height * column, _width, _height);
    }

    void OnGUI()
    {
        if (isTestOn == false)
        {
            return;
        }


        ip = GUI.TextField(GetRectPos(0, 1, 200, 50), ip, 25);
        port = GUI.TextField(GetRectPos(0, 2, 200, 50), port, 25);
        userName = GUI.TextField(GetRectPos(0, 3, 200, 50), userName, 25);

        if (GUI.Button(GetRectPos(0, 4, 200, 50), "Connect Socket"))
        {
            TcpSocket.inst.Connect(this.ip, System.Convert.ToInt32(this.port));
        }

        if (GUI.Button(GetRectPos(0, 5, 200, 50), "EnterRoom"))
        {
            EnterRoom(userName);
        }

        //if (GUI.Button(GetRectPos(0, 6, 200, 50), "JoinRoom(dummy)")) {
        //    int playerRandomNum = UnityEngine.Random.Range(0,1000);
        //    while (PlayerManager.inst.IsExsitPlayer(playerRandomNum)) {
        //        playerRandomNum = UnityEngine.Random.Range(0,1000);
        //    }
        //  // PlayerManager.inst.JoinedPlayer(playerRandomNum, "dummy"+ playerRandomNum.ToString());
        //}

    }
    #endregion

    public bool isTestOn = false;
    public void EnterRoom(string userName) {
        TcpSocket.inst.client.EnterRoom(userName, (req, result) => {
            Debug.Log("complated localPlayer joinRoom");
            PlayerManager.inst.JoinedPlayer(result, true);
        });
    }
}
