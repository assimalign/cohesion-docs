# ScopedLogger

Reusable abstract base class for `IScopedLogger` implementations.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Reusable abstract base class for `IScopedLogger` implementations. Inherits the template-method
pattern from `Logger` and adds idempotent disposal plus a `ParentId` field.

## Constructor

Throws `ArgumentException` when `category` is null or empty.

## Members

| Member | Description |
| --- | --- |
| `LogId ParentId { get; }` | Id of the seed entry that opened the scope. |
| `bool IsDisposed { get; }` | True after `Dispose` has run. |
| `override bool IsEnabled(LogLevel level)` | Returns false when `IsDisposed`; otherwise delegates to `Logger.IsEnabled`. |
| `void Dispose()` | Idempotent; flips the disposed flag and calls `DisposeCore`. |
| `protected virtual void DisposeCore()` | Implementation hook; defaults to no-op. |
| Inherited `WriteCore`, `BeginScopeCore` | Derived classes implement these from `Logger`. |

## Behavior after disposal

- **Contract** — `IsEnabled(level)` returns false.

- **Contract** — `Log(entry)` short-circuits through `Logger.Log` (silent drop) - no exception thrown.

- **Contract** — `BeginScope(entry)` still creates a child scope, but operations on that child will also
  short-circuit because the parent scope's disposal cascades through `IsEnabled`. Derived
  classes can override `BeginScopeCore` to throw `ObjectDisposedException` if stricter
  semantics are required.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ScopedLogger/OVERVIEW.md`.
