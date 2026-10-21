using SmartParking.Billing;
using SmartParking.Calendar;
using SmartParking.Domain;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Billing;

/// <summary>End-to-end fares from the default policy set, including the spec's best-value scenarios.</summary>
public class DefaultPricingScenarioTests
{
    private static readonly PricingEngine Engine = PricingEngineFactory.CreateDefault();

    [Fact]
    public void Spec_best_value_case_weekday_6_30_to_4pm_picks_early_bird_over_standard()
    {
        var session = Car(At(Monday, 6, 30), At(Monday, 16));

        Assert.Equal(31.00m, StandardHourlyPolicy.Default(new EmptyHolidayCalendar()).Quote(session)!.Value.Amount);

        var fare = Engine.CalculateFare(session);
        Assert.Equal("Early Bird", fare.PolicyName);
        Assert.Equal(new Money(15m), fare.Fee);
    }

    [Fact]
    public void Loyalty_discount_makes_the_special_cheaper_still()
    {
        var fare = Engine.CalculateFare(Car(At(Monday, 6, 30), At(Monday, 16), LoyaltyTier.Silver));

        Assert.Equal(new FareQuote("Early Bird", new Money(13.50m)), fare);
    }

    [Fact]
    public void Short_stay_that_misses_every_special_is_billed_hourly()
    {
        var fare = Engine.CalculateFare(Car(At(Monday, 8), At(Monday, 9)));

        Assert.Equal(new FareQuote("Standard Hourly", new Money(7.50m)), fare);
    }

    [Fact]
    public void Overnight_stay_picks_night_owl()
    {
        var fare = Engine.CalculateFare(Car(At(Monday, 22), At(Tuesday, 6)));

        Assert.Equal(new FareQuote("Night Owl", new Money(8m)), fare);
    }

    [Fact]
    public void Stays_over_24_hours_fall_back_to_standard_hourly_only()
    {
        // Mon 06:30 -> Tue 16:30 would look like Early Bird entry/exit times, but the stay is 34 hours.
        var fare = Engine.CalculateFare(Car(At(Monday, 6, 30), At(Tuesday, 16, 30)));

        Assert.Equal("Standard Hourly", fare.PolicyName);
    }

    [Fact]
    public void Exactly_24_hours_keeps_specials_in_play_but_loses_to_hourly_here()
    {
        // Mon 18:00 -> Tue 18:00 matches neither special's exit window.
        // 24 blocks: base 5 + 3 + 22 x 2 = 52; Monday 18:00 block peak +2.50; Tuesday blocks starting 07, 08, 09, 16, 17 peak +1 each.
        var fare = Engine.CalculateFare(Car(At(Monday, 18), At(Tuesday, 18)));

        Assert.Equal(new FareQuote("Standard Hourly", new Money(59.50m)), fare);
    }

    [Fact]
    public void Bus_pays_double_for_the_special()
    {
        var fare = Engine.CalculateFare(Of(VehicleType.Bus, At(Monday, 7), At(Monday, 17)));

        Assert.Equal(new FareQuote("Early Bird", new Money(30m)), fare);
    }

    [Fact]
    public void Honours_the_injected_holiday_calendar()
    {
        var holiday = new Support.FakeHolidayCalendar(new DateOnly(2026, 3, Monday));
        var engine = PricingEngineFactory.CreateDefault(holiday);

        Assert.Equal(new Money(5m), engine.CalculateFare(Car(At(Monday, 7), At(Monday, 8))).Fee);
    }

    [Fact]
    public void Early_bird_still_applies_on_a_public_holiday_and_beats_the_hourly_fare()
    {
        var engine = PricingEngineFactory.CreateDefault(new Support.FakeHolidayCalendar(new DateOnly(2026, 3, Monday)));

        // Holiday hourly would be 5 + 3 + 8 x 2 = 24.
        Assert.Equal(new FareQuote("Early Bird", new Money(15m)), engine.CalculateFare(Car(At(Monday, 7), At(Monday, 17))));
    }

    [Fact]
    public void Every_policy_in_the_assembly_is_registered_in_the_default_set()
    {
        var implementations = typeof(IPricingPolicy).Assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IPricingPolicy)))
            .ToHashSet();

        var registered = PricingEngineFactory.CreateDefaultPolicies(new EmptyHolidayCalendar()).Select(policy => policy.GetType()).ToHashSet();

        Assert.NotEmpty(implementations);
        Assert.True(implementations.SetEquals(registered), "Register new policies in PricingEngineFactory (and only there).");
    }
}
