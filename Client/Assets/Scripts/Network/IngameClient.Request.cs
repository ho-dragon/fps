using UnityEngine;
using System.Collections;

public partial class IngameClient : MonoBehaviour {
    public void EnterRoom(string playerName, Response<EnterRoomModel> callback) {
        send<EnterRoomModel>(req("enterRoom", "playerName", playerName), callback);
    }

    public void Init(int testCode, Response<object> callback) {
        send<object>(req("init", "testCode", testCode), callback);
    }
}
