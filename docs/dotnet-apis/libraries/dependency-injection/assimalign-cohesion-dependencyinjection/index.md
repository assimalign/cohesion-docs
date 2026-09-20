# Assimalign.Cohesion.DependencyInjection

Registers services and resolves instances through providers and scopes.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[DependencyInjection](../index.md)

## Scope

Registration descriptors are separate from call-site execution and scope ownership.
`ServiceProviderOptions.EnableDynamicCode=false` selects the interpreted resolver before a compiled
engine is created. That option does not remove constructor reflection; explicit factories or
instances are needed for reflection-free construction.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AsyncServiceScope` | `src/Scopes/AsyncServiceScope.cs` |
| `ServiceDescriptor` | `src/ServiceDescriptor.cs` |
| `ServiceProvider` | `src/ServiceProvider.cs` |
| `ServiceProviderBuilder` | `src/ServiceProviderBuilder.cs` |
| `IServiceContainer` | `src/Abstractions/IServiceContainer.cs` |
| `IServiceFactory<out TService>` | `src/Abstractions/IServiceFactory.cs` |
| `IServiceLookup` | `src/Abstractions/IServiceLookup.cs` |
| `IServiceProviderBuilder` | `src/Abstractions/IServiceProviderBuilder.cs` |
| `IServiceProviderFactory` | `src/Abstractions/IServiceProviderFactory.cs` |
| `IServiceScope` | `src/Abstractions/IServiceScope.cs` |
| `IServiceScopeFactory` | `src/Abstractions/IServiceScopeFactory.cs` |
| `ISupportRequiredService` | `src/Abstractions/ISupportRequiredService.cs` |
| `ServiceContainer` | `src/ServiceContainer.cs` |
| `ServiceLifetime` | `src/ServiceLifetime.cs` |
| `ServiceProviderBuilderExtensions` | `src/Extensions/ServiceProviderBuilderExtensions.cs` |
| `ServiceProviderExtensions` | `src/Extensions/ServiceProviderExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Assimalign.Cohesion.DependencyInjection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Scopes/AsyncServiceScope.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/ServiceDescriptor.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/ServiceProvider.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/ServiceProviderBuilder.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceContainer.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceFactory.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceLookup.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceProviderBuilder.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceProviderFactory.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceScope.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/IServiceScopeFactory.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Abstractions/ISupportRequiredService.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/ServiceContainer.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/ServiceLifetime.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Extensions/ServiceProviderBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Extensions/ServiceProviderExtensions.cs`.
