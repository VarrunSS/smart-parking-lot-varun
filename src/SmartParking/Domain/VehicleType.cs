namespace SmartParking.Domain;

/// <summary>
/// A class of vehicle. Vehicles differ only by data (rate multiplier and spot), so a new one is a new
/// instance rather than a new subclass.
/// </summary>
public sealed record VehicleType
{
    public static readonly VehicleType Motorcycle = new("Motorcycle", 0.8m, SpotType.Compact);
    public static readonly VehicleType Car = new("Car", 1.0m, SpotType.Compact);
    public static readonly VehicleType Bus = new("Bus", 2.0m, SpotType.Large);

    public VehicleType(string name, decimal rateMultiplier, SpotType requiredSpot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(rateMultiplier, 0m);
        Name = name;
        RateMultiplier = rateMultiplier;
        RequiredSpot = requiredSpot;
    }

    public string Name { get; }

    public decimal RateMultiplier { get; }

    public SpotType RequiredSpot { get; }

    public bool FitsIn(SpotType spot) => spot == RequiredSpot;

    public Money Scale(Money carRate) => carRate.Times(RateMultiplier);
}
