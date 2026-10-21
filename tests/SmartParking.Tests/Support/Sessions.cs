using SmartParking.Domain;
using SmartParking.Pricing;

namespace SmartParking.Tests.Support;

/// <summary>Test helpers. Reference week: Mon 2026-03-02 .. Sun 2026-03-08.</summary>
internal static class Sessions
{
    public const int PriorSunday = 1;
    public const int Monday = 2;
    public const int Tuesday = 3;
    public const int Friday = 6;
    public const int Saturday = 7;
    public const int Sunday = 8;

    public static DateTime At(int day, int hour, int minute = 0, int second = 0) => new(2026, 3, day, hour, minute, second);

    public static ParkingSession Of(VehicleType vehicle, DateTime entry, DateTime exit, LoyaltyTier? loyalty = null)
    {
        var spot = vehicle.RequiredSpot;
        return new ParkingSession(new Ticket(vehicle, spot, entry, loyalty), exit);
    }

    public static ParkingSession Car(DateTime entry, DateTime exit, LoyaltyTier? loyalty = null) =>
        Of(VehicleType.Car, entry, exit, loyalty);
}

internal sealed class FakeHolidayCalendar(params DateOnly[] holidays) : IHolidayCalendar
{
    public bool IsHoliday(DateOnly date) => holidays.Contains(date);
}
