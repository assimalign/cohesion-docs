# Cache

Cache contracts and an in-process memory-cache implementation.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Caching` | Defines cache entries, expiration, eviction, and typed access extensions. | [Overview](assimalign-cohesion-caching/index.md) |
| `Assimalign.Cohesion.Caching.InMemory` | Stores cache entries in a thread-safe in-process cache. | [Overview](assimalign-cohesion-caching-inmemory/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Caching` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Caching.InMemory` | `Assimalign.Cohesion.Caching` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src/Assimalign.Cohesion.Caching.csproj`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/src`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src/Assimalign.Cohesion.Caching.InMemory.csproj`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/src`.
