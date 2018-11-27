public enum SocketState
{
    Unconnected,
    Connecting,
    ConnectingIpv6,
    Connected,
    Closing,
    Closed
}

public static class IngameClientStatesExtensions
{
    public static bool IsConnecting(this SocketState state)
    {
        return state == SocketState.Connecting;
    }

    public static bool IsClosed(this SocketState state)
    {
        return state == SocketState.Closed;
    }
}
