using SmartParking.Domain;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Pricing;

public class NightOwlPolicyTests
{
    private static readonly NightOwlPolicy Policy = new();

    private static decimal? Fee(ParkingSession session) => Policy.Quote(session)?.Amount;

    [Fact]
    public void Has_a_name()
    {
        Assert.Equal("Night Owl", Policy.Name);
    }

    [Theory]
    [InlineData(17, 59, 59, false)]
    [InlineData(18, 0, 0, true)]
    [InlineData(23, 59, 59, true)]
    public void Entry_window_is_6pm_inclusive_to_end_of_day(int hour, int minute, int second, bool eligible)
    {
        var session = Car(At(Monday, hour, minute, second), At(Tuesday, 6));

        Assert.Equal(eligible ? 8.00m : null, Fee(session));
    }

    [Fact]
    public void Entry_just_after_midnight_is_outside_the_entry_window()
    {
        Assert.Null(Fee(Car(At(Tuesday, 0), At(Tuesday, 6))));
    }

    [Theory]
    [InlineData(4, 59, 59, false)]
    [InlineData(5, 0, 0, true)]
    [InlineData(9, 59, 59, true)]
    [InlineData(10, 0, 0, false)]
    public void Exit_window_is_5am_inclusive_to_10am_exclusive(int hour, int minute, int second, bool eligible)
    {
        var session = Car(At(Monday, 22), At(Tuesday, hour, minute, second));

        Assert.Equal(eligible ? 8.00m : null, Fee(session));
    }

    [Fact]
    public void Requires_exit_on_the_next_consecutive_day()
    {
        Assert.Null(Fee(Car(At(Monday, 19), At(Monday + 2, 6))));
    }

    [Fact]
    public void Applies_on_weekends_too()
    {
        Assert.Equal(8.00m, Fee(Car(At(Friday, 20), At(Saturday, 7))));
        Assert.Equal(8.00m, Fee(Car(At(Saturday, 20), At(Sunday, 7))));
    }

    [Fact]
    public void Works_across_a_month_and_year_boundary()
    {
        var session = Car(new DateTime(2026, 12, 31, 22, 0, 0), new DateTime(2027, 1, 1, 6, 0, 0));

        Assert.Equal(8.00m, Fee(session));
    }

    [Fact]
    public void Works_across_a_leap_day()
    {
        var session = Of(VehicleType.Motorcycle, new DateTime(2028, 2, 28, 22, 0, 0), new DateTime(2028, 2, 29, 6, 0, 0), LoyaltyTier.Platinum);

        Assert.Equal(4.48m, Fee(session));
    }

    [Theory]
    [MemberData(nameof(Fees))]
    public void Applies_vehicle_multiplier_and_loyalty_discount(VehicleType vehicle, LoyaltyTier tier, decimal expected)
    {
        Assert.Equal(expected, Policy.Quote(Of(vehicle, At(Monday, 22), At(Tuesday, 6), tier))!.Value.Amount);
    }

    public static TheoryData<VehicleType, LoyaltyTier, decimal> Fees => new()
    {
        { VehicleType.Car, LoyaltyTier.None, 8.00m },
        { VehicleType.Car, LoyaltyTier.Silver, 7.20m },
        { VehicleType.Car, LoyaltyTier.Gold, 6.40m },
        { VehicleType.Car, LoyaltyTier.Platinum, 5.60m },
        { VehicleType.Motorcycle, LoyaltyTier.None, 6.40m },
        { VehicleType.Motorcycle, LoyaltyTier.Silver, 5.76m },
        { VehicleType.Bus, LoyaltyTier.None, 16.00m },
        { VehicleType.Bus, LoyaltyTier.Gold, 12.80m },
        { VehicleType.Bus, LoyaltyTier.Platinum, 11.20m },
    };
}
