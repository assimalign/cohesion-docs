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

## Summary

Cohesion logging foundation. Defines the structured log event model, logger and provider
contracts, factory composition (with a per-provider rule-based filter pipeline and enrichment),
and the scope lifecycle. Concrete sinks live in sibling packages.

## Status

- Status: Stable foundation.
- Production source files: 25.
- Project references: `Assimalign.Cohesion.Core` (for the `LogId` value type generator
  pipeline).
- Package references: None.
- `NotImplementedException` markers: 0.

## Primary Responsibilities

- Define `ILoggerEntry` as the immutable structured log event consumed by every provider.
- Define `ILogger`, `IScopedLogger`, `ILoggerProvider`, `ILoggerFactory`, and the builder.
- Provide a thread-safe `LoggerFactory` that caches composite loggers per category. The
  factory pre-resolves a single winning `LoggerFilterRule` per (provider, category) pair at
  composite-construction time, then gates entries per provider on level + optional
  `ILoggerFilter`.
- Document the scope, filter, and enrichment contracts so providers, hosts, and integration
  packages honor them consistently.

## Project Family

| Project | Purpose |
| --- | --- |
| `Assimalign.Cohesion.Logging` | Foundation contracts + factory composition. |
| `Assimalign.Cohesion.Logging.Console` | Console sink. |
| `Assimalign.Cohesion.Logging.Debug` | `System.Diagnostics.Debug` sink. |
| `Assimalign.Cohesion.Logging.EventSource` | Bridge: forwards `System.Diagnostics.Tracing` events (every Cohesion library's internal event source by default) into a factory. |

Future providers (file rolling, structured collectors, OpenTelemetry bridge) live in their
own packages and depend on the foundation.

## Dependency Direction

- The foundation depends only on `Assimalign.Cohesion.Core`.
- Every provider depends on the foundation.
- The foundation never depends on a provider.

## Key Types

- `ILoggerEntry` / `LoggerEntry` / `LoggerEntryBuilder` - the event model.
- `LogLevel` - severity enum (`Trace`, `Debug`, `Information`, `Warning`, `Error`,
  `Critical`, `Event`, `None`).
- `LogId` - generated `Ulid` wrapper with `Empty` / `New()` factories.
- `ILogger` / `IScopedLogger` - writer contracts.
- `Logger` / `ScopedLogger` - abstract base classes that implement the boilerplate with
  non-virtual hot paths so derived sinks pay one virtual dispatch instead of two.
- `ILoggerProvider` / `LoggerProvider` - sink factory contract + abstract base class.
- `ILoggerFactory` / `LoggerFactory` - root cache + fan-out with per-provider rule gating. Exposes
  its composition as read-only views: `Providers`, `Enrichers`, `Rules`, `Forwarders`.
  `LoggerFactory.Create(string)` returns the concrete `Logger` via covariant return.
- `ILoggerFactoryBuilder` / `LoggerFactoryBuilder` - fluent registration; `HasForwarders` reports
  whether an application already registered forwarding so hosting defaults can preserve it.
- `LoggerFactoryOptions` - mutable configuration shape consumed by the factory.
- `ILoggerEnricher` - attribute pipeline.
- `ILoggerForwarder` - a component that writes entries into the factory from an outside source
  (`AddForwarder` on the builder); the factory creates it when built and disposes it before its
  providers.
- `LoggerFilterRule` - one rule (provider type + category + level + custom filter, all
  optional). `ILoggerFilter` is the custom filter shape.
- `LoggerExtensions` - typed helpers (`LogTrace`, `LogInformation`, `LogError`, ...).

## Source Layout

- `src/Abstractions/` - root contracts (`ILogger`, `ILogger.Scoped.cs` (`IScopedLogger`),
  `ILoggerEntry`, `ILoggerFactory`, `ILoggerProvider`, `ILoggerFactoryBuilder`,
  `ILoggerEnricher`, `ILoggerFilter`, `ILoggerForwarder`).
- `src/Extensions/LoggerExtensions.cs` - ergonomic helpers.
- `src/Internal/CompositeLogger.cs` - per-category fan-out with per-provider rule gating.
- `src/Internal/ScopedCompositeLogger.cs` - scope lifecycle.
- `src/Internal/NoopScopedLogger.cs` - resilience helper.
- `src/Internal/LoggerFilterRuleSelector.cs` - rule selection algorithm.
- `src/LoggerEntry.cs`, `src/LoggerEntryBuilder.cs`, `src/LogLevel.cs` - event types.
- `src/Logger.cs`, `src/ScopedLogger.cs`, `src/LoggerProvider.cs` - reusable abstract base
  classes implementing the boilerplate with non-virtual hot paths.
- `src/LoggerFactory.cs`, `src/LoggerFactoryBuilder.cs`, `src/LoggerFactoryOptions.cs` -
  factory plumbing.
- `src/LoggerFilterRule.cs` - filter rule type.
- `src/ValueTypes/LogId.cs` - LogId source-generation spec (the actual file is generated
  under `obj/CodeGeneration/`).
- `src/Properties/AssemblyInfo.cs` - `InternalsVisibleTo` for tests.

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
