using UnityEngine;
using System.Collections.Generic;

public class EnterRoomModel {
    public PlayerModel player;
    public PlayerModel[] otherPlayers;
    public GameContextModel runningGameContext;
}

public class PlayerModel {
    public bool isDead;
    public string name;
    public int number;
    public int teamCode;
    public int deadCount;
    public int killCount;
    public float currentHP;
    public float maxHP;
    public float lastYaw;
    public float[] lastPosition;
}