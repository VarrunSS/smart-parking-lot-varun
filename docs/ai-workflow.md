# AI workflow (reproducible setup)

Built with Claude Code (CLI). Everything needed to reproduce the process is in this repo.

## Setup
1. Install the .NET 10 SDK and Claude Code. Open the repo root in Claude Code.
2. Project context loads from `CLAUDE.md` (lean; links to `docs/`). Hooks and permissions come from `.claude/settings.json`.
3. Plugins used: the `superpowers` plugin (skills `brainstorming`, `test-driven-development`-style flow, plan mode) and the built-in `pdf` skill to read the two input PDFs.

## Project-defined pieces
| File | Purpose |
|---|---|
| `.claude/agents/bug-reviewer.md` | Read-only reviewer. A finding counts only with a concrete failing input. Run after each slice. |
| `.claude/agents/test-gap-auditor.md` | Compares tests with `docs/test-matrix.md` and requirements; names uncovered branches. |
| `.claude/skills/add-pricing-policy/SKILL.md` | Test-first checklist for adding or removing a policy. |
| `.claude/skills/add-vehicle-type/SKILL.md` | Checklist for adding or removing a vehicle. |
| `.claude/hooks/post-edit-build.ps1` | After editing a `.cs` file, builds with warnings as errors. |
| `.claude/hooks/stop-test-gate.ps1` | Blocks finishing while `dotnet test` is red. |

## Order of work (what was run)
1. Read both PDFs; wrote understanding, 13 gaps and assumptions (`docs/assumptions.md`) and got them approved.
2. Plan mode: produced and approved the plan (structure, design, test strategy, slices).
3. Slice loop, each test-first: failing tests, minimal code, `dotnet test`, update `docs/test-matrix.md`, commit.
   Slices: scaffold; Money/VehicleType/LoyaltyTier; Ticket/ParkingSession/TimeWindow; StandardHourlyPolicy; Early Bird and Night Owl; engine and factory; FareCalculator.
4. `bug-reviewer` ran in an isolated git worktree after the hourly slice, and again over the final code. The first run found constructor validation gaps; each was reproduced by a failing test, then fixed.
5. `test-gap-auditor` ran over the final suite; gaps whose expected values I re-derived by hand became new tests and matrix rows. The final bug review found no defects (it flagged unused `Money.Min`, which was removed).
6. Docs finalised (this folder and `README.md`).

## Verification commands
```
dotnet build -warnaserror
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

## Chat history
The full prompt history is supplied separately as `ai-chat-history.txt` by the author.
