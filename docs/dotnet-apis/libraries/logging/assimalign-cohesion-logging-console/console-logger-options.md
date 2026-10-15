# ConsoleLoggerOptions

Configuration shape for `ConsoleLoggerProvider`.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging.Console`.

## Remarks

Configuration shape for `ConsoleLoggerProvider`.

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Output` | `TextWriter?` | `null` -> `System.Console.Out` at write time | Standard output writer. |
| `ErrorOutput` | `TextWriter?` | `null` -> `System.Console.Error` at write time | Error output writer. |
| `IncludeAttributes` | `bool` | `true` | Render structured attributes inline. |
| `IncludeException` | `bool` | `true` | Render exception `ToString()` on a follow-up line. |
| `IncludeParentId` | `bool` | `false` | Render `parentId=...` for scoped entries. |
| `Formatter` | `Action<ILoggerEntry, TextWriter>?` | `null` | Override the built-in renderer. |

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/Assembly/Assimalign.Cohesion.Logging.Console/ConsoleLoggerOptions/OVERVIEW.md`.
- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/ConsoleLoggerOptions.cs`.
