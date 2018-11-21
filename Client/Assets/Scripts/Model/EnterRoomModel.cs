using UnityEngine;
using System.Collections.Generic;

public class EnterRoomModel {
    public PlayerModel player;
    public PlayerModel[] otherPlayers;
}

public class PlayerModel {
    public string name;
    public int number;
    public int teamCode;
    public float currentHP;
    public float maxHP;
    public float[] lastPosition;
}