# CLAUDE.md — Forgia

Read first every session. Rules here are binding.

---

## What Forgia is

Open-source desktop quoting tool for 3D printing bureaus.
Goal: 3MF/G-code → accurate quote → PDF in under 60 seconds.
Current priority: one complete vertical slice, nothing else.

github repository: https://github.com/patologia98/forgia

---

## Stack

| Area | Choice |
|---|---|
| Runtime | .NET 10 |
| UI | Avalonia 11 + CommunityToolkit.Mvvm |
| DB | SQLite + EF Core 8 |
| Logging | Serilog |
| Validation | FluentValidation |
| Testing | xUnit + FluentAssertions |
| PDF | QuestPDF |
| MQTT | MQTTnet |

No new libraries without owner approval.
Allowed licenses: MIT, Apache 2.0, BSD, MPL 2.0, MS-PL.
Forbidden: GPL, AGPL, LGPL.

---

## Architecture

```
Forgia.Domain          ← zero external deps
Forgia.Application     ← references Domain only
Forgia.Infrastructure  ← references Application + Domain
Forgia.UI              ← composition root, references everything
```

Rules:
- No EF Core / DbContext in Domain or Application.
- No Avalonia types outside UI.
- No business logic in ViewModels or code-behind.
- **Avoid abstraction until a second concrete use case exists.**

---

## Pricing formula

Canonical location: `Forgia.Domain/Pricing/QuoteCalculator.cs`. Never duplicate.

```
// Per plate
material_cost  = Σ (filament_usage.grams × spool.cost_per_kg / 1000)
electricity    = printer.power_w × print_time_h × electricity_€_per_kwh / 1000
amortization   = printer.purchase_cost × (print_time_h / printer.useful_life_h)
maintenance    = printer.maintenance_€_per_h × print_time_h
waste_overhead = (material_cost + electricity) × plate.waste_rate
plate_cost     = material_cost + electricity + amortization + maintenance + waste_overhead

// Per order
plates_cost    = Σ plate_cost
labor_cost     = Σ (labor_line.minutes × labor_line.hourly_rate / 60)
direct_cost    = plates_cost + labor_cost
quote_total    = direct_cost × (1 + margin_rate) × (1 + vat_rate)
```

Any change to this formula requires updated unit tests.

---

## Domain conventions

- Money: `decimal` only, never `double`.
- Percentages: stored 0..1.
- Durations: `TimeSpan`.
- Power: watts (`int`).
- Filament: grams (`decimal`).

---

## Session workflow

**Start:** read CLAUDE.md → OBJECTIVE.md → GUIDELINES.md → docs/MEMORY.md.

**Before opening source files:**
1. `get_minimal_context(task="…")` — ~100 tokens, tells you what to touch.
2. `get_impact_radius(symbol)` — before editing anything existing.
3. Only then open files with Read.

Install once: `pip install code-review-graph` + `claude skill add safishamsi/graphify` + `crg install-hooks`.

**End:** if architecture/domain/integration changed → append entry to docs/MEMORY.md. Format:
```
## YYYY-MM-DD — summary
WHY: …  WHAT: …  BREAKS: …  NEXT: …
```

---

## Commit style

Conventional Commits. Small commits. English only.
`feat:` `fix:` `refactor:` `test:` `docs:` `chore:`

---

## MUST NOT

- Add abstraction without a concrete second use case.
- Use `.Result` / `.Wait()` on Tasks.
- Put business logic in ViewModels or code-behind.
- Add cloud, auth, multi-user, analytics before slice is complete.
- Hard-code user-facing strings (use `Strings.resx`, English only).
- Duplicate the pricing formula.

---

## Response style

Caveman compression always. No filler. No pleasantries. Dense.

❌ "I have successfully updated the QuoteCalculator as requested."
✅ "Updated QuoteCalculator.cs — waste_overhead now uses printer historical avg if null."

---

## Repository layout

```
forgia/
├── CLAUDE.md
├── OBJECTIVE.md
├── GUIDELINES.md
├── README.md
├── LICENSE / NOTICE
├── Directory.Build.props
├── Directory.Packages.props
├── Forgia.sln
├── src/
│   ├── Forgia.Domain/
│   ├── Forgia.Application/
│   ├── Forgia.Infrastructure/
│   └── Forgia.UI/
├── tests/
│   ├── Forgia.Domain.Tests/
│   ├── Forgia.Application.Tests/
│   └── Forgia.Infrastructure.Tests/
└── docs/
    ├── MEMORY.md
    └── GRAPH_REPORT.md   ← Graphify output, regenerate on structural changes
```
