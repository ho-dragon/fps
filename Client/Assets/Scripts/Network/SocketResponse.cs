using UnityEngine;
using System.Collections;

public class SocketResponse : MonoBehaviour
{
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
