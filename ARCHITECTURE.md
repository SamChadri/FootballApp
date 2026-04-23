## Hybrid MAUI architecture (target)

### Modules

- **`src/Football.Core`**: pure domain models + business rules (no platform code)
- **`src/Football.Data.Sqlite`**: SQLite persistence for offline-first storage
- **`src/Football.Auth.Abstractions`**: auth contracts to support platform-specific sign-in
- **`Football.MauiApp`** (next phase): cross-platform UI and local device integration
- **Cloud API** (next phase): sync, auth federation, audit logs, access control

### Dependency rules

- UI depends on `Core`, `Data.*`, and `Auth.*`
- `Core` depends on nothing else in this repo
- `Data.*` depends on `Core`
- Platform-specific auth implementations depend on `Auth.Abstractions`

