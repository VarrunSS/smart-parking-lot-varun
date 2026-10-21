using SmartParking.Domain;

namespace SmartParking.Pricing;

/// <summary>
/// Template for flat-rate specials: a car base fee scaled by vehicle, reduced by loyalty tier. Subclasses decide
/// only <see cref="IsEligible"/>. Stays longer than 24 hours are never eligible.
/// </summary>
public abstract class DiscountedFlatRatePolicy(Money carBaseFee) : IPricingPolicy
{
    public abstract string Name { get; }

    public Money? Quote(ParkingSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.ExceedsOneDay || !IsEligible(session))
        {
            return null;
        }

        return session.Loyalty.ApplyTo(session.Vehicle.Scale(carBaseFee)).Rounded();
    }

    protected abstract bool IsEligible(ParkingSession session);
}
