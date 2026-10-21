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
        var one = new Money(1m);
        var two = new Money(2m);

        Assert.True(one < two);
        Assert.True(one <= two);
        Assert.True(two > one);
        Assert.True(two >= one);
        Assert.True(one <= new Money(1m));
        Assert.True(one >= new Money(1m));
        Assert.False(two < one);
        Assert.True(one.CompareTo(two) < 0);
    }

    [Fact]
    public void Formats_as_dollars()
    {
        Assert.Equal("$12.00", new Money(12m).ToString());
    }
}
