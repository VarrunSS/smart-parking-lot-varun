using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class VehicleTypeTests
{
    [Theory]
    [MemberData(nameof(Vehicles))]
    public void Has_the_specified_multiplier_and_spot(VehicleType vehicle, decimal multiplier, SpotType spot)
    {
        Assert.Equal(multiplier, vehicle.RateMultiplier);
        Assert.Equal(spot, vehicle.RequiredSpot);
    }

    public static TheoryData<VehicleType, decimal, SpotType> Vehicles => new()
    {
        { VehicleType.Motorcycle, 0.8m, SpotType.Compact },
        { VehicleType.Car, 1.0m, SpotType.Compact },
        { VehicleType.Bus, 2.0m, SpotType.Large },
    };

    [Theory]
    [InlineData(SpotType.Compact, true)]
    [InlineData(SpotType.Large, false)]
    public void Car_fits_only_compact_spots(SpotType spot, bool fits)
    {
        Assert.Equal(fits, VehicleType.Car.FitsIn(spot));
    }

    [Fact]
    public void Bus_fits_only_large_spots()
    {
        Assert.True(VehicleType.Bus.FitsIn(SpotType.Large));
        Assert.False(VehicleType.Bus.FitsIn(SpotType.Compact));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rejects_a_non_positive_multiplier(double multiplier)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VehicleType("X", (decimal)multiplier, SpotType.Compact));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Rejects_a_blank_name(string name)
    {
        Assert.Throws<ArgumentException>(() => new VehicleType(name, 1m, SpotType.Compact));
    }

    [Fact]
    public void A_new_vehicle_is_just_data()
    {
        var van = new VehicleType("Van", 1.3m, SpotType.Large);
        Assert.True(van.FitsIn(SpotType.Large));
        Assert.Equal(1.3m, van.RateMultiplier);
    }
}
