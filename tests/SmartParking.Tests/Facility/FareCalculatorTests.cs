using SmartParking.Domain;
using SmartParking.Facility;
using SmartParking.Pricing;
using static SmartParking.Tests.Support.Sessions;

namespace SmartParking.Tests.Facility;

public class FareCalculatorTests
{
    private readonly FareCalculator calculator = new(PricingEngineFactory.CreateDefault());

    private static Ticket TicketFor(VehicleType vehicle, DateTime entry, LoyaltyTier? tier = null) =>
        new(vehicle, vehicle.RequiredSpot, entry, tier);

    [Fact]
    public void Calculates_the_fare_for_an_exit()
    {
        var ticket = TicketFor(VehicleType.Car, At(Monday, 6, 30), LoyaltyTier.Silver);

        Assert.Equal(new FareQuote("Early Bird", new Money(13.50m)), calculator.CalculateFare(ticket, At(Monday, 16)));
    }

    [Fact]
    public void Rejects_an_exit_before_entry()
    {
        var ticket = TicketFor(VehicleType.Car, At(Monday, 8));

        Assert.Throws<InvalidParkingSessionException>(() => calculator.CalculateFare(ticket, At(Monday, 7)));
    }

    [Fact]
    public void Motorcycle_and_bus_fares_flow_through_end_to_end()
    {
        var motorcycle = TicketFor(VehicleType.Motorcycle, At(Monday, 6, 30));
        var bus = TicketFor(VehicleType.Bus, At(Monday, 6, 30));

        Assert.Equal(new Money(9.60m), calculator.CalculateFare(motorcycle, At(Monday, 8, 30)).Fee);
        Assert.Equal(new Money(24.00m), calculator.CalculateFare(bus, At(Monday, 8, 30)).Fee);
    }

    [Fact]
    public void A_vehicle_added_as_data_is_priced_without_other_changes()
    {
        var van = new VehicleType("Van", 1.5m, SpotType.Large);

        Assert.Equal(new Money(7.50m), calculator.CalculateFare(TicketFor(van, At(Monday, 11)), At(Monday, 12)).Fee);
    }
}
