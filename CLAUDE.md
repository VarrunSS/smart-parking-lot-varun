# Smart Parking Lot Rate Calculator

.NET 10 class library that returns the lowest valid parking fee. No UI, API or DB. Standard library only (xUnit for tests).

## Commands
- Build: `dotnet build -warnaserror`
- Test: `dotnet test`
- Coverage: `dotnet test --collect:"XPlat Code Coverage"`

## References (read when relevant)
- Behaviour and rules: @docs/requirements.md
- Gap decisions: @docs/assumptions.md
- Design and extension points: @docs/architecture.md
- Scenario list and expected values: @docs/test-matrix.md
- AI setup for reproduction: @docs/ai-workflow.md

## Rules
- SOLID, KISS, YAGNI. New policy or vehicle must not require editing the engine (use `/add-pricing-policy`, `/add-vehicle-type`).
- Test first. Every behaviour change updates tests and `docs/test-matrix.md` in the same commit.
- Money is `decimal`; never `double`.
- Run the `bug-reviewer` agent after each slice; fix only findings proven by a failing test or concrete trace.
- Never commit on `main`; work on `feature/rate-calculator`.
