# DebugLoggerProvider

Debug-stream sink implementation of `Assimalign.Cohesion.Logging.ILoggerProvider`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging.Debug`.

## Remarks

Debug-stream sink implementation of `Assimalign.Cohesion.Logging.ILoggerProvider`.

## Constructors

The parameterless constructor uses default options. The second throws `ArgumentNullException`
when `options` is null.

## Members

| Member | Description |
| --- | --- |
| `Name` | Always `"Debug"`. |
| `Create(string category)` | Returns a per-category logger. Throws `ObjectDisposedException` after `Dispose`. |
| `Dispose()` | Marks the provider disposed; subsequent operations no-op or throw. |

## Gating

- **Contract** — When `Options.Writer` is null and `Options.EmitOnlyWhenDebuggerAttached` is `true`
  (default), the provider emits only while `System.Diagnostics.Debugger.IsAttached` is
  `true`.

- **Contract** — Setting `Options.Writer` automatically bypasses the gate; the caller has opted in to
  capture lines.

## Notes

- **Contract** — `BeginScope` emits the seed entry through the write path so scope-open events appear in
  the debug stream.

- **Contract** — A throwing `Writer` is swallowed; the provider stays usable.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/Assembly/Assimalign.Cohesion.Logging.Debug/DebugLoggerProvider/OVERVIEW.md`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/DebugLoggerProvider.cs`.
