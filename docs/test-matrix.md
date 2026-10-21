# Test matrix

Every scenario with its expected value. Updated in the same commit as the tests. Status: ✅ covered by a test.

## Domain
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Money add / scale (5 x 1.5, 6 x 0.8) | exact decimals | MoneyTests | ✅ |
| Money rounding 1.005, 1.004, 2.675, -1.005 | 1.01, 1.00, 2.68, -1.01 (away from zero) | MoneyTests | ✅ |
| Money ordering, Min, ToString | `$12.00` | MoneyTests | ✅ |
| Vehicle multipliers Motorcycle / Car / Bus | 0.8 / 1.0 / 2.0 | VehicleTypeTests | ✅ |
| Spot fit: Motorcycle, Car to Compact only; Bus to Large only | true / false | VehicleTypeTests | ✅ |
| New vehicle defined as data only | works without other changes | VehicleTypeTests | ✅ |
| Loyalty discount None / Silver / Gold / Platinum on $15 | 15 / 13.50 / 12 / 10.50 | LoyaltyTierTests | ✅ |
| Discount rate outside 0..1 | throws | LoyaltyTierTests | ✅ |

## Ticket, session, time window
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Ticket records vehicle, spot, entry, tier; default tier None; unique ids | as given | TicketTests | ✅ |
| Car or Motorcycle in Large spot; Bus in Compact spot | `IncompatibleSpotException` | TicketTests | ✅ |
| Exit before entry | `InvalidParkingSessionException` | ParkingSessionTests | ✅ |
| Billable hours for 0, 1, 59, 60, 61, 120, 121 min and 3601 s | 1, 1, 1, 1, 2, 2, 3, 2 | ParkingSessionTests | ✅ |
| Exactly 24h vs 24h + 1s | not exceeding / exceeding one day | ParkingSessionTests | ✅ |
| TimeWindow 07:00-10:00 at 06:59, 07:00, 09:59, 10:00 | out, in, in, out | TimeWindowTests | ✅ |
| Window ending 24:00 contains 23:59:59, not 00:00 | true / false | TimeWindowTests | ✅ |
| Block overlap with window (before, partial start, inside, partial end, after, enclosing) | any-part overlap; block ending at window start does not overlap | TimeWindowTests | ✅ |
| Overlap checked on the given date only | next-day window not matched | TimeWindowTests | ✅ |
| Empty or out-of-range window | throws | TimeWindowTests | ✅ |

## Standard Hourly (car rates unless noted; reference week Mon 2026-03-02)
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Rate schedule hour 1 / 2 / 3 / 10 / 48; hour 0 | 5 / 3 / 2 / 2 / 2; throws | HourlyRateScheduleTests | ✅ |
| Spec example Mon 06:30-08:30 | $12.00 | StandardHourlyPolicyTests | ✅ |
| Off-peak 0 min, 1 min, 1h, 1h01, 2h, 3h, 5h | 5, 5, 5, 8, 8, 10, 14 | StandardHourlyPolicyTests | ✅ |
| Peak edges: block 06:00-07:00 / 06:59-07:59 / 07:00-08:00 / 09:00-10:00 / 10:00-11:00 | 5 / 7.50 / 7.50 / 7.50 / 5 | StandardHourlyPolicyTests, WeekdayPeakRuleTests | ✅ |
| Peak edges evening: 15:00-16:00 / 16:00-17:00 / 18:00-19:00 / 19:00-20:00 | 5 / 7.50 / 7.50 / 5 | StandardHourlyPolicyTests | ✅ |
| Surcharge per block: 15:30-17:30; 14:00-17:00 | 12.00; 11.00 | StandardHourlyPolicyTests | ✅ |
| Weekend (Sat 07:00, Sun 16:00); public holiday | no surcharge | StandardHourlyPolicyTests, WeekdayPeakRuleTests | ✅ |
| Fri 18:30-19:30 vs Sat 18:30-19:30 | 7.50 vs 5.00 | StandardHourlyPolicyTests | ✅ |
| Sun 23:30 to Mon 07:30 (weekend into weekday peak) | 21.00 | StandardHourlyPolicyTests | ✅ |
| Mon 08:00 to Wed 08:00 (48 blocks, 12 peak) | 114.00 | StandardHourlyPolicyTests | ✅ |
| Same, Tuesday a holiday | 108.00 | StandardHourlyPolicyTests | ✅ |
| Mon 11:00 to Tue 12:00 (more than 24h, 25 blocks, 6 peak) | 60.00 | StandardHourlyPolicyTests | ✅ |
| Vehicle multiplier on spec example: Motorcycle / Car / Bus | 9.60 / 12.00 / 24.00 | StandardHourlyPolicyTests | ✅ |
| Loyalty tier ignored | 12.00 | StandardHourlyPolicyTests | ✅ |

