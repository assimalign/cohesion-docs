# DatabaseHealthObservation

Immutable transport-neutral component observation.

Namespace: `Assimalign.Cohesion.Database.Hosting`.

## Signature

```csharp
public sealed class DatabaseHealthObservation
{
    public string Name { get; }
    public DatabaseHealthStatus Status { get; }
    public string? Description { get; }
    public IReadOnlyDictionary<string, string> Data { get; }
}
```

## Behavior

No public constructor. DatabaseApplicationContext.GetHealthAsync produces snapshots. Name is
the stable escaped component name; Status is the observed availability. Description contains
exception types rather than messages. Data is immutable, formatted with invariant culture;
provisioning detail keeps the 4,096 UTF-16-unit bound. Accessors add no exceptions.

## Related reference

[Assembly](index.md) · [DatabaseApplicationContext](database-application-context.md) · [DatabaseHealthStatus](database-health-status.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseHealthObservation.cs`.
