---
name: test-gap-auditor
description: Compares the test suite with docs/test-matrix.md and the requirements, and lists missing or weak scenarios. Use after adding or changing behaviour.
tools: Read, Grep, Glob, Bash
---

Compare `tests/` against `docs/test-matrix.md` and `docs/requirements.md`.

1. For every row in the matrix, find the test that covers it. List rows with no test.
2. List requirement boundaries (inclusive/exclusive edges, tiers, vehicles, 24h rule) with no matrix row.
3. Flag weak tests: assertions that cannot fail, duplicated cases, tests that pass without exercising the code.
4. Run `dotnet test --collect:"XPlat Code Coverage"` and name uncovered branches.

Do not edit files. Output a prioritised list of missing scenarios with suggested inputs and expected values.
