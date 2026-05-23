# MEMORY.md

Append-only. One entry per session that changes architecture, domain, or integrations.
Claude Code reads this every session start.

Format: `## YYYY-MM-DD — summary / WHY: … / WHAT: … / BREAKS: … / NEXT: …`

---

## 2026-05-20 — Project charter and stack locked

WHY: Define stack, arch, formula, domain model before first line of code.
WHAT: CLAUDE.md (stack + rules + formula), OBJECTIVE.md (5-phase roadmap), GUIDELINES.md (C# / EF Core / OWASP / caveman rules), KICKOFF_PROMPT.md (Phase 0 bootstrap).
BREAKS: Nothing — pre-code.
NEXT: Run Phase 0 via KICKOFF_PROMPT. Install CRG + Graphify before writing src/.

## 2026-05-20 — Lean rewrite

WHY: Original bootstrap too enterprise-heavy for pre-alpha single-maintainer. Removed CQRS ceremony, mandatory generic repos, premature abstraction rules, over-specified i18n.
WHAT: CLAUDE.md slimmed. GUIDELINES.md condensed to 14 sections. OBJECTIVE.md simplified to 5 phases. Caveman + CRG/Graphify + full order formula retained.
BREAKS: Old verbose kickoff prompt deprecated.
NEXT: Phase 0 bootstrap.
