using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class IngameClient : MonoBehaviour {
    public IngameClientStates State {
        get { return state; }
        private set { state = value; }
    }

    private const int RequestTimeout = 5000;
    private const int RECONNECT_TIMEOUT = 20;
    public Exception Exception { get; private set; }
    static long lastRequestId = 0;
    IngameClientStates state = IngameClientStates.Unconnected;
    List<IngameRequest> requests = new List<IngameRequest>();
    Queue<SocketRequestFormat> notifications = new Queue<SocketRequestFormat>();
    private Queue<long> RequestIdQueue;
    public  SocketRequest socketRequest;

    void Awake() {
        Exception = null;
        RequestIdQueue = new Queue<long>();
        State = IngameClientStates.Connecting;
    }

    public IngameRequest Send(string method, params object[] param) {
        return Send(method, null, param);
    }

    public IngameRequest Send<T>(string method, params object[] param) {
        return Send(method, typeof(T), param);
    }

    void OnDestroy() {
        State = IngameClientStates.Closed;
    }

    public void OnMessage(byte[] data) {
        SocketResponsePormat response = TcpSocket.inst.Deserializaer<SocketResponsePormat>(data);
        if (Logger.IsMutePacket(response.method) == false) {
            Logger.Debug("[OnMessage] webSocketMsg.id = " + response.id + " / data length = " + data.Length + " / bates length = " + response.bytes.Length);
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

    public interface IResponse {
        long GetRid();
        void ExcuteCallback();
    }

    public class ResponseEntry<T> : IResponse where T : class {
        public IngameRequest ingameRequest;
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


    public SocketRequestFormat req(string method, params object[] _params) {
        return new SocketRequestFormat(method, ++lastRequestId, _params);
    }

    public delegate void Response<T>(IngameRequest req, T result = null) where T : class;
    public List<IResponse> responseList = new List<IResponse>();
    public void send<T>(SocketRequestFormat request, Response<T> response) where T : class {
        IngameRequest ingameRequest = new IngameRequest(request, typeof(T));
        ingameRequest.RequestTime = DateTime.UtcNow;
        requests.Add(ingameRequest);
        EnqueueRequestId(lastRequestId);

        byte[] bytes = TcpSocket.inst.SerializeToByte(request);
        socketRequest.Send(bytes);

        if (Logger.IsMutePacket(request.method) == false) {
            Logger.Debug(string.Format("<color=#86E57F>[Send]</color> method = {0} rid = {1}", request.method, request.id));
        }        
        this.responseList.Add(new ResponseEntry<T>() { ingameRequest = ingameRequest, responseCallback = response });
    }

    private class CanceledRequest {
        public long rid = 0;
        public string method = "";
    }


    private List<CanceledRequest> canceledRequests;
    void CancelRequests(Exception ex) {
        canceledRequests = new List<CanceledRequest>();
        if (this.requests != null && this.requests.Count > 0) {
            foreach (IngameRequest i in this.requests) {
                canceledRequests.Add(new CanceledRequest() {
                    rid = i.RequestId,
                    method = i.RequestMethod
                });
            }
        }
        this.requests.ForEach(req => req.Exception = ex);
        this.requests = new List<IngameRequest>();
        this.responseList = new List<IResponse>();
    }


    void OnResponse(SocketResponsePormat res) {
        IngameRequest req = this.requests.Find(r => r.RequestId == res.id);
        if (req != null) {
            this.requests.Remove(req);
            req.Response = res;

            if (req .Equals("ping")) {
                return;
            }

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

