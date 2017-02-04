using System;

public class IngameTimeSync
{
    long delta = 0;
    long minRtt = long.MaxValue;

    public static DateTime Epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

    public static long LocalPosixTime
    {
        get { return (long)(System.DateTime.UtcNow - Epoch).TotalMilliseconds; }
    }

    internal void Update(long serverPosixTime, long rtt)
    {
        if (minRtt > rtt)
        {
            long estimated = rtt / 2 + serverPosixTime;
            delta = estimated - LocalPosixTime;
            minRtt = rtt;
        }
    }

    internal void Adjust(long serverPosixTime)
    {
        long delay = PosixTime - serverPosixTime;
        if (delay < 0)
            delta -= delay;
    }

    public DateTime Now
    {
        get { return System.DateTime.Now.AddMilliseconds(delta); }
    }

    public DateTime UtcNow
    {
        get { return System.DateTime.UtcNow.AddMilliseconds(delta); }
    }

    public long PosixTime
    {
        get { return (long)(delta + LocalPosixTime); }
    }
}