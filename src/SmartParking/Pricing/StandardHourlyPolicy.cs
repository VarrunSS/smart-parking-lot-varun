using SmartParking.Calendar;
using SmartParking.Domain;
using SmartParking.Rules;

namespace SmartParking.Pricing;

/// <summary>
/// Progressive hourly rate. The stay is split into floating one-hour blocks starting at entry time; any block
/// touching a peak period costs <c>peakMultiplier</c> times its rate. Always applicable.
/// </summary>
public sealed class StandardHourlyPolicy : IPricingPolicy
{
    private static readonly TimeSpan Block = TimeSpan.FromHours(1);

    private readonly HourlyRateSchedule schedule;
    private readonly IPeakPeriodRule peakRule;
    private readonly decimal peakMultiplier;

    internal StandardHourlyPolicy(HourlyRateSchedule schedule, IPeakPeriodRule peakRule, decimal peakMultiplier)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(peakRule);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(peakMultiplier, 0m);
        this.schedule = schedule;
        this.peakRule = peakRule;
        this.peakMultiplier = peakMultiplier;
    }

    public string Name => "Standard Hourly";

    public static StandardHourlyPolicy Default(IHolidayCalendar holidays) =>
        new(HourlyRateSchedule.Standard, WeekdayPeakRule.Standard(holidays), 1.5m);

    public Money? Quote(ParkingSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var carFee = Money.Zero;
        for (var hour = 1; hour <= session.BillableHours; hour++)
        {
            var blockStart = session.EntryTime + (hour - 1) * Block;
            var rate = schedule.RateForHour(hour);
            carFee += peakRule.IsPeak(blockStart, blockStart + Block) ? rate.Times(peakMultiplier) : rate;
        }

        return session.Vehicle.Scale(carFee).Rounded();
    }
}
