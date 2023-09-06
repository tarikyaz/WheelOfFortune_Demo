using System;

public static class TimerUtility
{
    // Unix epoch time
    static DateTime firstDt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

    // Get the current time in UTC
    public static DateTime CurrentTime
    {
        get
        {
            return DateTime.UtcNow;
        }
    }

    // Get the next day's time at 13:00 UTC
    public static DateTime GetRestTime(DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 13, 0, 0, 0).AddDays(1);

    // Convert a timestamp (seconds since Unix epoch) to a DateTime object
    public static DateTime ConvertTimestampToDateTime(double timestamp)
    {
        return firstDt.AddSeconds(timestamp);
    }

    // Convert a DateTime object to a timestamp (seconds since Unix epoch)
    public static double ConvertDateTimeToTimestamp(DateTime dateTime)
    {
        return (dateTime - firstDt).TotalSeconds;
    }
}
