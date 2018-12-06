public enum ErrorType {
    NONE = 0,
    ROOM_IS_PLAYING = 9001,
    ROOM_IS_FULL = 9002,
    NO_PLAYING_GAME = 9003,
}

public static class ErrorTypeExtenstion {
    public static string GetDesc(this ErrorType errorType) {
        switch(errorType) {
            case ErrorType.ROOM_IS_FULL:
                return "방이 가득 찼습니다.";
            case ErrorType.ROOM_IS_PLAYING:
                return "게임이 진행중입니다.";
            case ErrorType.NO_PLAYING_GAME:
                return "진행중인 게임이 없습니다.";
        }
        return "";
    }
}

