# DatabaseApplicationContext

Concrete host context with transport-neutral health observations.

Namespace: `Assimalign.Cohesion.Database.Hosting`.

## Signature

Selected health observation members:

```csharp
public sealed class DatabaseApplicationContext : HostContext, IDatabaseApplicationContext
{
    public IReadOnlyList<string> HealthComponentNames { get; }
    public ValueTask<IReadOnlyList<DatabaseHealthObservation>> GetHealthAsync(
        CancellationToken cancellationToken = default);
    public ValueTask<DatabaseHealthObservation> GetHealthAsync(string componentName,
        CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

HealthComponentNames discovers live membership without evaluating health, awaiting work or
waiting for lifecycle operations. Re-read to follow opens/drops. All-component GetHealthAsync
evaluates each engine independently; the named overload evaluates only that engine, or only the
host gate for database.accepting. A missing component answers Unhealthy, "not open". Names compare
ordinally; [names, escaping, order and status precedence](design.md#health-observations-and-contributors)
are shared with HTTP and managed-resource contributors.

Health never opens, reopens or waits for a database. Stored databases never attempted by this
process are absent; failed stored opens remain Unhealthy until cleared. Cancellation of the
caller's token and OutOfMemoryException propagate. Other evaluation exceptions become Unhealthy
for the engine's components, with types only. Discovery tolerates a broken inventory by retaining
the engine name, except OutOfMemoryException. A null/empty/white-space componentName throws
ArgumentException. Separate checks observe separate snapshots.

## Related reference

[Assembly](index.md) · [DatabaseHealthObservation](database-health-observation.md) · [DatabaseHealthStatus](database-health-status.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseApplicationContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/docs/DESIGN.md`.
