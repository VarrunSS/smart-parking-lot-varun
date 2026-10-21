using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>Car rate for the n-th billable hour: first, second, then a flat rate for every later hour.</summary>
public sealed record HourlyRateSchedule
{
    public static readonly HourlyRateSchedule Standard = new(new Money(5m), new Money(3m), new Money(2m));

    public HourlyRateSchedule(Money firstHour, Money secondHour, Money subsequentHours)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(firstHour, Money.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(secondHour, Money.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(subsequentHours, Money.Zero);
        FirstHour = firstHour;
        SecondHour = secondHour;
        SubsequentHours = subsequentHours;
    }

    public Money FirstHour { get; }

    public Money SecondHour { get; }

    public Money SubsequentHours { get; }

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
