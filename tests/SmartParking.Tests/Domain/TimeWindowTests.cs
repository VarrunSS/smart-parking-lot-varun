using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class TimeWindowTests
{
    private static readonly TimeWindow Morning = TimeWindow.Between(7, 0, 10, 0);

    private static DateTime At(int hour, int minute, int second = 0) => new(2026, 3, 2, hour, minute, second);

    [Theory]
    [InlineData(6, 59, false)]
    [InlineData(7, 0, true)]
    [InlineData(9, 59, true)]
    [InlineData(10, 0, false)]
    [InlineData(12, 0, false)]
    public void Contains_is_start_inclusive_and_end_exclusive(int hour, int minute, bool expected)
    {
        Assert.Equal(expected, Morning.Contains(At(hour, minute)));
    }

    [Fact]
    public void A_window_ending_at_midnight_contains_the_last_second_of_the_day()
    {
        var evening = TimeWindow.Between(18, 0, 24, 0);

        Assert.True(evening.Contains(At(23, 59, 59)));
        Assert.True(evening.Contains(At(18, 0)));
        Assert.False(evening.Contains(At(17, 59)));
        Assert.False(evening.Contains(At(0, 0)));
    }

    [Theory]
    [InlineData(6, 0, 7, 0, false)]
    [InlineData(6, 30, 7, 30, true)]
    [InlineData(7, 30, 8, 30, true)]
    [InlineData(9, 0, 10, 0, true)]
    [InlineData(9, 30, 10, 30, true)]
    [InlineData(10, 0, 11, 0, false)]
    [InlineData(5, 0, 12, 0, true)]
    public void Overlaps_a_block_if_any_part_falls_inside_the_window(int fromHour, int fromMinute, int toHour, int toMinute, bool expected)
    {
        var date = DateOnly.FromDateTime(At(0, 0));

        Assert.Equal(expected, Morning.Overlaps(date, At(fromHour, fromMinute), At(toHour, toMinute)));
    }

    [Fact]
    public void Overlap_is_evaluated_against_the_window_on_the_given_date()
    {
        var nextDay = DateOnly.FromDateTime(At(0, 0)).AddDays(1);

        Assert.False(Morning.Overlaps(nextDay, At(7, 0), At(8, 0)));
    }

    [Theory]
    [InlineData(10, 0, 7, 0)]
    [InlineData(7, 0, 7, 0)]
    [InlineData(-1, 0, 7, 0)]
    [InlineData(7, 0, 24, 1)]
    public void Rejects_an_empty_or_out_of_range_window(int startHour, int startMinute, int endHour, int endMinute)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TimeWindow.Between(startHour, startMinute, endHour, endMinute));
    }
}
