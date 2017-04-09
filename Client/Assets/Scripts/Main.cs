using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviourInstance<Main>
{
    #region OnGUI
    public string userName = "PlayerName";

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

        userName = GUI.TextField(GetRectPos(0, 1, 200, 50), userName, 25);

        if (GUI.Button(GetRectPos(0, 2, 200, 50), "Connect")) {
            TcpSocket.inst.Connect();
        }

        if (GUI.Button(GetRectPos(0, 3, 200, 50), "EnterRoom")) {
            EnterRoom(userName);
        }

        if (GUI.Button(GetRectPos(0, 4, 200, 50), "JoinRoom(dummy)")) {
            int playerRandomNum = UnityEngine.Random.Range(0,1000);
            while (PlayerManager.inst.IsExsitPlayer(playerRandomNum)) {
                playerRandomNum = UnityEngine.Random.Range(0,1000);
            }
           PlayerManager.inst.JoinedPlayer(playerRandomNum, "dummy"+ playerRandomNum.ToString());
        }
       
    }
    #endregion

    void Start() {
        TcpSocket.inst.SerializerTest();
    }

    public bool isTestOn = false;
    public void EnterRoom(string userName) {
        TcpSocket.inst.client.EnterRoom(userName, (req, result) => {
            Debug.Log("[입장완료]");
            PlayerManager.inst.player.Init(result.playerNum
                             , result.playerName
                             , (number, movePos) => {
                                 TcpSocket.inst.sender.MovePlayer(number, movePos);
                             });
        });
    }
}
