using System;
using System.Collections;
using Newtonsoft.Json;

public class SocketRequestEntry {
    public long RequestId { get; private set; }
    public SocketRequestState State { get; private set; }
    public Type ResultType { get; private set; }
    public string RequestMethod { get; protected set; }
    DateTime requestTime = DateTime.UtcNow;
    ResponseFormat response = null;
    Exception exception = null;

    public SocketRequestEntry(RequestFormat req, Type responseType) {
        RequestId = req.id;
        ResultType = responseType;
        RequestMethod = req.method;
        State = SocketRequestState.Unsent;
    }

    public void Reset() {
        throw new NotImplementedException();
    }
       
    public T Result<T>() {
        try {
            //Logger.Debug("[ingameRequest.Result<T>] Response.bytes.length = " + Response.bytes.Length);
            T result = BsonSerializer.Deserialize<T>(Response.bytes);
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
                    State = SocketRequestState.Done;
                else
                    Exception = new Exception(string.Format("response is not success / code = {0}", response.code.ToString()));
            }
        }
    }

    public ErrorType GetErrorType() {
        return (ErrorType)this.response.code;
    }

    public Exception Exception {
        get { return exception; }
        set
        {
            if (!State.IsFinal())
            {
                exception = value;
                if (exception is TimeoutException)
                    State = SocketRequestState.TimedOut;
                else
                    State = SocketRequestState.Error;
            }
        }
    }

    public DateTime RequestTime {
        get { return requestTime; }
        internal set {
            if (State == SocketRequestState.Unsent) {
                requestTime = value;
                State = SocketRequestState.Sent;
            }
        }
    }
}
