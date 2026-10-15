# ILoggerEntry

Immutable structured log event handed to every provider in the fan-out pipeline.

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.Logging`.

Assembly: `Assimalign.Cohesion.Logging`.

## Remarks

Immutable structured log event handed to every provider in the fan-out pipeline.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Id` | `LogId` | Unique entry id, generated as a `Ulid`. |
| `ParentId` | `LogId?` | Optional parent id when the entry was produced inside an `IScopedLogger`. |
| `Timestamp` | `DateTimeOffset` | UTC wall-clock captured at construction. |
| `Level` | `LogLevel` | Severity. |
| `Category` | `string` | Logger category (typically a fully qualified type name). Non-empty. |
| `Message` | `string` | Human-readable message. Never null; may be empty. |
| `Exception` | `Exception?` | Optional captured exception. |
| `Attributes` | `IReadOnlyDictionary<string, object?>` | Structured key-value payload. Never null. |

## Notes

- **Contract** — Implementations are immutable; consumers MUST NOT mutate the entry or the attribute
  dictionary returned by `Attributes`.

- **Contract** — Construct entries through `LoggerEntryBuilder` or the typed helpers on `LoggerExtensions`.

- **Contract** — Attribute keys are case-sensitive. Values may be null; consumers that flatten attributes
  to text should render null as the literal string "null".

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/Assembly/Assimalign.Cohesion.Logging/ILoggerEntry/OVERVIEW.md`.
