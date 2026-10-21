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
