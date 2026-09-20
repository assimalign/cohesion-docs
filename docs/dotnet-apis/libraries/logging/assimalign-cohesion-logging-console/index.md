# Assimalign.Cohesion.Logging.Console

Writes structured log entries to configurable output and error writers.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`ConsoleLoggerOptions`](console-logger-options.md)** — type reference.

- **[`ConsoleLoggerProvider`](console-logger-provider.md)** — type reference.

[Logging](../index.md)

## Scope

The provider separates routing from formatting and allows a custom formatter. Rendering is skipped
when an entry cannot be emitted. Console output remains a sink concern, leaving the structured event
contract in the logging foundation.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Logging`](../../logging/assimalign-cohesion-logging/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConsoleLoggerOptions` | `src/ConsoleLoggerOptions.cs` |
| `ConsoleLoggerProvider` | `src/ConsoleLoggerProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/Assimalign.Cohesion.Logging.Console.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/ConsoleLoggerOptions.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/ConsoleLoggerProvider.cs`.
