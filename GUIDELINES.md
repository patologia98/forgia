# GUIDELINES.md — Forgia

Lean engineering rules. Violations are blockers, not suggestions.

---

## 1. Project setup

- `net8.0`, C# 12, `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` on Domain + Application.
- `Directory.Build.props` for shared MSBuild props.
- `Directory.Packages.props` for centralized NuGet versions.
- `.editorconfig` from Microsoft defaults. Run `dotnet format --verify-no-changes` in CI.

---

## 2. Naming

- `PascalCase`: classes, methods, properties, interfaces (`I` prefix), constants.
- `camelCase`: locals, parameters.
- `_camelCase`: private fields. No Hungarian, no `m_`.
- Async methods end in `Async`.
- Booleans read as questions: `IsValid`, `HasFilament`, `CanPrint`.
- One public type per file. Braces always, even single-statement blocks.

---

## 3. Architecture

- Domain has zero project references.
- No `Microsoft.EntityFrameworkCore` in Domain or Application — ever.
- No `Avalonia.*` outside UI.
- Business logic stays in Domain. ViewModels expose state, not logic.
- **No abstraction without a second concrete use case.** No generic repositories, no CQRS ceremony, no mediator until justified.

---

## 4. Async

- All I/O is `async` / `await`. Never `.Result`, never `.Wait()`.
- Pass `CancellationToken` as last param on every public async method.
- `ConfigureAwait(false)` in Domain / Application / Infrastructure. Omit in UI.
- Long background work (MQTT, file watchers) → `BackgroundService`.

---

## 5. Error handling

- Exceptions for genuine unexpected failures.
- `Result<T>` where useful: validation, parsing, business-rule violations.
- Do not wrap everything in `Result<T>`. Keep it where it adds real value.
- Never catch `Exception` broadly unless you're at a process boundary and logging.
- Never swallow silently.

---

## 6. EF Core and SQL safety

Prefer LINQ — EF Core parameterizes automatically.

If raw SQL is unavoidable:
```csharp
// SAFE — EF parameterizes the interpolated expression
context.Items.FromSqlInterpolated($"SELECT * FROM Items WHERE Id = {id}");

// UNSAFE — string interpolation bypasses parameterization
context.Items.FromSqlRaw($"SELECT * FROM Items WHERE Id = {id}"); // ← SQL injection
```
Never concatenate user input into SQL. ([OWASP .NET Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/DotNet_Security_Cheat_Sheet.html))

SQLite path: `%LOCALAPPDATA%/Forgia/forgia.db`. Enable foreign keys in connection string.
Apply migrations at startup. Never edit a shipped migration — add a new one.

---

## 7. Validation

FluentValidation for DTO/command input. Validators live next to their use case.
Business invariants belong in Domain constructors/methods, not in external validators.

---

## 8. Logging

Serilog. Sinks: rolling file + console (debug only).
Structured: `_log.LogInformation("Quote {Id} total {Total}", id, total);`
Never log secrets, passwords, or API keys.

---

## 9. Testing

xUnit + FluentAssertions. No Moq — use NSubstitute.

Mandatory coverage:
- `QuoteCalculator` — all formula branches, edge cases.
- Slicer file parsers.
- Domain invariants.

Optional until vertical slice ships:
- ViewModel tests.
- Infrastructure integration tests.

Test naming: `Method_Condition_ExpectedResult`.

---

## 10. UI / MVVM

CommunityToolkit.Mvvm. `[ObservableProperty]`, `[RelayCommand]`, `[NotifyDataErrorInfo]`.
No code-behind logic. `InitializeComponent()` only in `.axaml.cs`.
Expose `IsBusy` on ViewModels for long operations. Wire `CancellationToken` to Cancel button.
Fluent theme, Light + Dark from day one. Colors in `Theme.axaml` — no literal hex in views.

---

## 11. Internationalization

English only. Single `Strings.resx` file, no language suffix.
No hard-coded user-facing strings in views or ViewModels.
Additional languages (IT, ES) deferred to post-v1 — architecture supports adding `.resx` siblings later without refactor.

---

## 12. Dependencies and licenses

Permitted: MIT, Apache 2.0, BSD-2/3, MS-PL, MPL 2.0.
Forbidden at runtime: GPL, AGPL, LGPL.
Every new package → add to CLAUDE.md stack table with version + license.
Run `dotnet list package --vulnerable` in CI. Vulnerable packages block merge.

---

## 13. Caveman compression

All Claude Code responses: dense, no filler, no pleasantries.
Drop articles where meaning is clear. No "I will now…", no "As requested…", no recap at end.
Applies to: agent replies, commit message bodies, inline comments.
Does not apply to: XML doc on public API, README, PDF output, user-visible error messages.

---

## 14. Review checklist

- [ ] `dotnet build -warnaserror` clean on touched projects.
- [ ] `dotnet test` passes.
- [ ] `dotnet format --verify-no-changes` clean.
- [ ] No new dep without license check + stack table update.
- [ ] No EF Core ref in Domain/Application.
- [ ] No Avalonia ref outside UI.
- [ ] No `FromSqlRaw` with string interpolation.
- [ ] No `.Result` / `.Wait()`.
- [ ] If formula changed: QuoteCalculator tests updated.
- [ ] If arch/domain/integration changed: MEMORY.md entry appended.
