using SmartParking.Domain;
using SmartParking.Pricing;

namespace SmartParking.Tests.Pricing;

public class HourlyRateScheduleTests
{
    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    [InlineData(10, 2)]
    [InlineData(48, 2)]
    public void Charges_5_then_3_then_2_per_hour(int hour, decimal expected)
    {
        Assert.Equal(new Money(expected), HourlyRateSchedule.Standard.RateForHour(hour));
    }

    [Fact]
    public void Rejects_hour_zero_or_negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => HourlyRateSchedule.Standard.RateForHour(0));
    }
}
