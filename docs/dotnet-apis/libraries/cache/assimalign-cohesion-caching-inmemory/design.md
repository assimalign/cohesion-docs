# Assimalign.Cohesion.Caching.InMemory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Caching.InMemory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`MemoryCache` implements the root entry and eviction contract. Expiration and invalidation remain
entry concerns, while capacity and compaction belong to the cache options. Disposing the cache and
disposing an entry have different roles: cache shutdown versus entry commit.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Caching`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src/Assimalign.Cohesion.Caching.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src`.
