# Forgia

**Pre-alpha. Not production-ready.**

Forgia is an open-source desktop quoting tool for 3D printing bureaus. Import a 3MF or G-code slicer file, select your printer and filament, get a full cost breakdown (material, electricity, amortization, maintenance, waste overhead), apply your margin, and export a PDF quote — all in under 60 seconds.

## Tech stack

| Area | Library | Version | License |
|---|---|---|---|
| Runtime | .NET | 8.0 | MIT |
| UI | Avalonia | 11.3.0 | MIT |
| UI toolkit | CommunityToolkit.Mvvm | 8.4.0 | MIT |
| Database | SQLite + EF Core | 8.0.11 | Apache 2.0 / MIT |
| Logging | Serilog | 4.2.0 | Apache 2.0 |
| Validation | FluentValidation | 11.11.0 | Apache 2.0 |
| Testing | xUnit | 2.9.3 | Apache 2.0 |
| Testing | FluentAssertions | 6.12.2 | Apache 2.0 |
| Testing | NSubstitute | 5.3.0 | BSD-3 |
| PDF | QuestPDF | 2024.10.4 | MIT |
| MQTT | MQTTnet | 4.3.7.1207 | MIT |

## Build

```
dotnet restore
dotnet build
dotnet test
```

Requires .NET 8 SDK. On first run, the SQLite database is created automatically at `%LOCALAPPDATA%/Forgia/forgia.db` (Linux: `~/.local/share/Forgia/forgia.db`).

## Roadmap

See [OBJECTIVE.md](OBJECTIVE.md).

## License

Apache 2.0 — see [LICENSE](LICENSE).
