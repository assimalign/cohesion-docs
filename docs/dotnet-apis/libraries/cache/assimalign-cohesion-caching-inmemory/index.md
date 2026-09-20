# Assimalign.Cohesion.Caching.InMemory

Stores cache entries in a thread-safe in-process cache.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`MemoryCache`](memory-cache.md)** — type reference.

- **[`MemoryCacheOptions`](memory-cache-options.md)** — type reference.

[Cache](../index.md)

## Scope

`MemoryCache` implements the root entry and eviction contract. Expiration and invalidation remain
entry concerns, while capacity and compaction belong to the cache options. Disposing the cache and
disposing an entry have different roles: cache shutdown versus entry commit.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Caching`](../../cache/assimalign-cohesion-caching/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `MemoryCache` | `src/MemoryCache.cs` |
| `MemoryCacheOptions` | `src/MemoryCacheOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src/Assimalign.Cohesion.Caching.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src/MemoryCache.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src/MemoryCacheOptions.cs`.
