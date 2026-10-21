namespace SmartParking.Pricing;

/// <summary>The single place that lists the active policies. Order matters: it breaks fare ties.</summary>
public static class PricingEngineFactory
{
    public static PricingEngine CreateDefault(IHolidayCalendar? holidays = null) =>
        new(CreateDefaultPolicies(holidays ?? new EmptyHolidayCalendar()));

    public static IReadOnlyList<IPricingPolicy> CreateDefaultPolicies(IHolidayCalendar holidays) =>
    [
        StandardHourlyPolicy.Default(holidays),
        new EarlyBirdPolicy(),
        new NightOwlPolicy(),
    ];
}
