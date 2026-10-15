# Assimalign.Cohesion.Hosting

Coordinates plain host and hosted-service lifecycles.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Hosting](../index.md)

## Scope

Plain Hosting depends only on Core. Host execution and run observers do not own resource mounts,
process signals, control planes, or health reporting. Those opt-in concerns extend the run seam from
sibling packages rather than changing every host.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `BackgroundService` | `src/BackgroundService.cs` |
| `DedicatedThreadService` | `src/DedicatedThreadService.cs` |
| `Host<TContext>` | `src/Implementation/Host.TContext.cs` |
| `HostContext` | `src/Implementation/HostContext.cs` |
| `HostEnvironment` | `src/Implementation/HostEnvironment.cs` |
| `HostException` | `src/Exceptions/HostException.cs` |
| `HostOptions<TContext>` | `src/Implementation/HostOptions.TContext.cs` |
| `HostStartupException` | `src/Exceptions/HostStartupException.cs` |
| `HostState` | `src/HostState.cs` |
| `IHost` | `src/Abstractions/IHost.cs` |
| `IHostBuilder` | `src/Abstractions/IHostBuilder.cs` |
| `IHostContext` | `src/Abstractions/IHostContext.cs` |
| `IHostEnvironment` | `src/Abstractions/IHostEnvironment.cs` |
| `IHostLifecycleService` | `src/Abstractions/IHostLifecycleService.cs` |
| `IHostRun` | `src/Abstractions/IHostRun.cs` |
| `IHostRunner` | `src/Abstractions/IHostRunner.cs` |

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Assimalign.Cohesion.Hosting.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/BackgroundService.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/DedicatedThreadService.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/Host.TContext.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/HostContext.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/HostEnvironment.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Exceptions/HostException.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/HostOptions.TContext.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Exceptions/HostStartupException.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/HostState.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHost.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostBuilder.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostContext.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostEnvironment.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostLifecycleService.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostRun.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Abstractions/IHostRunner.cs`.
