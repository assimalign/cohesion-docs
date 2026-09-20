# Assimalign.Cohesion.Caching design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Caching`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The root defines contracts rather than a runtime cache. An entry commits only when disposed, so
configuring an entry without disposal does not insert it. Implementations share the same eviction
vocabulary and expose typed convenience operations through `CacheExtensions`.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Assimalign.Cohesion.Caching.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src`.
