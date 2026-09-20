# ILoggerFilter

Per-entry predicate hung off a `LoggerFilterRule`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Per-entry predicate hung off a `LoggerFilterRule`. Receives the complete `ILoggerEntry` so
filters can branch on category, level, attributes, exception, or any combination of them.

## Method

Returning `true` admits the entry through this rule; returning `false` drops it for the
provider(s) the rule targets.

## Rules

- **Contract** — The filter runs only when its containing `LoggerFilterRule` has been selected for the
  current (provider, category) pair. Selection happens at logger creation time; the filter
  itself runs once per entry per matching provider.

- **Contract** — Filters run after the rule's `Level` check. If the rule has no `Level`, the filter is the
  sole gate.

- **Contract** — Implementations must be thread-safe; the filter runs on the caller's thread during
  `ILogger.Log(ILoggerEntry)`.

- **Contract** — A throwing filter is treated as `true` (admit). The composite swallows the exception so a
  bad filter cannot drop entries silently.

## Members

| Member | Responsibility |
|---|---|
| `ShouldLog(ILoggerEntry)` | Returns whether the accepted candidate proceeds to provider dispatch. |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ILoggerFilter/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Abstractions/ILoggerFilter.cs`.
