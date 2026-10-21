using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>Car rate for the n-th billable hour: first, second, then a flat rate for every later hour.</summary>
public sealed record HourlyRateSchedule(Money FirstHour, Money SecondHour, Money SubsequentHours)
{
    public static readonly HourlyRateSchedule Standard = new(new Money(5m), new Money(3m), new Money(2m));

    public Money RateForHour(int hour)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(hour, 1);
        return hour switch
        {
            1 => FirstHour,
            2 => SecondHour,
            _ => SubsequentHours,
        };
    }
}
