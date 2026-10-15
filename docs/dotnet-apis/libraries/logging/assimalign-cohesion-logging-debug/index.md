# Assimalign.Cohesion.Logging.Debug

Writes structured log entries to diagnostic debug output.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`DebugLoggerOptions`](debug-logger-options.md)** — type reference.

- **[`DebugLoggerProvider`](debug-logger-provider.md)** — type reference.

[Logging](../index.md)

## Scope

The default gate requires an attached debugger. Options provide a writer seam for deterministic
tests and alternate output. The provider follows the same entry and scope contracts as other logging
sinks.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Logging`](../../logging/assimalign-cohesion-logging/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `DebugLoggerOptions` | `src/DebugLoggerOptions.cs` |
| `DebugLoggerProvider` | `src/DebugLoggerProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/Assimalign.Cohesion.Logging.Debug.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/DebugLoggerOptions.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/DebugLoggerProvider.cs`.
