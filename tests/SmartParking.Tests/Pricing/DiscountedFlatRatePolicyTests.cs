using SmartParking.Domain;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Pricing;

public class DiscountedFlatRatePolicyTests
{
    /// <summary>Eligible for every session, so only the shared base rules are under test.</summary>
    private sealed class AlwaysEligiblePolicy() : DiscountedFlatRatePolicy(new Money(10m))
    {
        public override string Name => "Always";

        protected override bool IsEligible(ParkingSession session) => true;
    }

    private readonly AlwaysEligiblePolicy policy = new();

    [Fact]
    public void Exactly_24_hours_is_still_valid()
    {
        Assert.Equal(10.00m, policy.Quote(Car(At(Monday, 8), At(Tuesday, 8)))!.Value.Amount);
    }

    [Fact]
    public void More_than_24_hours_is_never_valid()
    {
        Assert.Null(policy.Quote(Car(At(Monday, 8), At(Tuesday, 8, 0, 1))));
    }

    [Fact]
    public void Applies_multiplier_then_discount_then_rounds_to_cents()
    {
        var session = Of(VehicleType.Motorcycle, At(Monday, 8), At(Monday, 9), LoyaltyTier.Silver);

        Assert.Equal(7.20m, policy.Quote(session)!.Value.Amount);
    }

    [Fact]
    public void Does_not_quote_when_not_eligible()
    {
        Assert.Null(new NeverEligiblePolicy().Quote(Car(At(Monday, 8), At(Monday, 9))));
    }

    private sealed class NeverEligiblePolicy() : DiscountedFlatRatePolicy(new Money(10m))
    {
        public override string Name => "Never";

        protected override bool IsEligible(ParkingSession session) => false;
    }
}
