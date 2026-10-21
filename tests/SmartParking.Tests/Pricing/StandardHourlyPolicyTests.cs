using SmartParking.Domain;
using SmartParking.Pricing;
using SmartParking.Tests.Support;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Pricing;

public class StandardHourlyPolicyTests
{
    private static readonly StandardHourlyPolicy Policy = StandardHourlyPolicy.Default(new EmptyHolidayCalendar());

    private static decimal Fee(ParkingSession session) => Policy.Quote(session)!.Value.Amount;

    [Fact]
    public void Has_a_name_and_always_applies()
    {
        Assert.Equal("Standard Hourly", Policy.Name);
        Assert.NotNull(Policy.Quote(Car(At(Monday, 11), At(Monday, 12))));
    }

    [Fact]
    public void Spec_example_floating_blocks_6_30_to_8_30_is_12_dollars()
    {
        Assert.Equal(12.00m, Fee(Car(At(Monday, 6, 30), At(Monday, 8, 30))));
    }

    [Theory]
    [InlineData(11, 0, 11, 0, 5.00)]
    [InlineData(11, 0, 11, 1, 5.00)]
    [InlineData(11, 0, 12, 0, 5.00)]
    [InlineData(11, 0, 12, 1, 8.00)]
    [InlineData(11, 0, 13, 0, 8.00)]
    [InlineData(11, 0, 14, 0, 10.00)]
    [InlineData(11, 0, 16, 0, 14.00)]
    public void Charges_progressive_rates_off_peak(int fh, int fm, int th, int tm, double expected)
    {
        Assert.Equal((decimal)expected, Fee(Car(At(Monday, fh, fm), At(Monday, th, tm))));
    }

    [Theory]
    [InlineData(6, 0, 7, 0, 5.00)]
    [InlineData(6, 59, 7, 59, 7.50)]
    [InlineData(7, 0, 8, 0, 7.50)]
    [InlineData(9, 0, 10, 0, 7.50)]
    [InlineData(10, 0, 11, 0, 5.00)]
    [InlineData(15, 0, 16, 0, 5.00)]
    [InlineData(16, 0, 17, 0, 7.50)]
    [InlineData(18, 0, 19, 0, 7.50)]
    [InlineData(19, 0, 20, 0, 5.00)]
    public void Applies_the_peak_surcharge_with_inclusive_start_and_exclusive_end(int fh, int fm, int th, int tm, double expected)
    {
        Assert.Equal((decimal)expected, Fee(Car(At(Monday, fh, fm), At(Monday, th, tm))));
    }

    [Fact]
    public void Surcharge_applies_per_block_not_to_the_whole_stay()
    {
        // 15:30-17:30: block 1 (15:30-16:30) overlaps peak => 5 x 1.5; block 2 (16:30-17:30) peak => 3 x 1.5.
        Assert.Equal(12.00m, Fee(Car(At(Monday, 15, 30), At(Monday, 17, 30))));
        // 14:00-17:00: block 1 off-peak 5, block 2 (15-16) off-peak 3, block 3 (16-17) peak 2 x 1.5.
        Assert.Equal(11.00m, Fee(Car(At(Monday, 14), At(Monday, 17))));
    }

    [Fact]
    public void No_peak_surcharge_on_weekends()
    {
        Assert.Equal(5.00m, Fee(Car(At(Saturday, 7), At(Saturday, 8))));
        Assert.Equal(5.00m, Fee(Car(At(Sunday, 16), At(Sunday, 17))));
    }

    [Fact]
    public void No_peak_surcharge_on_public_holidays()
    {
        var policy = StandardHourlyPolicy.Default(new FakeHolidayCalendar(new DateOnly(2026, 3, Monday)));

        Assert.Equal(5.00m, policy.Quote(Car(At(Monday, 7), At(Monday, 8)))!.Value.Amount);
    }

    [Fact]
    public void Friday_evening_peak_applies_even_when_the_stay_runs_into_the_weekend()
    {
        Assert.Equal(7.50m, Fee(Car(At(Friday, 18, 30), At(Friday, 19, 30))));
        Assert.Equal(5.00m, Fee(Car(At(Saturday, 18, 30), At(Saturday, 19, 30))));
    }

    [Fact]
    public void Weekend_into_weekday_morning_peaks_only_for_the_weekday_block()
    {
        // Sun 23:30 -> Mon 07:30 = 8 blocks. 5 + 3 + 5 x 2 + last block (06:30-07:30 overlaps Monday peak) 2 x 1.5.
        Assert.Equal(21.00m, Fee(Car(At(PriorSunday, 23, 30), At(Monday, 7, 30))));
    }

    [Fact]
    public void Multi_day_stay_checks_every_block_against_its_own_date()
    {
        // Mon 08:00 -> Wed 08:00 = 48 blocks, 12 of them peak.
        // Off-peak 36 x 2 = 72; peak: 7.50 + 4.50 + 10 x 3.00 = 42 => 114.
        Assert.Equal(114.00m, Fee(Car(At(Monday, 8), At(Monday + 2, 8))));
    }

    [Fact]
    public void Multi_day_stay_drops_the_surcharge_on_a_holiday_in_the_middle()
    {
        var policy = StandardHourlyPolicy.Default(new FakeHolidayCalendar(new DateOnly(2026, 3, Tuesday)));

        // Tuesday's 6 peak blocks lose 1.00 each (3.00 -> 2.00).
        Assert.Equal(108.00m, policy.Quote(Car(At(Monday, 8), At(Monday + 2, 8)))!.Value.Amount);
    }

    [Fact]
    public void Applies_the_vehicle_multiplier_to_the_whole_fee()
    {
        var entry = At(Monday, 6, 30);
        var exit = At(Monday, 8, 30);

        Assert.Equal(9.60m, Policy.Quote(Of(VehicleType.Motorcycle, entry, exit))!.Value.Amount);
        Assert.Equal(12.00m, Policy.Quote(Of(VehicleType.Car, entry, exit))!.Value.Amount);
        Assert.Equal(24.00m, Policy.Quote(Of(VehicleType.Bus, entry, exit))!.Value.Amount);
    }

    [Fact]
    public void Ignores_loyalty_tier()
    {
        Assert.Equal(12.00m, Fee(Car(At(Monday, 6, 30), At(Monday, 8, 30), LoyaltyTier.Platinum)));
    }

    [Fact]
    public void Is_not_limited_to_24_hours()
    {
        // Mon 11:00 -> Tue 12:00 = 25 blocks, peak 16-19 on Monday (3 blocks) and 07-10 on Tuesday (3 blocks).
        // Blocks start 11:00, so Tue 07:00-10:00 blocks are starts 07,08,09; Mon starts 16,17,18.
        // 5 + 3 + 23 x 2 = 54 base; 6 peak blocks at rate 2 add 1 each => 60.
        Assert.Equal(60.00m, Fee(Car(At(Monday, 11), At(Tuesday, 12))));
    }
}
