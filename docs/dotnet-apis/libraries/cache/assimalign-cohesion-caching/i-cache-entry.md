# ICacheEntry

Mutable, disposable handle to a cache entry.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Caching`.

Assembly: `Assimalign.Cohesion.Caching`.

## Remarks

Mutable, disposable handle to a cache entry. Configure it, then dispose to commit.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Key` | `object` | The key supplied to `ICache.CreateEntry`. |
| `Value` | `object?` | The value stored on commit. Defaults to `null`. |
| `AbsoluteExpiration` | `DateTimeOffset?` | Explicit UTC deadline. |
| `AbsoluteExpirationRelativeToNow` | `TimeSpan?` | Offset evaluated at commit. Must be > 0. |
| `SlidingExpiration` | `TimeSpan?` | Idle window. Must be > 0. Never extends past `AbsoluteExpiration`. |
| `ExpirationTokens` | `IList<IChangeToken>` | Tokens that, on notification, evict the entry with `TokenExpired`. |
| `PostEvictionCallbacks` | `IList<PostEvictionCallbackRegistration>` | Callbacks fired after eviction. |
| `Priority` | `CacheEntryPriority` | Eviction priority. Defaults to `Normal`. |
| `Size` | `long?` | Optional logical size. Required when the cache enforces a size limit. |

## Lifecycle

- **Contract** — Configure properties in any order.

- **Contract** — Call `Dispose` exactly once to commit. Calling `Dispose` a second time is a no-op.

- **Contract** — Disposing without ever setting `Value` commits an entry whose stored value is `null`.

## Validation on commit

Implementations should reject:

- **Contract** — `AbsoluteExpirationRelativeToNow <= TimeSpan.Zero` -> `CacheException(InvalidEntry)`.

- **Contract** — `SlidingExpiration <= TimeSpan.Zero` -> `CacheException(InvalidEntry)`.

- **Contract** — `Size < 0` -> `CacheException(InvalidEntry)`.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/Assembly/Assimalign.Cohesion.Caching/ICacheEntry/OVERVIEW.md`.
