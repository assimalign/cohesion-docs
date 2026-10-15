# CacheException

Domain exception raised by Cohesion cache implementations.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Caching`.

Assembly: `Assimalign.Cohesion.Caching`.

## Remarks

Domain exception raised by Cohesion cache implementations. Carries a `CacheErrorCode` so
callers can branch on the failure mode without text matching.

## Property

| Property | Description |
| --- | --- |
| `ErrorCode` | The diagnostics code attached to the exception. |

## `CacheErrorCode` values

| Code | Description |
| --- | --- |
| `Unknown` | Unclassified error. |
| `Disposed` | The cache has been disposed. |
| `InvalidEntry` | Committed entry failed validation (negative size, non-positive expiration, etc.). |
| `CapacityExceeded` | The entry could not fit even after eviction. |

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/Assembly/Assimalign.Cohesion.Caching/CacheException/OVERVIEW.md`.
