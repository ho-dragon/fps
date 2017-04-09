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

public partial class IngameClient : MonoBehaviour
{
    public IngameClientStates State
    {
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
    IngameTimeSync timeSync = new IngameTimeSync();
    public  SocketRequest socketRequest;


    Dictionary<string, string> NotiExecuteType = new Dictionary<string, string>(){
		{"Dead", "Que"},
		{"Fight","Que"},
		{"Win", "Que"},
		{"GunSound", "Que"},
		{"StartBossBattle", "Que"}
	};

    void Awake() {
        Exception = null;
        RequestIdQueue = new Queue<long>();
        State = IngameClientStates.Connecting;
    }

    public IngameRequest Send(string method, params object[] param)
    {
        return Send(method, null, param);
    }

    public IngameRequest Send<T>(string method, params object[] param)
    {
        return Send(method, typeof(T), param);
    }


    public bool TryGetNextNotification(out SocketRequestFormat webSocketMsg)
    {
        if (notifications.Count > 0)
        {
            webSocketMsg = notifications.Dequeue();
            return true;
        }

        webSocketMsg = null;
        return false;
    }

    public string GetExecuteType(string method)
    {
        if (NotiExecuteType.ContainsKey(method))
        {
            return NotiExecuteType[method];
        }
        else
            return "";
    }

    public bool TryGetNextNoti(out SocketRequestFormat webSocketMsg)
    {
        if (notifications.Count > 0)
        {
            webSocketMsg = notifications.Dequeue();
            return true;
        }

        webSocketMsg = null;
        return false;
    }

    public int GetNoticeCount()
    {
        return notifications.Count;
    }


    void OnDestroy()
    {
        ReleaseWebSocket(true);
    }


    public void OnMessage(byte[] data) {
        SocketRequestFormat webSocketMsg = TcpSocket.inst.Deserializaer<SocketRequestFormat>(data);
        webSocketMsg.bytes = data;
        Debug.Log("[OnMessage] webSocketMsg.id = " + webSocketMsg.id + " / data length = " + data.Length);
        timeSync.Adjust(webSocketMsg.time);
   
        if (webSocketMsg.IsNotification) {
            Debug.Log("[OnMessage] IsNotification = true");
            OnNotification(webSocketMsg);
        } else  {
            Debug.Log("[OnMessage] IsNotification = false");
            DequeueRequestId(webSocketMsg.id);
            OnResponse(webSocketMsg);
        }
    }

    void ReleaseWebSocket(bool close)
    {
        //if (ws != null)
        //{
        //    while (notifications.Count > 0)
        //    {
        //        Ingame_Socket.Instance.EndSocketMessage(notifications.Dequeue());
        //    }
        //    CancelRequests(new Exception("WebSocket is closed"));

        //    notifications.Clear();
        //    ws.OnMessage -= OnMessage;
        //    ws.OnBinary -= OnBinary;
        //    ws.OnOpen -= OnOpen;
        //    ws.OnClose -= OnClose;
        //    ws.OnError -= OnError;
        //    if (close)
        //        ws.Close();
        //    ws = null;
        //}
        State = IngameClientStates.Closed;
    }


    public SocketRequestFormat req(string method, params object[] _params) {
        return new SocketRequestFormat(method, ++lastRequestId, timeSync.PosixTime, _params);
    }

    public delegate void Response<T>(IngameRequest req, T result = null) where T : class;

    public void send<T>(SocketRequestFormat websocketRequest, Response<T> response) where T : class {
        IngameRequest ingameRequest = new IngameRequest(websocketRequest, typeof(T));
        ingameRequest.RequestTime = DateTime.UtcNow;
        requests.Add(ingameRequest);
        EnqueueRequestId(lastRequestId);

        byte[] json = TcpSocket.inst.SerializeToByte(websocketRequest);
        socketRequest.Send(json);
        Debug.Log(string.Format("<color=#86E57F>[Send]</color> {0}: {1}", websocketRequest.method, websocketRequest.id));
        StartCoroutine(waitingResponse<T>(ingameRequest, response));
    }

    private IEnumerator waitingResponse<T>(IngameRequest ingameRequest, Response<T> response) where T : class
    {
        Debug.Log("waitingResponse--0");
        yield return StartCoroutine(ingameRequest);
        Debug.Log("waitingResponse--1");
        if (ingameRequest.State.IsDone() == false) {
            response(ingameRequest, null);
            yield break;
        }
        Debug.Log("waitingResponse--2");

        T result = ingameRequest.Result<T>();
        if (result == null && typeof(T) == typeof(object)) {
            result = (T)new object();
        }
        response(ingameRequest, result);
    }

    IEnumerator DelaySend(string method, Type resultType, params object[] param)
    {
        yield return new WaitForSeconds(.1f);
        Send(method, resultType, param);
    }

    void CancelRequests(Exception ex)
    {
        requests.ForEach(req => req.Exception = ex);
        requests = new List<IngameRequest>();
    }

    Type GetResponseResultType(long id)
    {
        IngameRequest req = requests.Find(r => r.RequestId == id);
        return req == null ? null : req.ResultType;
    }

    void OnNotification(SocketRequestFormat notification)
    {
        if (notification != null)
        {
           // GlobalData.AddNotiMessage(notification);
            notifications.Enqueue(notification);
        }
    }

    void OnResponse(SocketRequestFormat res) {
        Debug.Log("[OnResponse]");
        IngameRequest req = requests.Find(r => r.RequestId == res.id);
        if (req != null) {
            Debug.Log("[onresponse] response id = " + req.RequestId);
            requests.Remove(req);
            timeSync.Update(res.time, (long)req.Elasped.TotalMilliseconds);
            req.Response = res;
        } else {
            Debug.Log("[onresponse] response is null");
        }

    }

    Queue<long> RequestIdQueue;
    public int GetQueueCount()
    {
        return RequestIdQueue.Count;
    }

    public long GetIdNumber()
    {
        if (RequestIdQueue.Count > 0)
        {
            return RequestIdQueue.Peek();
        }
        return -1;
    }

    void EnqueueRequestId(long _Id)
    {
        RequestIdQueue.Enqueue(_Id);
    }

    bool DequeueRequestId(long _Id, string _Msg = "")
    {
        if (RequestIdQueue.Count == 0)
        {
            return false;
        }

        if (RequestIdQueue.Peek() == _Id)
        {
            RequestIdQueue.Dequeue();
            return true;
        }

        return false;
    }
}

