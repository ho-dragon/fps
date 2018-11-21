using UnityEngine;
using System.Collections;

public partial class IngameClient : MonoBehaviour {
    //public void Init(string testMsg, bool testBool,long testNum, int testInt, Response<object> callback) {
    //    send<object>(req("init"), callback);
    //}

    public void EnterRoom(string playerName, Response<EnterRoomModel> callback) {
        send<EnterRoomModel>(req("enterRoom", "playerName", playerName), callback);
    }

    public void Attack(int attackPlayer, int damagedPlayer, int attackPosition, Response<DamageModel> callback) {
        send<DamageModel>(req("attackPlayer", "attackPlayer", attackPlayer
                                      , "damagedPlayer", damagedPlayer
                                      , "attackPosition" , attackPosition), callback);
    }

    public void MovePlayer(int playerNum, Vector3 playerPos) {
        send<PlayerMoveModel>(req("movePlayer", "playerNum", playerNum
                                              , "playerPosX", playerPos.x
                                              , "playerPosY", playerPos.y
                                              , "playerPosZ", playerPos.z), null);
    }
}
