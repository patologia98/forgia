# OBJECTIVE.md — Forgia

---

## Vision

Forgia helps 3D printing bureaus generate accurate quotes from slicer files in seconds.

```
Import file → real cost breakdown → apply margin → export PDF
```

Not a slicer. Not a print farm controller. Not a cloud SaaS.

---

## Primary user

Single operator, 1–N FDM printers, currently quoting by hand (spreadsheets, gut feel).
Pain: underpriced jobs, no visibility on printer ROI, manual inventory.

---

## Success criteria (v1)

User can import a real 3MF/G-code, pick printer + filament, get accurate cost breakdown, export PDF — in under 60 seconds.

---

## Roadmap

### Phase 0 — Foundation
App boots. SQLite auto-created. CI green.

Done when: `dotnet run` launches MainWindow, DB migrated, build + tests pass.

### Phase 1 — Vertical slice ← current focus
Entities: Customer, Order, Project, Plate, Printer, FilamentSpool, LaborActivity.
Logic: QuoteCalculator (full formula), 3MF + G-code parser, PDF export.
UI: New Quote wizard — file drop, printer/filament pick, cost breakdown, PDF button.

Done when: real 3MF imports correctly and PDF quote exports correctly.

### Phase 2 — Inventory and waste
Filament stock tracking, spool depletion, failed-print logging, waste rate auto-calibration.

### Phase 3 — Bambu Lab P1S integration
MQTT LAN mode, live printer status, automatic print-attempt logging. Read-only first.

### Phase 4 — ROI dashboard
Per-printer: revenue vs cost, amortization burn, payback period.

### Phase 5 — Polish and release
Themed UI (Light/Dark), PDF template with logo, Settings screen, Windows installer.

---

## Non-goals for v1

Cloud sync, browser version, mobile, multi-user, fiscal invoicing, AI failure prediction, other printer brands (architecture supports it — we just don't ship drivers).

---

## Decision log

| Date | Decision | Rationale |
|---|---|---|
| 2026-05 | Avalonia UI | Cross-platform without UI rewrite later |
| 2026-05 | SQLite + EF Core | Zero-install for end user |
| 2026-05 | Apache 2.0 | Patent grant, compatible with future commercial offering |
| 2026-05 | Bambu MQTT LAN only | No cloud credentials in v1 |
| 2026-05 | English-only UI | Single locale reduces complexity; IT/ES deferred post-v1 |
| 2026-05 | Vertical slice first | Avoid building unused abstractions |
| 2026-05 | Lean architecture | Single maintainer, pre-alpha — YAGNI until second use case |

Add a row for every architectural decision that future contributors might question.
