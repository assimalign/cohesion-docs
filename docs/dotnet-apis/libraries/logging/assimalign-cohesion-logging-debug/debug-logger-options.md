# DebugLoggerOptions

Configuration shape for `DebugLoggerProvider`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging.Debug`.

## Remarks

Configuration shape for `DebugLoggerProvider`.

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `EmitOnlyWhenDebuggerAttached` | `bool` | `true` | When true and `Writer` is null, the provider emits only while a debugger is attached. |
| `IncludeAttributes` | `bool` | `true` | Render structured attributes inline. |
| `IncludeException` | `bool` | `true` | Render exception `ToString()` on a follow-up line. |
| `Writer` | `Action<string>?` | `null` | Override `Debug.WriteLine`; useful for tests and tools. |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/Assembly/Assimalign.Cohesion.Logging.Debug/DebugLoggerOptions/OVERVIEW.md`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/DebugLoggerOptions.cs`.
