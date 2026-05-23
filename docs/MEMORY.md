# MEMORY.md

Append-only. One entry per session that changes architecture, domain, or integrations.
Claude Code reads this every session start.

Format: `## YYYY-MM-DD — summary / WHY: … / WHAT: … / BREAKS: … / NEXT: …`

---

## 2026-05-23 — Phase 1 vertical slice complete

WHY: First working end-to-end path: .3mf/.gcode → cost breakdown → PDF.
WHAT:
- Domain: Customer, Printer, FilamentSpool, Order, Project, Plate, LaborActivity entities. QuoteCalculator (canonical formula in Domain/Pricing/), PlateResult + OrderResult records.
- Infrastructure: EF Core mappings (InitialSchema migration replaces empty scaffold). BambuGcode3mfParser (reads slice_info.config from ZIP: prediction=seconds, weight=grams). BambuGcodeParser (header comment block). Result<T> for parse failures. QuoteDocument + QuoteExporter (QuestPDF, A4, full breakdown).
- UI: MainWindowViewModel (first-launch detection). SetupViewModel (printer + spool to DB). NewQuoteViewModel (parse → compute → export). FileDialogService (Avalonia StorageProvider). Strings.resx centralising all user-facing strings. SetupView + NewQuoteView (drag-drop, live breakdown, Export PDF button). App.axaml DataTemplates for ViewModel→View navigation. QuestPDF community license set at startup.
BREAKS: Old empty InitialSchema migration replaced — anyone with an existing forgia.db from Phase 0 must delete it.
NEXT: Phase 2 — filament stock tracking, spool depletion, waste rate auto-calibration.

## 2026-05-23 — Phase 0 complete

WHY: Bootstrap buildable solution before any domain logic.
WHAT: Forgia.sln + 4 src projects + 3 test projects. Clean Architecture dependency graph. Directory.Build.props (net10.0, Nullable, ImplicitUsings, TreatWarningsAsErrors on Domain+Application). Directory.Packages.props (centralized NuGet). .editorconfig (Microsoft defaults). .gitignore. Apache 2.0 LICENSE + NOTICE. Avalonia 11.3 shell (MainWindow shows name + version). Generic Host + Serilog (rolling file + console) + ForgiaDbContext (SQLite, foreign keys). EF Core Initial migration applied at startup. CI workflow (restore, build -warnaserror, format --verify-no-changes, test, vulnerability check). code-review-graph 2.3.3 installed + built (61 nodes, 135 edges, 7 communities).
BREAKS: Nothing — first code.
NEXT: Phase 1 — owner approval required before starting.
