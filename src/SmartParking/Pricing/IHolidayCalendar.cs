namespace SmartParking.Pricing;

public interface IHolidayCalendar
{
    bool IsHoliday(DateOnly day);
}

/// <summary>Default calendar with no public holidays.</summary>
public sealed class EmptyHolidayCalendar : IHolidayCalendar
{
    public bool IsHoliday(DateOnly day) => false;
}
