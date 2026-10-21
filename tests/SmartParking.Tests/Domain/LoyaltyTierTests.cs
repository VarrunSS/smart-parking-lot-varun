using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class LoyaltyTierTests
{
    [Theory]
    [MemberData(nameof(Tiers))]
    public void Has_the_specified_discount(LoyaltyTier tier, decimal rate)
    {
        Assert.Equal(rate, tier.DiscountRate);
    }

    public static TheoryData<LoyaltyTier, decimal> Tiers => new()
    {
        { LoyaltyTier.None, 0m },
        { LoyaltyTier.Silver, 0.10m },
        { LoyaltyTier.Gold, 0.20m },
        { LoyaltyTier.Platinum, 0.30m },
    };

    [Theory]
    [MemberData(nameof(Discounts))]
    public void Applies_discount_to_a_fee(LoyaltyTier tier, decimal expected)
    {
        Assert.Equal(new Money(expected), tier.ApplyTo(new Money(15m)));
    }

    public static TheoryData<LoyaltyTier, decimal> Discounts => new()
    {
        { LoyaltyTier.None, 15m },
        { LoyaltyTier.Silver, 13.5m },
        { LoyaltyTier.Gold, 12m },
        { LoyaltyTier.Platinum, 10.5m },
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Rejects_a_blank_name(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new LoyaltyTier(name!, 0.1m));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(1.01)]
    public void Rejects_a_discount_outside_zero_to_one(double rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoyaltyTier("Bad", (decimal)rate));
    }
}
