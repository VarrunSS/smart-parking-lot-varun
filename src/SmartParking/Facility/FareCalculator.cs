using SmartParking.Domain;
using SmartParking.Pricing;

namespace SmartParking.Facility;

/// <summary>Entry point at the exit gate: turns a ticket and an exit time into the final fare.</summary>
public sealed class FareCalculator(PricingEngine pricing)
{
    public FareQuote CalculateFare(Ticket ticket, DateTime exitTime) =>
        pricing.CalculateFare(new ParkingSession(ticket, exitTime));
}
