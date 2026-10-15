# LoggerExtensions

Typed-level helpers over `ILogger`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Typed-level helpers over `ILogger`. Each helper builds a `LoggerEntry`, runs the level guard,
and writes to the logger.

## Methods

| Method | Level |
| --- | --- |
| `LogTrace(this ILogger, string category, string message, IReadOnlyDictionary<string, object?>? attributes = null)` | `Trace` |
| `LogDebug(this ILogger, string category, string message, IReadOnlyDictionary<string, object?>? attributes = null)` | `Debug` |
| `LogInformation(this ILogger, string category, string message, IReadOnlyDictionary<string, object?>? attributes = null)` | `Information` |
| `LogWarning(this ILogger, string category, string message, IReadOnlyDictionary<string, object?>? attributes = null)` | `Warning` |
| `LogError(this ILogger, string category, string message, Exception? exception = null, IReadOnlyDictionary<string, object?>? attributes = null)` | `Error` |
| `LogCritical(this ILogger, string category, string message, Exception? exception = null, IReadOnlyDictionary<string, object?>? attributes = null)` | `Critical` |
| `Log(this ILogger, LogLevel level, string category, string message, Exception? exception = null, IReadOnlyDictionary<string, object?>? attributes = null)` | explicit |
| `BeginScope(this ILogger, string category, string message, IReadOnlyDictionary<string, object?>? attributes = null)` | scope helper |

## Short-circuiting

Each helper calls `ILogger.IsEnabled(level)` first; when the logger is not enabled, no
`LoggerEntry` is allocated.

## Argument validation

All helpers throw `ArgumentNullException` for a null logger and `ArgumentException` for an
empty `category`.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/LoggerExtensions/OVERVIEW.md`.