## Early Bird, Night Owl, shared flat-rate base
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Early Bird entry 05:59:00, 06:00, 08:59:59, 09:00 (exit 17:00) | n/a, 15, 15, n/a | EarlyBirdPolicyTests | ✅ |
| Early Bird exit 15:29:59, 15:30, 18:59:59, 19:00 (entry 07:00) | n/a, 15, 15, n/a | EarlyBirdPolicyTests | ✅ |
| Early Bird across two calendar days; on a Saturday | n/a; 15 | EarlyBirdPolicyTests | ✅ |
| Early Bird tiers None/Silver/Gold/Platinum (car) | 15 / 13.50 / 12 / 10.50 | EarlyBirdPolicyTests | ✅ |
| Early Bird Motorcycle / Bus / Bus Platinum / Motorcycle Gold | 12 / 30 / 21 / 9.60 | EarlyBirdPolicyTests | ✅ |
| Night Owl entry 17:59:59, 18:00, 23:59:59 (exit next day 06:00) | n/a, 8, 8 | NightOwlPolicyTests | ✅ |
| Night Owl entry Tue 00:00 | n/a | NightOwlPolicyTests | ✅ |
| Night Owl exit 04:59:59, 05:00, 09:59:59, 10:00 | n/a, 8, 8, n/a | NightOwlPolicyTests | ✅ |
| Night Owl exit two days later; Fri to Sat and Sat to Sun | n/a; 8, 8 | NightOwlPolicyTests | ✅ |
| Night Owl car tiers; Motorcycle None/Silver; Bus None/Gold | 8, 7.20, 6.40, 5.60; 6.40, 5.76; 16, 12.80 | NightOwlPolicyTests | ✅ |
| Flat base: exactly 24h valid, 24h + 1s invalid | quote / null | DiscountedFlatRatePolicyTests | ✅ |
| Flat base: multiplier then discount then round; not eligible | 7.20; null | DiscountedFlatRatePolicyTests | ✅ |

## Engine and default policy set
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Cheapest applicable policy wins; non-applicable ignored | min fee | PricingEngineTests | ✅ |
| Tie between equal fares | first registered | PricingEngineTests | ✅ |
| No policy applies; no policies supplied | `InvalidOperationException`; `ArgumentException` | PricingEngineTests | ✅ |
| New policy competes without engine changes | wins if cheaper | PricingEngineTests | ✅ |
| Spec best-value: car Mon 06:30-16:00 (hourly 31.00) | Early Bird 15.00; Silver 13.50 | DefaultPricingScenarioTests | ✅ |
| Car Mon 08:00-09:00 | Standard Hourly 7.50 | DefaultPricingScenarioTests | ✅ |
| Car Mon 22:00 to Tue 06:00 | Night Owl 8.00 | DefaultPricingScenarioTests | ✅ |
| Car Mon 06:30 to Tue 16:30 (34h) | Standard Hourly only | DefaultPricingScenarioTests | ✅ |
| Car Mon 18:00 to Tue 18:00 (exactly 24h) | Standard Hourly (matches no special) | DefaultPricingScenarioTests | ✅ |
| Bus Mon 07:00-17:00 | Early Bird 30.00 | DefaultPricingScenarioTests | ✅ |
| Holiday calendar injected through factory | 5.00 | DefaultPricingScenarioTests | ✅ |
| Every `IPricingPolicy` in the assembly is registered by the factory | sets equal | DefaultPricingScenarioTests | ✅ |

## Validation (from bug-reviewer finding)
| Scenario | Expected | Test class | Status |
|---|---|---|---|
| Vehicle multiplier 0 or negative; blank vehicle or tier name | throws | VehicleTypeTests, LoyaltyTierTests | ✅ |
| Negative hourly rate; peak multiplier 0 or negative | throws | HourlyRateScheduleTests, StandardHourlyPolicyTests | ✅ |
| Entry 06:30, exit 06:40: whole floating block judged for peak | 7.50 (documented spec reading) | StandardHourlyPolicyTests | ✅ |
