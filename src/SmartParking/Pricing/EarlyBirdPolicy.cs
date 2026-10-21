using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>Commuter flat rate: enter 06:00-09:00 and leave 15:30-19:00 on the same calendar day.</summary>
public sealed class EarlyBirdPolicy() : DiscountedFlatRatePolicy(new Money(15m))
{
    private static readonly TimeWindow EntryWindow = TimeWindow.Between(6, 0, 9, 0);
    private static readonly TimeWindow ExitWindow = TimeWindow.Between(15, 30, 19, 0);

    public override string Name => "Early Bird";

    protected override bool IsEligible(ParkingSession session) =>
        session.EntryTime.Date == session.ExitTime.Date
        && EntryWindow.Contains(session.EntryTime)
        && ExitWindow.Contains(session.ExitTime);
}
