public enum IngameClientStates
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
    public static bool IsConnecting(this IngameClientStates state)
    {
        return state == IngameClientStates.Connecting;
    }

    public static bool IsClosed(this IngameClientStates state)
    {
        return state == IngameClientStates.Closed;
    }
}
