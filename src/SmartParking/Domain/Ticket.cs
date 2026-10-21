namespace SmartParking.Domain;

/// <summary>Issued at entry. Guarantees the vehicle fits the spot it was assigned.</summary>
public sealed class Ticket
{
    public Ticket(VehicleType vehicle, SpotType spot, DateTime entryTime, LoyaltyTier? loyalty = null)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        if (!vehicle.FitsIn(spot))
        {
            throw new IncompatibleSpotException($"{vehicle.Name} cannot park in a {spot} spot.");
        }

        Id = Guid.NewGuid();
        Vehicle = vehicle;
        Spot = spot;
        EntryTime = entryTime;
        Loyalty = loyalty ?? LoyaltyTier.None;
    }

    public Guid Id { get; }

    public VehicleType Vehicle { get; }

    public SpotType Spot { get; }

    public DateTime EntryTime { get; }

    public LoyaltyTier Loyalty { get; }
}
