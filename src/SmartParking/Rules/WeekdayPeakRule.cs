using SmartParking.Calendar;

namespace SmartParking.Rules;

/// <summary>Peak windows apply only on weekdays (Mon-Fri) that are not public holidays.</summary>
internal sealed class WeekdayPeakRule(IHolidayCalendar holidays, IReadOnlyList<TimeWindow> windows) : IPeakPeriodRule
{
    public static WeekdayPeakRule Standard(IHolidayCalendar holidays) =>
        new(holidays, [TimeWindow.Between(7, 0, 10, 0), TimeWindow.Between(16, 0, 19, 0)]);

    public bool IsPeak(DateTime blockStart, DateTime blockEnd)
    {
        var lastDay = DateOnly.FromDateTime(blockEnd > blockStart ? blockEnd.AddTicks(-1) : blockStart);
        for (var day = DateOnly.FromDateTime(blockStart); day <= lastDay; day = day.AddDays(1))
        {
            if (IsPeakDay(day) && windows.Any(window => window.Overlaps(day, blockStart, blockEnd)))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPeakDay(DateOnly day) =>
        day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && !holidays.IsHoliday(day);
}
