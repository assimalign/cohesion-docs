# Assimalign.Cohesion.Logging

Defines structured log entries, provider composition, filtering, enrichment, and scopes.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`ILogger`](i-logger.md)** — type reference.

- **[`ILoggerEnricher`](i-logger-enricher.md)** — type reference.

- **[`ILoggerEntry`](i-logger-entry.md)** — type reference.

- **[`ILoggerFactory`](i-logger-factory.md)** — type reference.

- **[`ILoggerFactoryBuilder`](i-logger-factory-builder.md)** — type reference.

- **[`ILoggerFilter`](i-logger-filter.md)** — type reference.

- **[`IScopedLogger`](i-scoped-logger.md)** — type reference.

- **[`Logger`](logger.md)** — type reference.

- **[`LoggerExtensions`](logger-extensions.md)** — type reference.

- **[`LoggerFilterRule`](logger-filter-rule.md)** — type reference.

- **[`LoggerProvider`](logger-provider.md)** — type reference.

- **[`LogLevel`](log-level.md)** — type reference.

- **[`ScopedLogger`](scoped-logger.md)** — type reference.

[Logging](../index.md)

## Scope

Providers consume one immutable entry model and concrete sinks remain separate. Factory filtering is
evaluated per provider. Scope entries correlate identifiers rather than creating an implicit ambient
stack, so adapters must preserve the actual correlation contract.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ILogger` | `src/Abstractions/ILogger.cs` |
| `ILoggerEnricher` | `src/Abstractions/ILoggerEnricher.cs` |
| `ILoggerEntry` | `src/Abstractions/ILoggerEntry.cs` |
| `ILoggerFactory` | `src/Abstractions/ILoggerFactory.cs` |
| `ILoggerFactoryBuilder` | `src/Abstractions/ILoggerFactoryBuilder.cs` |
| `ILoggerFilter` | `src/Abstractions/ILoggerFilter.cs` |
| `ILoggerProvider` | `src/Abstractions/ILoggerProvider.cs` |
| `IScopedLogger` | `src/Abstractions/ILogger.Scoped.cs` |
| `Logger` | `src/Logger.cs` |
| `LoggerEntry` | `src/LoggerEntry.cs` |
| `LoggerEntryBuilder` | `src/LoggerEntryBuilder.cs` |
| `LoggerExtensions` | `src/Extensions/LoggerExtensions.cs` |
| `LoggerFactory` | `src/LoggerFactory.cs` |
| `LoggerFactoryBuilder` | `src/LoggerFactoryBuilder.cs` |
| `LoggerFactoryOptions` | `src/LoggerFactoryOptions.cs` |
| `LoggerFilterRule` | `src/LoggerFilterRule.cs` |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Assimalign.Cohesion.Logging.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILogger.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerEnricher.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerEntry.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerFactory.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerFilter.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerProvider.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILogger.Scoped.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Logger.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerEntry.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerEntryBuilder.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Extensions/LoggerExtensions.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerFactory.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerFactoryOptions.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerFilterRule.cs`.
