---
name: add-pricing-policy
description: Add or remove a pricing policy in the parking billing engine. Use when a new rate rule is requested or an existing one is retired.
---

1. Add the rule to `docs/requirements.md` and any new gap to `docs/assumptions.md`.
2. Add scenarios (boundaries, vehicles, tiers) to `docs/test-matrix.md`.
3. Write failing tests in `tests/SmartParking.Tests/Pricing/` first.
4. Implement a class in `src/SmartParking/Pricing/` implementing `IPricingPolicy`. For a flat rate with loyalty discount, extend `DiscountedFlatRatePolicy` and supply only the eligibility rule and base fee.
5. Register or remove it in `PricingEngineFactory.CreateDefault`. This is the only place that lists policies; do not touch `PricingEngine`.
6. Run `dotnet test`; the registration reflection test must pass.
7. Run the `bug-reviewer` agent.
