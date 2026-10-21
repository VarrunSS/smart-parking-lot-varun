namespace SmartParking.Domain;

/// <summary>
/// A class of vehicle. Vehicles differ only by data (rate multiplier and spot), so a new one is a new
/// instance rather than a new subclass.
/// </summary>
public sealed record VehicleType(string Name, decimal RateMultiplier, SpotType RequiredSpot)
{
    public static readonly VehicleType Motorcycle = new("Motorcycle", 0.8m, SpotType.Compact);
    public static readonly VehicleType Car = new("Car", 1.0m, SpotType.Compact);
    public static readonly VehicleType Bus = new("Bus", 2.0m, SpotType.Large);

    public bool FitsIn(SpotType spot) => spot == RequiredSpot;

    public Money Scale(Money carRate) => carRate.Times(RateMultiplier);
}
