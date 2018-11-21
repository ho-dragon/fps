using UnityEngine;
using System.Collections.Generic;

public class EnterRoomModel {
    public PlayerModel player;
    public List<PlayerModel> otherPlayers;
}

public class PlayerModel {
    public string name;
    public int number;
    public int teamCode;
    public float currentHP;
    public float maxHP;
    public List<float> lastPosition;
}