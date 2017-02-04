using System;
using System.Collections;
using Newtonsoft.Json;

public class IngameRequest : IEnumerator
{
    public long RequestId { get; private set; }
    public IngameRequestStates State { get; private set; }
    public Type ResultType { get; private set; }
    public SocketRequestFormat Response
    {
        get { return response; }
        internal set
        {
            if (!State.IsFinal())
            {
                response = value;
                if (response == null)
                    Exception = new Exception("Null response");
                else if (response.code == 200)
                    State = IngameRequestStates.Done;
                //else
                   // Exce//ion = new IngameException(response);
            }
        }
    }
    public Exception Exception
    {
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

    public DateTime RequestTime
    {
        get { return requestTime; }
        internal set
        {
            if (State == IngameRequestStates.Unsent)
            {
                requestTime = value;
                State = IngameRequestStates.Sent;
            }
        }
    }

    public TimeSpan Elasped
    {
        get { return DateTime.UtcNow - RequestTime; }
    }

    DateTime requestTime = DateTime.UtcNow;
    SocketRequestFormat response = null;
    Exception exception = null;

    public IngameRequest(SocketRequestFormat req, Type responseType)
    {
        RequestId = req.id;
        ResultType = responseType;
        State = IngameRequestStates.Unsent;
    }

    public T Result<T>()
    {
        try
        {
            WebSocketResult<T> res = TcpSocket.inst.Deserializaer<WebSocketResult<T>>(Response.bytes);
            return res.result;
        }
        catch (System.Exception e)
        {
            return default(T);
        }
    }

    public object Current
    {
        get { return null; }
    }

    public bool MoveNext()
    {
        return !State.IsFinal();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
