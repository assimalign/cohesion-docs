# DatabaseOpenFailure

Immutable safe metadata for the last failed open of a stored database.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

```csharp
public sealed class DatabaseOpenFailure
{
    public DatabaseName Name { get; }
    public string? FailureCode { get; }
    public string ExceptionType { get; }
    public DateTimeOffset FailedAt { get; }
    public long Attempts { get; }
}
```

## Behavior

No public constructor. DatabaseEngine.FailedOpens publishes read-only snapshots. FailureCode
is the structured/coded diagnostic when available, otherwise null. ExceptionType retains the
type name; no message or exception object is retained. FailedAt is the last failure's UTC time;
Attempts counts consecutive failed opens. [Clearing and exclusions](database-engine.md#failedopens)
are owned by the engine. Accessors add no exceptions.

## Related reference

[Assembly](index.md) · [DatabaseEngine](database-engine.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseOpenFailure.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseFailedOpenTests.cs`.
