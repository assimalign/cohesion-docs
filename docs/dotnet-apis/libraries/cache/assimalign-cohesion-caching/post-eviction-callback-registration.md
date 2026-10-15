# PostEvictionCallbackRegistration

Pairs a `PostEvictionDelegate` with caller-supplied state.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Caching`.

Assembly: `Assimalign.Cohesion.Caching`.

## Remarks

Pairs a `PostEvictionDelegate` with caller-supplied state. Added to
`ICacheEntry.PostEvictionCallbacks`
to fire after the entry leaves the cache.

## Constructor

Throws `ArgumentNullException` when `callback` is null.

## Properties

| Property | Description |
| --- | --- |
| `EvictionCallback` | The delegate to invoke. |
| `State` | Caller state passed back to the callback. |

## Notes

- **Contract** — Callbacks fire after the entry is removed from the cache, never before.

- **Contract** — An exception thrown by one callback does not prevent the remaining callbacks for the same
  entry from firing.

- **Contract** — Callbacks may run on any thread; do not assume the disposing thread.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching/docs/Assembly/Assimalign.Cohesion.Caching/PostEvictionCallbackRegistration/OVERVIEW.md`.
