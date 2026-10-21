namespace SmartParking.Domain;

/// <summary>USD amount. Arithmetic stays exact; call <see cref="Rounded"/> once when a fee is final.</summary>
public readonly record struct Money(decimal Amount) : IComparable<Money>
{
    public static Money Zero => new(0m);

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);

    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;

    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;

    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;

    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;

    public static Money Min(Money first, Money second) => first <= second ? first : second;

    public Money Times(decimal factor) => new(Amount * factor);

    public Money Rounded() => new(Math.Round(Amount, 2, MidpointRounding.AwayFromZero));

    public int CompareTo(Money other) => Amount.CompareTo(other.Amount);

    public override string ToString() => string.Create(System.Globalization.CultureInfo.InvariantCulture, $"${Amount:0.00}");
}
