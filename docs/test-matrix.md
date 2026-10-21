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
