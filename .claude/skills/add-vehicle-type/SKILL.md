---
name: add-vehicle-type
description: Add or remove a vehicle type in the parking billing engine. Use when a new vehicle class or multiplier is requested.
---

1. Update the multiplier table in `docs/requirements.md` and the spot mapping if needed.
2. Add test rows to `docs/test-matrix.md` (multiplier on hourly and flat fees, spot compatibility).
3. Write failing tests first in `tests/SmartParking.Tests/Domain/`.
4. Add one `VehicleType` static instance in `src/SmartParking/Domain/VehicleType.cs` (name, multiplier, required `SpotType`). Add a new `SpotType` only if a new spot class is needed.
5. No policy or engine changes should be necessary; if one is, the design has regressed, so stop and fix the design.
6. Run `dotnet test`, then the `bug-reviewer` agent.
