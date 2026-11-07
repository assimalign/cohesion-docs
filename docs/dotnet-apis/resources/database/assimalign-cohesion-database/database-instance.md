# DatabaseInstance

The model-independent base of a logical database.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

Selected admission member:

```csharp
public abstract class DatabaseInstance : IAsyncDisposable, IDisposable
{
    public bool IsJournalAdmissionPaused { get; }
}
```

## IsJournalAdmissionPaused

True when any independently checkpointed storage of this database has paused new write brackets.
This folds pure live-state reads; it does not observe a threshold or request a checkpoint. A
pending pause leaves the database online, with Degraded/CK06006 health. Reads retain ordinary
snapshot and locking rules; persistent worker failures retain the offline window.

## Related reference

[Assembly](index.md) · [DatabaseEngine](database-engine.md) · [Admission policy](design.md#journal-admission)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseInstance.cs`.
