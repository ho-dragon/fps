using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections.Generic;

public partial class PacketManager: MonoBehaviour {
    public void EnterRoom(string playerName, Response<EnterRoomModel> callback) {
        send<EnterRoomModel>(req("enterRoom", "playerName", playerName), callback);
    }

    public void Attack(int attackPlayer, int damagedPlayer, int attackPosition, Response<DamageModel> callback) {
        send<DamageModel>(req("attackPlayer", "attackPlayer", attackPlayer
                                      , "damagedPlayer", damagedPlayer
                                      , "attackPosition", attackPosition), callback);
    }

    public void MovePlayer(int playerNum, Vector3 playerPos) {
        send<PlayerMoveModel>(req("movePlayer", "playerNum", playerNum
                                              , "playerPosX", playerPos.x
                                              , "playerPosY", playerPos.y
                                              , "playerPosZ", playerPos.z), null);
    }

    public Socket socket;
    private const int RequestTimeout = 5000;
    private const int RECONNECT_TIMEOUT = 20;
    public Exception Exception { get; private set; }
    static long lastRequestId = 0;
    SocketState state = SocketState.Unconnected;
    List<SocketRequest> requests = new List<SocketRequest>();
    Queue<RequestFormat> notifications = new Queue<RequestFormat>();
    private Queue<long> RequestIdQueue;
    public delegate void Response<T>(SocketRequest req, T result = null) where T : class;
    public List<IResponse> responseList = new List<IResponse>();

    private class CanceledRequest {
        public long rid = 0;
        public string method = "";
    }

    public interface IResponse {
        long GetRid();
        void ExcuteCallback();
    }

    public class ResponseEntry<T> : IResponse where T : class {
        public SocketRequest ingameRequest;
        public Response<T> responseCallback;

        public long GetRid() {
            return this.ingameRequest.RequestId;
        }

        public void ExcuteCallback() {
            if (ingameRequest.State == IngameRequestStates.Error) {//ex NO_ENEMY_EXIST
                Logger.Error("[INgameClinet.ResponseEntry.ExcuteCallback] error / state = " + ingameRequest.State.ToString());
                responseCallback(this.ingameRequest, null);
                return;
            }

            if (ingameRequest.State.IsDone() == false) {
                Logger.Error("[INgameClinet.ResponseEntry.ExcuteCallback] failed / not done / state = " + ingameRequest.State.ToString());
                responseCallback(this.ingameRequest, null);
                return;
            }

            T result = this.ingameRequest.Result<T>();

            if (result == null) {
                Logger.Error("[IngameClinet.ResponseEntry.ExcuteCallback] result is null -- 1");
            }

            if (result == null && typeof(T) == typeof(object)) {
                Logger.Error("[IngameClinet.ResponseEntry.ExcuteCallback] result is null -- 2");
                result = (T)new object();
            }

            if (responseCallback != null) {
                responseCallback(this.ingameRequest, result);
            }
        }
    }

    public SocketState State {
        get { return state; }
        private set { state = value; }
    }

    void Awake() {
        Exception = null;
        RequestIdQueue = new Queue<long>();
        State = SocketState.Connecting;
    }

    public SocketRequest Send(string method, params object[] param) {
        return Send(method, null, param);
    }

    public SocketRequest Send<T>(string method, params object[] param) {
        return Send(method, typeof(T), param);
    }

    void OnDestroy() {
        State = SocketState.Closed;
    }

    public void OnMessage(ResponseFormat response) {
        if (Logger.IsMutePacket(response.method) == false) {
            Logger.Debug("[OnMessage] webSocketMsg.id = " + response.id + " / bates length = " + response.bytes.Length);
        }

        if (response.IsNotification) {
            if (Logger.IsMutePacket(response.method) == false) {
                Logger.Debug("[OnMessage] IsNotification = true");
            }
            TcpSocket.inst.receiver.RecevieNotification(response);
        } else  {
            if (Logger.IsMutePacket(response.method) == false) {
                Logger.Debug("[OnMessage] IsNotification = false");
            }
            DequeueRequestId(response.id);
            OnResponse(response);
        }
    }


    public RequestFormat req(string method, params object[] _params) {
        return new RequestFormat(method, ++lastRequestId, _params);
    }

    public void send<T>(RequestFormat request, Response<T> response) where T : class {
        SocketRequest ingameRequest = new SocketRequest(request, typeof(T));
        ingameRequest.RequestTime = DateTime.UtcNow;
        requests.Add(ingameRequest);
        EnqueueRequestId(lastRequestId);

        byte[] bytes = TcpSocket.inst.SerializeToByte(request);
        TcpSocket.inst.Send(bytes);

        if (Logger.IsMutePacket(request.method) == false) {
            Logger.Debug(string.Format("<color=#86E57F>[Send]</color> method = {0} rid = {1}", request.method, request.id));
        }        
        this.responseList.Add(new ResponseEntry<T>() { ingameRequest = ingameRequest, responseCallback = response });
    }

    private void Send(byte[] data) {
        if (TcpSocket.inst.IsConnected == false) {
            return;
        }
        int sendDataLength = data.Length;
        byte[] header = BitConverter.GetBytes(sendDataLength);
        byte[] body = data;
        byte[] sendData = ByteMerge(header, body);

        if (this.socket.Connected == false) {
            Logger.Error("[SocketRequest] disconnected from server");
            return;
        }
        this.socket.Send(sendData);
    }

    private byte[] ByteMerge(byte[] buffer1, byte[] buffer2) {
        byte[] tmp = new byte[buffer1.Length + buffer2.Length];
        for (int i = 0; i < buffer1.Length; i++) {
            tmp[i] = buffer1[i];
        }
        for (int j = 0; j < buffer2.Length; j++) {
            tmp[buffer1.Length + j] = buffer2[j];
        }
        return tmp;
    }

    private List<CanceledRequest> canceledRequests;
    void CancelRequests(Exception ex) {
        canceledRequests = new List<CanceledRequest>();
        if (this.requests != null && this.requests.Count > 0) {
            foreach (SocketRequest i in this.requests) {
                canceledRequests.Add(new CanceledRequest() {
                    rid = i.RequestId,
                    method = i.RequestMethod
                });
            }
        }
        this.requests.ForEach(req => req.Exception = ex);
        this.requests = new List<SocketRequest>();
        this.responseList = new List<IResponse>();
    }


    void OnResponse(ResponseFormat res) {
        SocketRequest req = this.requests.Find(r => r.RequestId == res.id);
        if (req != null) {
            this.requests.Remove(req);
            req.Response = res;

            IResponse response = this.responseList.Find(x => x.GetRid() == res.id);
            if (response == null) {
                Logger.Error("[IngameClient.OnResponse] response is not found");
                return;
            }

            if (this.responseList.Remove(response) == false) {
                Logger.Error("[IngameClient.OnResponse] remove failed.");
            }
            response.ExcuteCallback();
        }
    }
    

    void EnqueueRequestId(long requestId) {
        RequestIdQueue.Enqueue(requestId);
    }

    bool DequeueRequestId(long requestId) {
        if (RequestIdQueue.Count == 0) {
            return false;
        }

        if (RequestIdQueue.Peek() == requestId) {
            RequestIdQueue.Dequeue();
            return true;
        }

        return false;
    }
}

