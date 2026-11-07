# DatabaseEngine

The model-independent base that manages logical databases and composes their journal admission.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

Selected protected composition member:

```csharp
public abstract class DatabaseEngine : IAsyncDisposable, IDisposable
{
    protected void ConfigureJournalAdmission(DatabaseInstance database, long journalSizeLimit,
        params Assimalign.Cohesion.Database.Storage.Storage[] storages);
}
```

## ConfigureJournalAdmission

Model engines call this after open-time provisioning and before publishing the database. It arms
every independently checkpointed data/catalog storage with the resolved byte limit and attaches
its live state to [DatabaseInstance](database-instance.md). Zero disables the backstop. Shutdown
disables pausing before server/worker joins and database cleanup.

Null database, storage array or array entry throws ArgumentNullException; a negative limit throws
ArgumentOutOfRangeException. Configuring the same database twice throws InvalidOperationException.
See [threshold, failure and read behavior](design.md#journal-admission).

## Related reference

[Assembly](index.md) · [Interlock](../assimalign-cohesion-database-transactions/transaction-checkpoint-interlock.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseEngine.cs`.
