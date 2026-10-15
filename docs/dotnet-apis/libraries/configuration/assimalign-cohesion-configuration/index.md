# Assimalign.Cohesion.Configuration

Builds configuration from providers into keys, values, sections, and snapshots.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

Provider registration is separated from configuration consumption. `ConfigurationBuilder` gathers
factories, while `ConfigurationManager` supports a longer-lived orchestration role. Format-specific
parsing belongs in provider packages rather than the core object model.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Configuration` | `src/Configuration.cs` |
| `ConfigurationBindingAttribute` | `src/Decorators/ConfigurationBindingAttribute.cs` |
| `ConfigurationBindingAttribute<T>` | `src/Decorators/ConfigurationBindingAttribute.T.cs` |
| `ConfigurationBuilder` | `src/ConfigurationBuilder.cs` |
| `ConfigurationBuilderContext` | `src/ConfigurationBuilderContext.cs` |
| `ConfigurationErrorCode` | `src/Exceptions/ConfigurationErrorCode.cs` |
| `ConfigurationException` | `src/Exceptions/ConfigurationException.cs` |
| `ConfigurationManager` | `src/ConfigurationManager.cs` |
| `Key` | `src/ValueObjects/Key.cs` |
| `ConfigurationBinder` | `src/ConfigurationBinder.cs` |
| `ConfigurationBinderOptions` | `src/ConfigurationBinderOptions.cs` |
| `ConfigurationExtensions` | `src/Extensions/ConfigurationExtensions.Entry.cs` |
| `ConfigurationOptions` | `src/ConfigurationOptions.cs` |
| `ConfigurationProvider` | `src/ConfigurationProvider.cs` |
| `ConfigurationSetStrategy` | `src/ConfigurationSetStrategy.cs` |
| `IConfiguration` | `src/Abstractions/IConfiguration.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Assimalign.Cohesion.Configuration.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Configuration.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Decorators/ConfigurationBindingAttribute.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Decorators/ConfigurationBindingAttribute.T.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationBuilder.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationBuilderContext.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Exceptions/ConfigurationErrorCode.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Exceptions/ConfigurationException.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationManager.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ValueObjects/Key.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationBinder.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationBinderOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Extensions/ConfigurationExtensions.Entry.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationProvider.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/ConfigurationSetStrategy.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Abstractions/IConfiguration.cs`.
