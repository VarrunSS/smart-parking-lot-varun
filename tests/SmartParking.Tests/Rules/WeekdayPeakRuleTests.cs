using SmartParking.Calendar;
using SmartParking.Rules;
using SmartParking.Tests.Support;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Rules;

public class WeekdayPeakRuleTests
{
    private static readonly WeekdayPeakRule Rule = WeekdayPeakRule.Standard(new EmptyHolidayCalendar());

    [Theory]
    [InlineData(6, 0, 7, 0, false)]
    [InlineData(6, 59, 7, 59, true)]
    [InlineData(7, 0, 8, 0, true)]
    [InlineData(9, 0, 10, 0, true)]
    [InlineData(10, 0, 11, 0, false)]
    [InlineData(15, 0, 16, 0, false)]
    [InlineData(16, 0, 17, 0, true)]
    [InlineData(18, 0, 19, 0, true)]
    [InlineData(18, 30, 19, 30, true)]
    [InlineData(19, 0, 20, 0, false)]
    [InlineData(12, 0, 13, 0, false)]
    public void Weekday_blocks_are_peak_when_any_part_overlaps_a_window(int fh, int fm, int th, int tm, bool expected)
    {
        Assert.Equal(expected, Rule.IsPeak(At(Monday, fh, fm), At(Monday, th, tm)));
    }

    [Fact]
    public void Weekends_are_never_peak()
    {
        Assert.False(Rule.IsPeak(At(Saturday, 7, 0), At(Saturday, 8, 0)));
        Assert.False(Rule.IsPeak(At(Sunday, 16, 0), At(Sunday, 17, 0)));
    }

    [Fact]
    public void Holidays_are_never_peak()
    {
        var holiday = new FakeHolidayCalendar(new DateOnly(2026, 3, Monday));
        var rule = WeekdayPeakRule.Standard(holiday);

        Assert.False(rule.IsPeak(At(Monday, 7, 0), At(Monday, 8, 0)));
        Assert.True(rule.IsPeak(At(Tuesday, 7, 0), At(Tuesday, 8, 0)));
    }

    [Fact]
    public void A_block_crossing_midnight_uses_the_weekday_of_each_date()
    {
        // Sunday 23:30 to Monday 00:30 touches no window, but Sunday 06:30 to 07:30 would touch Monday's only if on Monday.
        Assert.False(Rule.IsPeak(At(PriorSunday, 23, 30), At(Monday, 0, 30)));
        Assert.True(Rule.IsPeak(At(Monday, 6, 30), At(Monday, 7, 30)));
    }
}
