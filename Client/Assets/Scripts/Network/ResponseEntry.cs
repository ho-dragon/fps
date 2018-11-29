public interface IResponse {
    long GetRid();
    void ExcuteCallback();
}

public class ResponseEntry<T> : IResponse where T : class {
    public SocketRequestEntry request;
    public PacketManager.Response<T> responseCallback;

    public long GetRid() {
        return this.request.RequestId;
    }

    public void ExcuteCallback() {
        if (request.State == SocketRequestState.Error) {//ex NO_ENEMY_EXIST
            Logger.Error("[ResponseEntry.ExcuteCallback] error / state = " + request.State.ToString());
            responseCallback(this.request, null);
            return;
        }

        if (request.State.IsDone() == false) {
            Logger.Error("[ResponseEntry.ExcuteCallback] failed / not done / state = " + request.State.ToString());
            responseCallback(this.request, null);
            return;
        }

        T result = this.request.Result<T>();

        if (result == null) {
            Logger.Error("[ResponseEntry.ExcuteCallback] result is null -- 1");
        }

        if (result == null && typeof(T) == typeof(object)) {
            Logger.Error("[ResponseEntry.ExcuteCallback] result is null -- 2");
            result = (T)new object();
        }

        if (responseCallback != null) {
            responseCallback(this.request, result);
        }
    }
}
