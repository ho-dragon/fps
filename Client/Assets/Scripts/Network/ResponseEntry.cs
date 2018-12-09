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
        if (request.State == SocketRequestState.Error) {
            Logger.Error("[ResponseEntry.ExcuteCallback] error / state = " + request.State.ToString());
            responseCallback(this.request, null);
            HandleError(request.GetErrorType());
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

    private void HandleError(ErrorType errorType) {
        Logger.Error("[ResponseEntry.HandleError] erroType = " + errorType.ToString());
        switch (errorType) {
            case ErrorType.NO_PLAYING_GAME://재접속시 게임종료됐을때
                UIManager.inst.Alert(errorType.GetDesc());
                break;
            case ErrorType.ROOM_IS_PLAYING:
                UIManager.inst.Alert(errorType.GetDesc());
                TcpSocket.inst.Request.JoinRunningGame(Main.inst.GetPlayerId(), (req, result) => {
                    Main.inst.JoinRoom(true, result);
                });
                break;
            case ErrorType.ROOM_IS_FULL:
                UIManager.inst.Alert("방이 가득 찼습니다. 게임 종료까지  남았습니다.");
                break;
        }
    }
}
