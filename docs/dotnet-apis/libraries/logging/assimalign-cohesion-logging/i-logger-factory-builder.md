# ILoggerFactoryBuilder

Fluent registration surface for `ILoggerFactory`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Fluent registration surface for `ILoggerFactory`. Build the factory in a single thread, then
publish the resulting (thread-safe) factory.

## Properties

| Property | Description |
| --- | --- |
| `bool HasForwarders` | Gets whether at least one forwarder has been registered with this builder, without creating it. |

Hosting defaults use this probe to preserve application-selected forwarding. The interface defaults
to `false` for compatibility with existing implementations; an implementation that records forwarder
registrations should override it. `LoggerFactoryBuilder` reads its registration count.

## Methods

| Method | Description |
| --- | --- |
| `AddProvider(ILoggerProvider provider)` | Registers a provider. Duplicate names throw `InvalidOperationException`. |
| `SetMinimumLevel(LogLevel level)` | Factory-wide minimum level. Used as the fallback when no `LoggerFilterRule` matches a (provider, category) pair. Defaults to `Information`. |
| `AddRule(LoggerFilterRule rule)` | Adds a rule to the filter ruleset. Rules are evaluated per (provider, category) pair via the selection algorithm documented on `LoggerFilterRule`. |
| `AddRule(string categoryPrefix, LogLevel minimumLevel)` | Convenience overload that constructs and adds a `LoggerFilterRule { Category = categoryPrefix, Level = minimumLevel }`. |
| `AddEnricher(ILoggerEnricher enricher)` | Adds an enricher to the pipeline. Enrichers run in registration order. |
| `AddForwarder(Func<ILoggerFactory, ILoggerForwarder> create)` | Registers a component that writes entries into the built factory. The factory creates it once during construction and disposes it before its providers. Forwarder names must be unique. |
| `Build()` | Materializes the factory. The builder is single-use; subsequent operations throw `InvalidOperationException`. |

## Exceptions

- **Contract** — `ArgumentNullException` for null `provider`, `rule`, or `enricher`.

- **Contract** — `ArgumentException` for an empty `categoryPrefix`.

- **Contract** — `InvalidOperationException` for duplicate provider names or reuse after `Build`.

## Implementation

`LoggerFactoryBuilder` is the default implementation. Builders are not thread-safe; treat
them as scratch space scoped to a single setup routine.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ILoggerFactoryBuilder/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/LoggerFactoryBuilder.cs`.
