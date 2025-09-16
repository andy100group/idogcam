using System;

public class TimeRange
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public override string ToString()
    {
        return $"{StartTime:hh\\:mm}am-{EndTime:hh\\:mm}pm";
    }
}