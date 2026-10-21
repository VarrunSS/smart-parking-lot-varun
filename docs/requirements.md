# Requirements (distilled from "The Smart Parking Lot_v2.pdf")

Single source of truth for behaviour. Assumptions that fill gaps are in `assumptions.md`.

## Goal
Billing engine: given a ticket and an exit time, return the **lowest valid fare** (USD, single currency).

## Domain
- Vehicles: Motorcycle (0.8x), Car (1.0x), Bus (2.0x). All rates below are CAR rates; multiply by the vehicle multiplier.
- Spots: Compact (Motorcycle, Car), Large (Bus).
- Ticket: issued at entry, carries the entry timestamp.
- Weekday = Mon-Fri excluding public holidays.

## Policy 1: Standard Hourly (with peak surcharge)
- Rate: $5.00 first hour, $3.00 second hour, $2.00 every hour after.
- Duration is rounded **up** to whole hours. Blocks float: block 1 starts exactly at entry time.
- A block is peak if **any part** overlaps a weekday peak window: 07:00-10:00 or 16:00-19:00 (start inclusive, end exclusive). A peak block costs 1.5x for that block.
- Example: car, weekday, 06:30-08:30. Block 1 = $5 x 1.5 = $7.50, block 2 = $3 x 1.5 = $4.50. Total $12.00.

## Policy 2: Early Bird (flat $15.00, loyalty discount)
- Entry in [06:00, 09:00) AND exit in [15:30, 19:00), same calendar day.

## Policy 3: Night Owl (flat $8.00, loyalty discount)
- Entry in [18:00, 23:59:59] AND exit in [05:00, 10:00) on the next consecutive calendar day.

## Loyalty discount (Policies 2 and 3)
SILVER 10%, GOLD 20%, PLATINUM 30%.

## Best value
Evaluate every applicable policy, charge the minimum. Example: car, weekday 06:30-16:00 may qualify for Standard and Early Bird.

## Over 24 hours
Stay **more than 24 hours** invalidates Early Bird and Night Owl. Only Standard Hourly applies.

## Assignment constraints (Assignment_Instructions.pdf)
- Judged on design and reasoning: domain modelling (behaviour separate from state, encapsulation), SOLID, KISS, YAGNI, readable and extensible code.
- Tests are mandatory for confidence; TDD appreciated. If tests were omitted, validation would have to be explained.
- No heavy frameworks, REST APIs or DBs. Standard library only.
- Required docs: README.md (assumptions, decisions, trade-offs, alternatives), AI interaction history, reproducible AI setup (skills, agents, commands, plans).
