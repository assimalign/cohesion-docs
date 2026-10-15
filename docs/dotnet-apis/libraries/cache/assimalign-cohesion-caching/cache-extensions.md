# CacheExtensions

Typed access on top of `ICache.TryGetValue` / `ICache.CreateEntry`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Caching`.

Assembly: `Assimalign.Cohesion.Caching`.

## Remarks

Typed access on top of `ICache.TryGetValue` / `ICache.CreateEntry`.

### `Get(ICache, object)`
Returns the stored value as `object?`, or `null` when the entry is missing.

### `Get<TValue>(ICache, object)`
Returns the stored value cast to `TValue`. Returns the default value when the entry is
missing or the stored value is of an incompatible runtime type.

### `TryGetValue<TValue>(ICache, object, out TValue)`
Strongly typed counterpart of `ICache.TryGetValue`. Emits the default value and returns
`false` when the entry is missing or the stored value is of an incompatible runtime type.

### `Set<TValue>(ICache, object, TValue)`
Creates and commits an entry, returning the committed value.

### `Set<TValue>(ICache, object, TValue, DateTimeOffset absoluteExpiration)`
As above, with an absolute expiration timestamp.

### `Set<TValue>(ICache, object, TValue, TimeSpan absoluteExpirationRelativeToNow)`
As above, with an absolute expiration evaluated against commit time.

### `Set<TValue>(ICache, object, TValue, IChangeToken expirationToken)`
As above, with a token-driven expiration.

### `GetOrCreate<TValue>(ICache, object, Func<ICacheEntry, TValue> factory)`
Returns the existing value when present; otherwise invokes the factory with a fresh
`ICacheEntry`, commits the factory's return value, and returns it.

## Argument validation

All overloads throw `ArgumentNullException` for any null required argument.

## Members

| Member | Responsibility |
|---|---|
| `Get(ICache, object)` | Returns the stored value or `null`. |
| `Get<TValue>(ICache, object)` | Returns a compatible value or the type's default. |
| `TryGetValue<TValue>` | Reports whether a compatible typed value was found. |
| `Set<TValue>` | Creates and commits an entry; overloads add absolute, relative, or token expiration. |
| `GetOrCreate<TValue>` | Reads an existing value or creates and commits a value from the supplied entry factory. |

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/Assembly/Assimalign.Cohesion.Caching/CacheExtensions/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Extensions/CacheExtensions.cs`.
