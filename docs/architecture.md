# Architecture

## Shape
```
FareCalculator (Facility)          ticket + exit time -> FareQuote
   └─ PricingEngine                evaluates every IPricingPolicy, returns the cheapest applicable
        ├─ StandardHourlyPolicy    always applies; floating 1h blocks + peak surcharge
        ├─ EarlyBirdPolicy  ┐      both extend DiscountedFlatRatePolicy:
        └─ NightOwlPolicy   ┘      24h guard + vehicle multiplier + loyalty discount, subclass supplies IsEligible
PricingEngineFactory               the only place that lists active policies
```
Domain value objects (`Money`, `VehicleType`, `LoyaltyTier`, `TimeWindow`, `HourlyRateSchedule`) hold state and tiny pure behaviour. `Ticket` and `ParkingSession` guard their invariants (spot fits vehicle; exit not before entry).

## Patterns and why
| Pattern | Where | Reason |
|---|---|---|
| Strategy | `IPricingPolicy` | Each rate rule is independent and swappable; the engine never names a concrete policy. |
| Template method | `DiscountedFlatRatePolicy` | Policies 2 and 3 share the 24h guard, multiplier and discount (the spec calls this out). Subclasses define only eligibility. |
| Factory | `PricingEngineFactory` | One composition point; adding or removing a policy is a one-line change. Order also defines tie-breaking. |
| Composition | `StandardHourlyPolicy` takes `HourlyRateSchedule` and `IPeakPeriodRule` | Rates and peak definition change independently of the block algorithm. |
| Dependency inversion | `IHolidayCalendar`, `IPeakPeriodRule` | Holidays are external data; tests inject fakes. |
| Data-driven value object | `VehicleType`, `LoyaltyTier` | Vehicles and tiers differ only by data, so a subclass per vehicle would be speculative (YAGNI). |

## Extension points
- **New policy:** implement `IPricingPolicy` (or extend `DiscountedFlatRatePolicy`), register in `PricingEngineFactory`. A reflection test fails if one is forgotten. See skill `add-pricing-policy`.
- **Remove a policy:** delete it from the factory list and its class and tests.
- **New vehicle:** add one `VehicleType` instance (and a `SpotType` only if needed). See skill `add-vehicle-type`.
- **New loyalty tier:** one `LoyaltyTier` instance.
- **Different peak hours or rates:** construct `WeekdayPeakRule` / `HourlyRateSchedule` with other values.

## Deliberately not built (YAGNI)
Spot allocation and capacity, persistence, customer lookup, time zones and DST, currency conversion, policy configuration files.
