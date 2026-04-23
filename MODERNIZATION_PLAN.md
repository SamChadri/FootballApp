## Phase 1 (now): cross-platform core scaffolding

### Goal
Build + test core/data on macOS with `dotnet test`, without touching UWP.

### Delivered in this phase
- `FootballHybrid.sln` containing new SDK-style projects under `src/` and `tests/`
- `Football.Core` with domain records
- `Football.Data.Sqlite` with initial repository + schema stub
- `Football.Auth.Abstractions` with `IAuthService`
- `Football.Tests` with a first SQLite integration test

### Next phase
- Create `Football.MauiApp` and wire:
  - db path provider using `FileSystem.AppDataDirectory`
  - DI container + navigation
  - minimal screens (list games, add game)

