using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>Overnight flat rate: enter from 18:00 and leave 05:00-10:00 on the next consecutive calendar day.</summary>
public sealed class NightOwlPolicy() : DiscountedFlatRatePolicy(new Money(8m))
{
    private static readonly TimeWindow EntryWindow = TimeWindow.Between(18, 0, 24, 0);
    private static readonly TimeWindow ExitWindow = TimeWindow.Between(5, 0, 10, 0);

    public override string Name => "Night Owl";

    protected override bool IsEligible(ParkingSession session) =>
        session.EntryTime.Date.AddDays(1) == session.ExitTime.Date
        && EntryWindow.Contains(session.EntryTime)
        && ExitWindow.Contains(session.ExitTime);
}
