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
            ErrorHandle(request.GetErrorType());
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

    private void ErrorHandle(ErrorType errorType) {
        switch(errorType) {
            case ErrorType.NO_PLAYING_GAME://재접속시 게임종료됐을때

                break;
            case ErrorType.ROOM_IS_PLAYING:// 재접속 선택 팝업
                TcpSocket.inst.Request.JoinRunningGame(Main.inst.userName, (req, result) => {
                    Main.inst.JoinRoom(true, result);
                });
                break;
            case ErrorType.ROOM_IS_FULL://방 입장이 불가능 .. 대기

                break;
        }
    }
}
