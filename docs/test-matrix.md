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
