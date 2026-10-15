# Example: Store and read a cache entry

Commit an entry to a memory cache and read it through the shared cache contract.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using Assimalign.Cohesion.Caching;
using Assimalign.Cohesion.Caching.InMemory;

using var cache = new MemoryCache();
cache.Set("k", "v");

bool found = cache.TryGetValue("k", out object? value);
int count = cache.Count;
```

## Walkthrough

`Set` creates and commits an entry through the cache extensions. The test checks that lookup
succeeds, the stored string is returned, and `Count` is one. Disposing the cache releases its
entries.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/tests/MemoryCacheTests.cs`.
- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Extensions/CacheExtensions.cs`.
