namespace SmartParking.Domain;

/// <summary>A daily time-of-day range, start inclusive and end exclusive. The end may be 24:00 (midnight).</summary>
public sealed record TimeWindow
{
    private static readonly TimeSpan EndOfDay = TimeSpan.FromHours(24);

    private TimeWindow(TimeSpan start, TimeSpan end)
    {
        if (start < TimeSpan.Zero || start >= end || end > EndOfDay)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "A window must satisfy 00:00 <= start < end <= 24:00.");
        }

        Start = start;
        End = end;
    }

    public TimeSpan Start { get; }

    public TimeSpan End { get; }

    public static TimeWindow Between(int startHour, int startMinute, int endHour, int endMinute) =>
        new(new TimeSpan(startHour, startMinute, 0), new TimeSpan(endHour, endMinute, 0));

    public bool Contains(DateTime moment) => moment.TimeOfDay >= Start && moment.TimeOfDay < End;

    /// <summary>True if any part of [from, to) falls inside this window on the given calendar date.</summary>
    public bool Overlaps(DateOnly date, DateTime from, DateTime to)
    {
        var windowStart = date.ToDateTime(TimeOnly.MinValue) + Start;
        var windowEnd = date.ToDateTime(TimeOnly.MinValue) + End;
        return from < windowEnd && to > windowStart;
    }
}
