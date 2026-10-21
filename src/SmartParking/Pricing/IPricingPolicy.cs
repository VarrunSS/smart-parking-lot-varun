using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>A rate rule. Returns the fee for a session, or null when the rule does not apply to it.</summary>
public interface IPricingPolicy
{
    string Name { get; }

    Money? Quote(ParkingSession session);
}
