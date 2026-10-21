namespace SmartParking.Pricing;

public interface IPeakPeriodRule
{
    /// <summary>True if any part of the half-open interval [blockStart, blockEnd) is a peak period.</summary>
    bool IsPeak(DateTime blockStart, DateTime blockEnd);
}
