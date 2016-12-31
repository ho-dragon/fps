using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviourInstance<PlayerManager> {
    private List<Player> players;
    public Player player;

    protected override void _Awake() {
        this.players = new List<Player>();
    }

    private void Start() {
        if (this.player != null) {
            this.player.AddCamera(PlayerCamera.inst);
            this.player.isPlayable = true;
        }
    }

    public bool IsExsitPlayer(int playerNum) {
        return this.players.Exists(x => x.Number == playerNum);
    }

    public void JoinedPlayer(int playerNum, string playerName) {
        if (this.players.Exists(x => x.Number == playerNum)) {
            Debug.LogError("[Main.JoinPlayer] already exist player = " + playerNum);
            return;
        }

        GameObject clone = Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity) as GameObject;
        clone.transform.position = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(10, 20), UnityEngine.Random.Range(0, 10));
        Player newPlayer = clone.GetComponent<Player>();
        newPlayer.Init(playerNum
                     , playerName
                     , null);
        this.players.Add(newPlayer);
    }

    public void MovePlayer(int playerNumb, Vector3 movePosition) {
        Player player = this.players.Find(x => x.Number == playerNumb);
        if(player != null) {
            if(player.isPlayable == false) {
                player.SetMovePosition(movePosition);
            }
        }
    }
}
