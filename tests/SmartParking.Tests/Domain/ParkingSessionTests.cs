using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class ParkingSessionTests
{
    private static readonly DateTime Entry = new(2026, 3, 2, 9, 0, 0);

    private static ParkingSession Session(TimeSpan stay) =>
        new(new Ticket(VehicleType.Car, SpotType.Compact, Entry), Entry + stay);

    [Fact]
    public void Rejects_an_exit_before_entry()
    {
        var ticket = new Ticket(VehicleType.Car, SpotType.Compact, Entry);

        Assert.Throws<InvalidParkingSessionException>(() => new ParkingSession(ticket, Entry.AddSeconds(-1)));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(59, 1)]
    [InlineData(60, 1)]
    [InlineData(61, 2)]
    [InlineData(120, 2)]
    [InlineData(121, 3)]
    public void Rounds_billable_time_up_to_whole_hours_with_a_one_hour_minimum(int minutes, int hours)
    {
        Assert.Equal(hours, Session(TimeSpan.FromMinutes(minutes)).BillableHours);
    }

    [Fact]
    public void Rounds_a_single_second_over_the_hour_up()
    {
        Assert.Equal(2, Session(TimeSpan.FromSeconds(3601)).BillableHours);
    }

    [Fact]
    public void Exactly_24_hours_is_not_longer_than_a_day()
    {
        var session = Session(TimeSpan.FromHours(24));

        Assert.False(session.ExceedsOneDay);
        Assert.Equal(24, session.BillableHours);
    }

    [Fact]
    public void One_second_beyond_24_hours_is_longer_than_a_day()
    {
        Assert.True(Session(TimeSpan.FromHours(24) + TimeSpan.FromSeconds(1)).ExceedsOneDay);
    }

    [Fact]
    public void Exposes_entry_exit_vehicle_and_loyalty_from_the_ticket()
    {
        var ticket = new Ticket(VehicleType.Bus, SpotType.Large, Entry, LoyaltyTier.Silver);
        var exit = Entry.AddHours(2);

        var session = new ParkingSession(ticket, exit);

        Assert.Equal(Entry, session.EntryTime);
        Assert.Equal(exit, session.ExitTime);
        Assert.Equal(VehicleType.Bus, session.Vehicle);
        Assert.Equal(LoyaltyTier.Silver, session.Loyalty);
    }
}
