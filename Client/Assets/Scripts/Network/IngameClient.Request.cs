using UnityEngine;
using System.Collections;

public partial class IngameClient : MonoBehaviour {
    public void EnterRoom(string playerName, Response<EnterRoomModel> callback) {
        send<EnterRoomModel>(req("enterRoom", "playerName", playerName), callback);
    }

    public void Init(string testMsg, bool testBool,long testNum, int testInt, Response<object> callback)
    {
       // send<object>(req("init", "testCode", testCode), callback);
        send<object>(req("init", "testMsg", testMsg, "testBool", testBool, "testNum", testNum, "testInt", testInt), callback);
    }
}
