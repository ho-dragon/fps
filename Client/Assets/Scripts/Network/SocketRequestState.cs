
public enum SocketRequestState : int
{
    Unsent = 0,
    Sent = 1,
    Done = 2,
    Error = 3,
    TimedOut = 4
}


public static class IngameRequestStatesExtensions
{
    public static bool IsFinal(this SocketRequestState state)
    {
        return state >= SocketRequestState.Done;
    }

    public static bool IsDone(this SocketRequestState state)
    {
        return state == SocketRequestState.Done;
    }
}