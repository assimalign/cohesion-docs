# DatabaseHealthStatus

Observed availability of a Database component.

Namespace: `Assimalign.Cohesion.Database.Hosting`.

## Signature

```csharp
public enum DatabaseHealthStatus { Unhealthy = 0, Degraded = 1, Healthy = 2 }
```

## Behavior

Unhealthy means unavailable or readiness not established. Degraded means available with an
outstanding condition. Healthy means no observed health condition. Aggregation takes the
minimum status. Reading values adds no exceptions.

## Related reference

[Assembly](index.md) · [DatabaseHealthObservation](database-health-observation.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseHealthStatus.cs`.
