# CacheEntryPriority

Relative eviction priority for `ICacheEntry`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Caching`.

Assembly: `Assimalign.Cohesion.Caching`.

## Remarks

Relative eviction priority for `ICacheEntry`.

| Value | Ordinal | Description |
| --- | --- | --- |
| `Low` | 0 | First to be evicted under capacity pressure. |
| `Normal` | 1 | Default. |
| `High` | 2 | Evicted only after lower-priority entries have been removed. |
| `NeverRemove` | 3 | Exempt from capacity-driven eviction. Still removed by explicit `Remove`, expiration, or token invalidation. |

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/Assembly/Assimalign.Cohesion.Caching/CacheEntryPriority/OVERVIEW.md`.
