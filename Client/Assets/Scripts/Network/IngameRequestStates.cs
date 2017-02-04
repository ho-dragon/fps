
public enum IngameRequestStates : int
{
    Unsent = 0,
    Sent = 1,
    Done = 2,
    Error = 3,
    TimedOut = 4
}


public static class IngameRequestStatesExtensions
{
    public static bool IsFinal(this IngameRequestStates state)
    {
        return state >= IngameRequestStates.Done;
    }

    public static bool IsDone(this IngameRequestStates state)
    {
        return state == IngameRequestStates.Done;
    }
}