using UnityEngine;
using System.Collections;

public partial class IngameClient : MonoBehaviour {
    public void EnterRoom(string playerName, Response<EnterRoomModel> callback) {
        send<EnterRoomModel>(req("enterRoom", "playerName", playerName), callback);
    }
}
