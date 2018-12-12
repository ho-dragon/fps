using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public partial class PacketManager: MonoBehaviour {
    private Socket socket;
    public Exception Exception { get; private set; }
    public delegate void Response<T>(SocketRequestEntry req, T result = null) where T : class;
    public List<IResponse> responseList = new List<IResponse>();
    private long lastRequestId = 0;
    private SocketState state = SocketState.Unconnected;
    private List<SocketRequestEntry> requests = new List<SocketRequestEntry>();
    private Queue<long> RequestIdQueue;
    private TcpBufferHandelr resolver;
    private PacketNotification packetNotification;
    private PacketRequest packetRequest;

    public void SetSocket(Socket socket) {
        this.socket = socket;
        this.state = SocketState.Connected;
    }

    public PacketRequest PacketRequest {
        get { return this.packetRequest; }
    }

    private class CanceledRequest {
        public long rid = 0;
        public string method = "";
    }

    public SocketState State {
        get { return state; }
    }

    void Awake() {
        this.state = SocketState.Unconnected;
        Exception = null;
        RequestIdQueue = new Queue<long>();
        this.resolver = new TcpBufferHandelr();
        this.packetNotification = new PacketNotification();
        this.packetRequest = new PacketRequest(this);
    }

    public SocketRequestEntry Send(string method, params object[] param) {
        return Send(method, null, param);
    }

    public SocketRequestEntry Send<T>(string method, params object[] param) {
        return Send(method, typeof(T), param);
    }

    void OnDestroy() {
        this.state = SocketState.Closed;
    }

    private void OnMessage(ResponseFormat response) {
        if( response == null) {
            Logger.Error("[OnMessage] response is null");
            return;
        }
        if (Logger.IsMutePacket(response.method) == false) {
            if (response.bytes != null) {
                Logger.Debug("[OnMessage] webSocketMsg.id = " + response.id + " / bates length = " + response.bytes.Length);
            }
        }

        if (response.IsNotification) {
            if (Logger.IsMutePacket(response.method) == false) {
                Logger.Debug("[OnMessage] IsNotification = true");
            }
            this.packetNotification.RecevieNotification(response);
        } else  {
            if (Logger.IsMutePacket(response.method) == false) {
                Logger.Debug("[OnMessage] IsNotification = false");
                Logger.Debug(string.Format("<color=#FFFFFF>[Response]</color> method = {0} rid = {1}", response.method, response.id));
            }
            DequeueRequestId(response.id);
            OnResponse(response);
        }
    }

    public RequestFormat CreateRequestFormat(string method, params object[] _params) {
        return new RequestFormat(method, ++lastRequestId, _params);
    }

    public void Send<T>(RequestFormat request, Response<T> response) where T : class {
        if (this.state != SocketState.Connected) {
            return;
        }
        SocketRequestEntry ingameRequest = new SocketRequestEntry(request, typeof(T));
        ingameRequest.RequestTime = DateTime.UtcNow;
        requests.Add(ingameRequest);
        EnqueueRequestId(lastRequestId);

        byte[] bytes = BsonSerializer.SerializeToByte(request);
        Send(bytes);

        if (Logger.IsMutePacket(request.method) == false) {
            Logger.Debug(string.Format("<color=#86E57F>[Send]</color> method = {0} rid = {1}", request.method, request.id));
        }        
        this.responseList.Add(new ResponseEntry<T>() { request = ingameRequest, responseCallback = response });
    }

    private void Send(byte[] data) {
        int sendDataLength = data.Length;
        byte[] header = BitConverter.GetBytes(sendDataLength);
        byte[] body = data;
        byte[] sendData = MergeBytes(header, body);

        if (this.socket.Connected == false) {
            this.state = SocketState.Unconnected;
            Logger.Error("[SocketRequest] disconnected from server");
            UIManager.inst.Alert("게임 서버와 접속이 끊어졌습니다.");
            return;
        }
        this.socket.Send(sendData);
    }

    private byte[] MergeBytes(byte[] buffer1, byte[] buffer2) {
        byte[] tmp = new byte[buffer1.Length + buffer2.Length];
        for (int i = 0; i < buffer1.Length; i++) {
            tmp[i] = buffer1[i];
        }
        for (int j = 0; j < buffer2.Length; j++) {
            tmp[buffer1.Length + j] = buffer2[j];
        }
        return tmp;
    }

    void OnResponse(ResponseFormat res) {
        SocketRequestEntry req = this.requests.Find(r => r.RequestId == res.id);
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


    public void ReceiveBuffer(byte[] buffer) {       
        this.resolver.OnRecevie(buffer, 0, buffer.Length, CallbackRecevieBuffer);
    }

    public void CallbackRecevieBuffer(byte[] data) {
        byte[] header = new byte[4];
        Array.Copy(data, header, 4);
        int dataSize = GetBodySize(header);
        if (dataSize < 1) {
            return;
        }
        byte[] body = new byte[dataSize];
        Array.Copy(data, 4, body, 0, dataSize);
        Deserialize(body);
    }

    private int GetBodySize(byte[] data) {
        return BitConverter.ToInt32(data, 0);
    }

    public void Deserialize(byte[] data) {
        ResponseFormat response = BsonSerializer.Deserialize<ResponseFormat>(data);
        this.OnMessage(response);
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

