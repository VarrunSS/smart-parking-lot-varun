
namespace SmartParking.Domain;

/// <summary>A completed stay: a ticket plus the time the vehicle left.</summary>
public sealed class ParkingSession
{
    private static readonly TimeSpan OneDay = TimeSpan.FromHours(24);

    public ParkingSession(Ticket ticket, DateTime exitTime)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        if (exitTime < ticket.EntryTime)
        {
            throw new InvalidParkingSessionException($"Exit {exitTime:s} is before entry {ticket.EntryTime:s}.");
        }

        Ticket = ticket;
        ExitTime = exitTime;
    }

    public Ticket Ticket { get; }

    public DateTime ExitTime { get; }

    public DateTime EntryTime => Ticket.EntryTime;

    public VehicleType Vehicle => Ticket.Vehicle;

    public LoyaltyTier Loyalty => Ticket.Loyalty;

    public TimeSpan Duration => ExitTime - EntryTime;

    /// <summary>Strictly more than 24 hours; exactly 24 hours is still a single-day or overnight stay.</summary>
    public bool ExceedsOneDay => Duration > OneDay;

    /// <summary>Duration rounded up to whole hours, never less than one.</summary>
    public int BillableHours => (int)Math.Max(1, (Duration.Ticks + TimeSpan.TicksPerHour - 1) / TimeSpan.TicksPerHour);
}
