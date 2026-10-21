using SmartParking.Domain;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Pricing;

public class EarlyBirdPolicyTests
{
    private static readonly EarlyBirdPolicy Policy = new();

    private static decimal? Fee(ParkingSession session) => Policy.Quote(session)?.Amount;

    [Fact]
    public void Has_a_name()
    {
        Assert.Equal("Early Bird", Policy.Name);
    }

    [Theory]
    [InlineData(5, 59, 0, false)]
    [InlineData(6, 0, 0, true)]
    [InlineData(8, 59, 59, true)]
    [InlineData(9, 0, 0, false)]
    public void Entry_window_is_6am_inclusive_to_9am_exclusive(int hour, int minute, int second, bool eligible)
    {
        var session = Car(At(Monday, hour, minute, second), At(Monday, 17));

        Assert.Equal(eligible ? 15.00m : null, Fee(session));
    }

    [Theory]
    [InlineData(15, 29, 59, false)]
    [InlineData(15, 30, 0, true)]
    [InlineData(18, 59, 59, true)]
    [InlineData(19, 0, 0, false)]
    public void Exit_window_is_3_30pm_inclusive_to_7pm_exclusive(int hour, int minute, int second, bool eligible)
    {
        var session = Car(At(Monday, 7), At(Monday, hour, minute, second));

        Assert.Equal(eligible ? 15.00m : null, Fee(session));
    }

    [Fact]
    public void Requires_the_same_calendar_day()
    {
        Assert.Null(Fee(Car(At(Monday, 8), At(Tuesday, 16))));
    }

    [Fact]
    public void Applies_on_weekends_too()
    {
        Assert.Equal(15.00m, Fee(Car(At(Saturday, 8), At(Saturday, 17))));
    }

    [Theory]
    [MemberData(nameof(Tiers))]
    public void Applies_the_loyalty_discount_to_a_car(LoyaltyTier tier, decimal expected)
    {
        Assert.Equal(expected, Fee(Car(At(Monday, 8), At(Monday, 17), tier)));
    }

    public static TheoryData<LoyaltyTier, decimal> Tiers => new()
    {
        { LoyaltyTier.None, 15.00m },
        { LoyaltyTier.Silver, 13.50m },
        { LoyaltyTier.Gold, 12.00m },
        { LoyaltyTier.Platinum, 10.50m },
    };

    [Theory]
    [MemberData(nameof(VehicleFees))]
    public void Scales_the_flat_fee_by_the_vehicle_multiplier(VehicleType vehicle, LoyaltyTier tier, decimal expected)
    {
        Assert.Equal(expected, Policy.Quote(Of(vehicle, At(Monday, 8), At(Monday, 17), tier))!.Value.Amount);
    }

    public static TheoryData<VehicleType, LoyaltyTier, decimal> VehicleFees => new()
    {
        { VehicleType.Motorcycle, LoyaltyTier.None, 12.00m },
        { VehicleType.Bus, LoyaltyTier.None, 30.00m },
        { VehicleType.Bus, LoyaltyTier.Platinum, 21.00m },
        { VehicleType.Motorcycle, LoyaltyTier.Gold, 9.60m },
        { VehicleType.Motorcycle, LoyaltyTier.Silver, 10.80m },
        { VehicleType.Bus, LoyaltyTier.Gold, 24.00m },
    };
}
