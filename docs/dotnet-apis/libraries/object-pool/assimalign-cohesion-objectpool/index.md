# Assimalign.Cohesion.ObjectPool

Rents and returns reusable objects through factories and retention policies.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ObjectPool](../index.md)

## Scope

Object creation, return acceptance, and retention are separate decisions. Generic pool contracts
allow reusable callers while a default implementation supplies standard retention. Returning rented
objects is the caller's responsibility, including exceptional paths.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `CancellationTokenSourcePool` | `src/CancellationTokenSourcePool.cs` |
| `DefaultObjectPool<T, TArgs>` | `src/DefaultObjectPool.T.TArgs.cs` |
| `DefaultObjectPool<T>` | `src/DefaultObjectPool.T.cs` |
| `ObjectPool<T, TArgs>` | `src/ObjectPool.T.TArgs.cs` |
| `ObjectPool<T>` | `src/ObjectPool.T.cs` |
| `ObjectPoolExtensions` | `src/Extensions/ObjectPoolExtensions.cs` |
| `ObjectPoolFactory<T, TArgs>` | `src/ObjectPoolFactory.T.TArgs.cs` |
| `ObjectPoolFactory<T>` | `src/ObjectPoolFactory.T.cs` |
| `ObjectPoolPolicy<T>` | `src/ObjectPoolPolicy.T.cs` |

## Sources

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/Assimalign.Cohesion.ObjectPool.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/CancellationTokenSourcePool.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/DefaultObjectPool.T.TArgs.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/DefaultObjectPool.T.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/ObjectPool.T.TArgs.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/ObjectPool.T.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/Extensions/ObjectPoolExtensions.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/ObjectPoolFactory.T.TArgs.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/ObjectPoolFactory.T.cs`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/ObjectPoolPolicy.T.cs`.
