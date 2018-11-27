using System;
using System.Collections;
using Newtonsoft.Json;

public class SocketRequest : IEnumerator {
    public long RequestId { get; private set; }
    public IngameRequestStates State { get; private set; }
    public Type ResultType { get; private set; }
    public string RequestMethod { get; protected set; }
    DateTime requestTime = DateTime.UtcNow;
    ResponseFormat response = null;
    Exception exception = null;

    public SocketRequest(RequestFormat req, Type responseType) {
        RequestId = req.id;
        ResultType = responseType;
        RequestMethod = req.method;
        State = IngameRequestStates.Unsent;
    }

    public object Current {
        get { return null; }
    }

    public bool MoveNext() {
        return !State.IsFinal();
    }

    public void Reset() {
        throw new NotImplementedException();
    }
       
    public T Result<T>() {
        try {
            Logger.Debug("[ingameRequest.Result<T>] Response.bytes.length = " + Response.bytes.Length);
            T result = TcpSocket.inst.Deserialize<T>(Response.bytes);
            return result;
        } catch (System.Exception e) {
            Logger.Error("[ingameRequest.Result<T>] deserialize failed / msg = " + e.Message);
            return default(T);
        }
    }

    public ResponseFormat Response {
        get { return response; }
        internal set {
            if (!State.IsFinal()) {
                response = value;
                if (response == null)
                    Exception = new Exception("Null response");
                else if (response.code == 200)
                    State = IngameRequestStates.Done;
                else
                    Exception = new Exception(string.Format("response is not success / code = {0}", response.code.ToString()));
            }
        }
    }

    public Exception Exception {
        get { return exception; }
        set
        {
            if (!State.IsFinal())
            {
                exception = value;
                if (exception is TimeoutException)
                    State = IngameRequestStates.TimedOut;
                else
                    State = IngameRequestStates.Error;
            }
        }
    }

    public DateTime RequestTime {
        get { return requestTime; }
        internal set {
            if (State == IngameRequestStates.Unsent) {
                requestTime = value;
                State = IngameRequestStates.Sent;
            }
        }
    }
}
