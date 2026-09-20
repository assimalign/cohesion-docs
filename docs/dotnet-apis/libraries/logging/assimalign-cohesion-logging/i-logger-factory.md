# ILoggerFactory

Roots the logging pipeline.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Roots the logging pipeline. Caches composite loggers per category, owns the registered
providers' lifecycle.

## Properties

| Property | Description |
| --- | --- |
| `Providers` | The providers fan-out targets registered with the factory. |

## Methods

| Method | Description |
| --- | --- |
| `ILogger Create(string category)` | Returns the cached composite logger for `category` (case-insensitive). |
| `void Dispose()` | Disposes every owned provider; subsequent operations throw `ObjectDisposedException`. |

## Exceptions

- **Contract** — `ArgumentException` for null or empty `category`.

- **Contract** — `ObjectDisposedException` when the factory has been disposed.

## Implementation

`LoggerFactory` is the default implementation. Build it through `LoggerFactoryBuilder`:

The factory's `Create` is thread-safe and lock-free for the cache hit path.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ILoggerFactory/OVERVIEW.md`.
