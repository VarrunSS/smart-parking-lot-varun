# Smart Parking Lot Rate Calculator

A .NET 10 class library that computes the **lowest valid parking fee** for a ticket and an exit time. No UI, API or database; standard library only (xUnit for tests).

```csharp
var calculator = new FareCalculator(PricingEngineFactory.CreateDefault(holidayCalendar));
var ticket = new Ticket(VehicleType.Car, SpotType.Compact, entryTime, LoyaltyTier.Silver);
FareQuote fare = calculator.CalculateFare(ticket, exitTime);   // fare.PolicyName, fare.Fee
```

## Run
```
dotnet build -warnaserror
dotnet test
dotnet test --collect:"XPlat Code Coverage"     # about 95% line / 98% branch at last run
```

## Design
- **Strategy:** every rate rule implements `IPricingPolicy` (`Money? Quote(session)`, null = not applicable). `PricingEngine` evaluates all and returns the cheapest; it knows no concrete policy.
- **Template method:** Early Bird and Night Owl share the 24h guard, vehicle multiplier and loyalty discount in `DiscountedFlatRatePolicy`; each supplies only its eligibility windows (the spec's "reusable components" hint).
- **Factory:** `PricingEngineFactory` is the single place listing active policies. Registration order breaks ties.
- **Composition:** `StandardHourlyPolicy` is built from `HourlyRateSchedule` and `IPeakPeriodRule` (`WeekdayPeakRule` over an injected `IHolidayCalendar`).
- **Domain model:** state lives in small immutable value objects (`Money`, `VehicleType`, `LoyaltyTier`, `TimeWindow`); `Ticket` and `ParkingSession` enforce invariants (vehicle fits spot, exit not before entry).
- More detail and extension steps: [docs/architecture.md](docs/architecture.md).

### Flexibility
- Add or remove a policy: one class plus one line in `PricingEngineFactory`; a reflection test fails if a policy class is not registered.
- Add or remove a vehicle: one `VehicleType` instance (name, multiplier, spot). No policy or engine change.
- Change rates, peak windows or loyalty tiers: construct the value objects with other numbers.

## Assumptions
The spec leaves gaps; these were decided up front (full table with alternatives: [docs/assumptions.md](docs/assumptions.md)).
1. The vehicle multiplier applies to every policy, including the flat fees (bus Early Bird = $30).
2. Early Bird and Night Owl are not weekday-restricted; only the peak surcharge is.
3. Public holidays come from an injected `IHolidayCalendar` (default: none).
4. Each peak window is checked on its own calendar date, so blocks crossing midnight and multi-day stays are handled.
5. Exactly 24h00m still allows the specials; anything longer allows only Standard Hourly.
6. A zero-length stay bills the one-hour minimum; exit before entry is an error.
7. Loyalty tier travels on the ticket; no tier means no discount.
8. Money is `decimal`, kept exact internally and rounded to cents (away from zero) once per quote.
9. Times are local wall-clock `DateTime`; time zones and DST are out of scope.
10. Ties go to the earlier-registered policy; the result names the winning policy.
11. Scope is the billing engine plus the ticket and spot model; no capacity, allocation or persistence.
12. Night Owl entry "to 23:59:59" is treated as "before next midnight".
13. The 24h rule is encoded in the shared flat-rate base even though the specials' windows already imply it.

## Trade-offs and alternatives considered
- **Vehicles as data, not subclasses.** They differ only by multiplier and spot, so a hierarchy would add classes with no behaviour (YAGNI). If vehicles ever need different behaviour, `VehicleType` can become an abstract type without touching the engine.
- **Policies return `Money?` instead of a separate `IsApplicable` method.** One call, no way to price a policy that does not apply. Cost: "not applicable" has no reason attached.
- **Template method vs. a configurable `FlatRatePolicy`.** A data-only flat policy with injected eligibility rules would avoid inheritance, but two small subclasses are clearer than a rules mini-framework for two policies.
- **Whole floating block judged for peak.** A 06:30-06:40 stay pays 1.5x because its billed block overlaps 07:00. This follows the spec's partial-overlap rule and has a documented test.
- **No DI container, no logging, no configuration files.** Constructors take collaborators; the factory is plain code.
- **Considered and rejected:** computing the cheapest fare by simulating each minute (slower, harder to read); hard-coding holidays (region-specific, untestable).

## Testing and confidence
Test-first, per slice. [docs/test-matrix.md](docs/test-matrix.md) lists every scenario with its expected value and is updated with the tests. It covers the spec's worked examples, every inclusive/exclusive time boundary, each loyalty tier and vehicle, weekends and holidays, midnight and multi-day stays, the 24h rule, tie-breaking, and extensibility (a stub policy and a data-only vehicle work with no engine change).
Expected values for multi-day cases were worked out by hand before the code ran, so tests are not just echoing the implementation.

## AI usage
Built with Claude Code. Process, agents, skills and hooks are in [docs/ai-workflow.md](docs/ai-workflow.md) and `.claude/`. Every AI-written change was checked by tests I could read, and read-only reviewer agents (bug review twice, test-gap audit once) attacked the result; only findings backed by a failing test were acted on. The full prompt history is in `ai-chat-history.txt`.
