# MEMORY.md

Append-only. One entry per session that changes architecture, domain, or integrations.
Claude Code reads this every session start.

Format: `## YYYY-MM-DD — summary / WHY: … / WHAT: … / BREAKS: … / NEXT: …`

---

## 2026-05-23 — Phase 0 complete

WHY: Bootstrap buildable solution before any domain logic.
WHAT: Forgia.sln + 4 src projects + 3 test projects. Clean Architecture dependency graph. Directory.Build.props (net8.0, Nullable, ImplicitUsings, TreatWarningsAsErrors on Domain+Application). Directory.Packages.props (centralized NuGet). .editorconfig (Microsoft defaults). .gitignore. Apache 2.0 LICENSE + NOTICE. Avalonia 11.3 shell (MainWindow shows name + version). Generic Host + Serilog (rolling file + console) + ForgiaDbContext (SQLite, foreign keys). EF Core Initial migration applied at startup. CI workflow (restore, build -warnaserror, format --verify-no-changes, test, vulnerability check). code-review-graph 2.3.3 installed + built (61 nodes, 135 edges, 7 communities).
BREAKS: Nothing — first code.
NEXT: Phase 1 — owner approval required before starting.
