# LoggerProvider

Reusable abstract base class for `ILoggerProvider` implementations.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Reusable abstract base class for `ILoggerProvider` implementations. Owns the disposal flag,
category validation, and the bridge between the strongly typed (`Logger`) return type and
the `ILoggerProvider` interface.

## Members

| Member | Description |
| --- | --- |
| `abstract string Name { get; }` | Provider name; derived classes supply. |
| `bool IsDisposed { get; }` | True after `Dispose` has run. |
| `Logger Create(string category)` | Validates the category, throws `ObjectDisposedException` after disposal, and calls `CreateCore`. Covariant return: the public method returns `Logger` so callers holding a strongly typed reference avoid the interface cast. |
| `void Dispose()` | Idempotent; flips the disposed flag and calls `DisposeCore`. |
| `protected abstract Logger CreateCore(string category)` | Derived classes build the actual logger here. |
| `protected virtual void DisposeCore()` | Implementation hook; defaults to no-op. |

`ILoggerProvider.Create(string)` is implemented as an explicit bridge that delegates to the
typed `Create(string)`. Callers holding the interface still get an `ILogger`; callers holding
the concrete provider type get a `Logger`.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/LoggerProvider/OVERVIEW.md`.
