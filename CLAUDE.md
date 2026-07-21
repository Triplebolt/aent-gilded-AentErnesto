# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A training fork of the Gilded Rose Refactoring Kata. You inherit a working-but-messy
inventory system and change it *safely*. This repo is **C#-only** (.NET 10, C# 14,
xUnit). Note: `README.md` and `texttests/README.md` still mention a TypeScript variant
and its `npm`/`ts` commands — that implementation has been removed; ignore those references.

## Commands

All commands run from the repo root.

```bash
# Build
dotnet build csharp/GildedRose.sln

# Run all tests (unit + approval)
dotnet test csharp/GildedRose.sln

# Run a single test by name
dotnet test csharp/GildedRose.sln --filter "Name=KeepsItemNameUnchanged"
dotnet test csharp/GildedRose.sln --filter "Name=ThirtyDays"

# Run the app / print the N-day simulation to stdout
dotnet run --project csharp/GildedRose -- 30

# Golden-master check (defaults to C#). This is a bash script — on Windows run it
# from Git Bash, not PowerShell.
./scripts/golden-master-check.sh
```

## Three layers of behavioral protection

A change is only safe if all three still pass. They overlap deliberately — the unit
tests are intentionally thin, so the two output-capture checks are what actually guard
behavior:

1. **Unit test** — `csharp/GildedRoseTests/GildedRoseTest.cs`. A single sparse test.
   Do not assume it covers the behavior you're changing.
2. **Approval test** — `csharp/GildedRoseTests/ApprovalTest.cs` uses **Verify** to
   capture `Program.Main(["30"])` stdout and diff it against
   `ApprovalTest.ThirtyDays.verified.txt`. A mismatch writes a `.received.txt`; the
   test fails until the `.verified.txt` is updated.
3. **Golden master** — `texttests/ThirtyDays/stdout.gr` is the canonical 30-day output,
   diffed by `scripts/golden-master-check.sh` locally and in CI.

CI (`.github/workflows/ci.yml`) runs `dotnet test` plus the golden-master check on every
PR; the `ci-ok` job gates the merge. **Green CI is the merge condition.**

## Hard rules

- **The golden master is the law.** Any change to current behavior fails CI — even
  behavior that looks like a bug. If behavior *should* change, that's a conversation
  first, then a deliberate regeneration.
- **Never hand-edit** `texttests/ThirtyDays/stdout.gr` or `*.verified.txt` to force a
  passing check. Regenerate them from the program only after an agreed behavior change:
  - Golden master: `dotnet run --project csharp/GildedRose -- 30 > texttests/ThirtyDays/stdout.gr`
  - Verify snapshot: run the test, review the `.received.txt`, then promote it to `.verified.txt`.
  - After regenerating, `git diff` must show **only** the lines your change was meant to
    move. Anything else means behavior leaked.
- **Do not alter the `Item` class (`csharp/GildedRose/Item.cs`) or the `Items` property.**
  Per the kata spec they are off-limits; all logic changes go in `UpdateQuality`
  (`csharp/GildedRose/GildedRose.cs`) or new code.

## Architecture

- `csharp/GildedRose/` — the executable. `GildedRose.UpdateQuality()` holds the tangled
  legacy logic to refactor; `Program.Main` seeds a fixed item list and prints a
  day-by-day simulation (arg = number of days). Special-cased item names: `"Aged Brie"`,
  `"Sulfuras, Hand of Ragnaros"`, `"Backstage passes to a TAFKAL80ETC concert"`.
- `csharp/GildedRoseTests/` — xUnit tests (see the three layers above).
- `GildedRoseRequirements.md` — the business spec. Read it *against* the code; they
  don't always agree, and reconciling them is the point of the exercise.
- Quality is capped at 50 for normal items, but **Sulfuras is legendary**: it is fixed at
  80 and never changes — the 50 cap deliberately does not apply to it (per the spec).
- The **"Conjured"** item rule (degrade twice as fast) is specified in the requirements
  but **not yet implemented** — `Program.cs` seeds a "Conjured Mana Cake" with a
  `// this conjured item does not work properly yet` note. This is the feature to add.
