using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class TicketTests
{
    private static readonly DateTime Entry = new(2026, 3, 2, 9, 0, 0);

    [Fact]
    public void Records_vehicle_spot_entry_time_and_tier()
    {
        var ticket = new Ticket(VehicleType.Bus, SpotType.Large, Entry, LoyaltyTier.Gold);

        Assert.Equal(VehicleType.Bus, ticket.Vehicle);
        Assert.Equal(SpotType.Large, ticket.Spot);
        Assert.Equal(Entry, ticket.EntryTime);
        Assert.Equal(LoyaltyTier.Gold, ticket.Loyalty);
    }

    [Fact]
    public void Defaults_to_no_loyalty_tier()
    {
        Assert.Equal(LoyaltyTier.None, new Ticket(VehicleType.Car, SpotType.Compact, Entry).Loyalty);
    }

    [Fact]
    public void Each_ticket_has_a_unique_id()
    {
        var first = new Ticket(VehicleType.Car, SpotType.Compact, Entry);
        var second = new Ticket(VehicleType.Car, SpotType.Compact, Entry);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Rejects_a_car_in_a_large_spot()
    {
        Assert.Throws<IncompatibleSpotException>(() => new Ticket(VehicleType.Car, SpotType.Large, Entry));
    }

    [Fact]
    public void Rejects_a_motorcycle_in_a_large_spot()
    {
        Assert.Throws<IncompatibleSpotException>(() => new Ticket(VehicleType.Motorcycle, SpotType.Large, Entry));
    }

    [Fact]
    public void Rejects_a_bus_in_a_compact_spot()
    {
        Assert.Throws<IncompatibleSpotException>(() => new Ticket(VehicleType.Bus, SpotType.Compact, Entry));
    }
}
