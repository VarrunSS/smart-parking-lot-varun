using SmartParking.Domain;
using SmartParking.Pricing;

namespace SmartParking.Billing;

public sealed record FareQuote(string PolicyName, Money Fee);

/// <summary>Evaluates every policy and charges the lowest applicable fee. Knows nothing about specific policies.</summary>
public sealed class PricingEngine
{
    private readonly IReadOnlyList<IPricingPolicy> policies;

    public PricingEngine(IEnumerable<IPricingPolicy> policies)
    {
        ArgumentNullException.ThrowIfNull(policies);
        this.policies = [.. policies];
        if (this.policies.Count == 0)
        {
            throw new ArgumentException("At least one pricing policy is required.", nameof(policies));
        }
    }

    /// <summary>The cheapest applicable quote; on a tie the policy registered first wins.</summary>
    public FareQuote CalculateFare(ParkingSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        FareQuote? best = null;
        foreach (var policy in policies)
        {
            var fee = policy.Quote(session);
            if (fee is { } quoted && (best is null || quoted < best.Fee))
            {
                best = new FareQuote(policy.Name, quoted);
            }
        }

        return best ?? throw new InvalidOperationException("No pricing policy applies to this session.");
    }
}
