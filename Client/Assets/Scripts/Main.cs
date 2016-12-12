using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviourInstance<Main> {
    private List<Player> player;
    public SocketRequest sender;

    void Awake() {
        this.player = new List<Player>();
    }

    public void EnterRoom(string userName) {
        this.sender.EnterRoom(userName);
    }

    public void JoinPlayer(int playerNum, string playerName) {
        if (this.player.Exists(x => x.Number == playerNum)) {
            Debug.LogError("[Main.JoinPlayer] already exist player = "+ playerNum);
            return;
        }
        Object prefab = Resources.Load("Player");
        GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        clone.transform.position = new Vector3(0, 5, UnityEngine.Random.Range(0, 10));
        Player newPlayer = clone.GetComponent<Player>();
        newPlayer.Init(playerNum, playerName);
        this.player.Add(newPlayer);
    }
}
