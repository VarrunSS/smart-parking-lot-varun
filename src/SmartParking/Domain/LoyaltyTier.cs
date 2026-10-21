
namespace SmartParking.Domain;

public sealed record LoyaltyTier
{
    public static readonly LoyaltyTier None = new("None", 0m);
    public static readonly LoyaltyTier Silver = new("Silver", 0.10m);
    public static readonly LoyaltyTier Gold = new("Gold", 0.20m);
    public static readonly LoyaltyTier Platinum = new("Platinum", 0.30m);

    public LoyaltyTier(string name, decimal discountRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(discountRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discountRate, 1m);
        Name = name;
        DiscountRate = discountRate;
    }

    public string Name { get; }

    public decimal DiscountRate { get; }

    public Money ApplyTo(Money fee) => fee.Times(1m - DiscountRate);
}
