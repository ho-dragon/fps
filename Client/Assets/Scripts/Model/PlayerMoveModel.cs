using UnityEngine;
using System.Collections;

public class PlayerMoveModel {
   public int playerNum;
   public float playerPosX;
   public float playerPosY;
   public float playerPosZ;
   public float playerYaw;
}

public class PLayerActionModel {
    public int playerNum;
    public PLAYER_ACTION_TYPE actionType;
}

public enum PLAYER_ACTION_TYPE {
    Idle = 0,
    Walk = 1,
    Run = 2,
    Attack = 3,
    Death = 4,
    Damage = 5,
    Jump = 6,
    Aiming = 7,
    Sitting = 8
}