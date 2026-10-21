using SmartParking.Billing;
using SmartParking.Domain;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Billing;

public class PricingEngineTests
{
    private sealed class FixedPolicy(string name, decimal? fee) : IPricingPolicy
    {
        public string Name => name;

        public Money? Quote(ParkingSession session) => fee is null ? null : new Money(fee.Value);
    }

    private static PricingEngine EngineOf(params IPricingPolicy[] policies) => new(policies);

    private static readonly ParkingSession AnySession = Car(At(Monday, 11), At(Monday, 12));

    [Fact]
    public void Picks_the_cheapest_applicable_policy()
    {
        var engine = EngineOf(new FixedPolicy("A", 12m), new FixedPolicy("B", 8m), new FixedPolicy("C", 9m));

        var fare = engine.CalculateFare(AnySession);

        Assert.Equal("B", fare.PolicyName);
        Assert.Equal(new Money(8m), fare.Fee);
    }

    [Fact]
    public void Ignores_policies_that_do_not_apply_even_if_they_would_be_cheaper()
    {
        var engine = EngineOf(new FixedPolicy("Costly", 20m), new FixedPolicy("NotApplicable", null));

        Assert.Equal("Costly", engine.CalculateFare(AnySession).PolicyName);
    }

    [Fact]
    public void A_tie_goes_to_the_policy_registered_first()
    {
        Assert.Equal("First", EngineOf(new FixedPolicy("First", 10m), new FixedPolicy("Second", 10m)).CalculateFare(AnySession).PolicyName);
        Assert.Equal("Second", EngineOf(new FixedPolicy("Second", 10m), new FixedPolicy("First", 10m)).CalculateFare(AnySession).PolicyName);
    }

    [Fact]
    public void Fails_loudly_when_no_policy_applies()
    {
        Assert.Throws<InvalidOperationException>(() => EngineOf(new FixedPolicy("None", null)).CalculateFare(AnySession));
    }

    [Fact]
    public void Requires_at_least_one_policy()
    {
        Assert.Throws<ArgumentException>(() => EngineOf());
    }

    [Fact]
    public void A_policy_added_later_competes_without_changing_the_engine()
    {
        var engine = EngineOf(new FixedPolicy("Existing", 10m), new FixedPolicy("Weekend Special", 4m));

        Assert.Equal("Weekend Special", engine.CalculateFare(AnySession).PolicyName);
    }
}
