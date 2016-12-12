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
        }
    }

    private void JoinPlayer(string[] msgs) {
        int playerNum = System.Convert.ToInt32(msgs[1]);
        string playerName = msgs[2];
        Main.inst.JoinPlayer(playerNum, playerName);
    }
}
