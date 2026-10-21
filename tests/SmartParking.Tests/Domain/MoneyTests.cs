using SmartParking.Domain;

namespace SmartParking.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void Adds_amounts()
    {
        Assert.Equal(new Money(7.50m), new Money(5m) + new Money(2.50m));
    }

    [Fact]
    public void Scales_by_a_factor_without_early_rounding()
    {
        Assert.Equal(7.5m, new Money(5m).Times(1.5m).Amount);
        Assert.Equal(4.8m, new Money(6m).Times(0.8m).Amount);
    }

    [Theory]
    [InlineData(1.005, 1.01)]
    [InlineData(1.004, 1.00)]
    [InlineData(2.675, 2.68)]
    [InlineData(-1.005, -1.01)]
    public void Rounds_to_cents_away_from_zero(double amount, double expected)
    {
        Assert.Equal((decimal)expected, new Money((decimal)amount).Rounded().Amount);
    }

    [Fact]
    public void Orders_by_amount()
    {
        Assert.True(new Money(1m) < new Money(2m));
        Assert.Equal(new Money(1m), Money.Min(new Money(2m), new Money(1m)));
    }

    [Fact]
    public void Formats_as_dollars()
    {
        Assert.Equal("$12.00", new Money(12m).ToString());
    }
}
