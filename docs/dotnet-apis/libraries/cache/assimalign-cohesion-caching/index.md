# Assimalign.Cohesion.Caching

Defines cache entries, expiration, eviction, and typed access extensions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`CacheEntryPriority`](cache-entry-priority.md)** — type reference.

- **[`CacheEvictionReason`](cache-eviction-reason.md)** — type reference.

- **[`CacheException`](cache-exception.md)** — type reference.

- **[`CacheExtensions`](cache-extensions.md)** — type reference.

- **[`ICache`](i-cache.md)** — type reference.

- **[`ICacheEntry`](i-cache-entry.md)** — type reference.

- **[`PostEvictionCallbackRegistration`](post-eviction-callback-registration.md)** — type reference.

[Cache](../index.md)

## Scope

The root defines contracts rather than a runtime cache. An entry commits only when disposed, so
configuring an entry without disposal does not insert it. Implementations share the same eviction
vocabulary and expose typed convenience operations through `CacheExtensions`.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `CacheEntryPriority` | `src/CacheEntryPriority.cs` |
| `CacheErrorCode` | `src/Exceptions/CacheErrorCode.cs` |
| `CacheEvictionReason` | `src/CacheEvictionReason.cs` |
| `CacheException` | `src/Exceptions/CacheException.cs` |
| `CacheExtensions` | `src/Extensions/CacheExtensions.cs` |
| `ICache` | `src/Abstractions/ICache.cs` |
| `ICacheEntry` | `src/Abstractions/ICacheEntry.cs` |
| `PostEvictionCallbackRegistration` | `src/PostEvictionCallbackRegistration.cs` |

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Assimalign.Cohesion.Caching.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/CacheEntryPriority.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Exceptions/CacheErrorCode.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/CacheEvictionReason.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Exceptions/CacheException.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Extensions/CacheExtensions.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Abstractions/ICache.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Abstractions/ICacheEntry.cs`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/PostEvictionCallbackRegistration.cs`.
