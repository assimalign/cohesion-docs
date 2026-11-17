# DatabaseEngine

The model-independent base that manages logical databases and composes their journal admission.

Namespace: `Assimalign.Cohesion.Database`.

## Signature

Selected inventory and protected composition members:

```csharp
public abstract class DatabaseEngine : IAsyncDisposable, IDisposable
{
    public IReadOnlyList<DatabaseInstance> OpenDatabases { get; }
    public IReadOnlyList<DatabaseOpenFailure> FailedOpens { get; }
    protected abstract IReadOnlyList<DatabaseInstance> GetOpenDatabasesCore();
    protected void ConfigureJournalAdmission(DatabaseInstance database, long journalSizeLimit,
        params Assimalign.Cohesion.Database.Storage.Storage[] storages);
}
```

## OpenDatabases and custom engines

OpenDatabases is public, non-virtual and synchronous. It copies the protected core's published
instance snapshot into a new read-only list, filters closing instances, retains offline instances
and returns empty after disposal. Reading performs no I/O, takes no engine lock and never opens,
reopens or waits. GetDatabasesAsync keeps each model's enumeration semantics; it can open stored
Documents, Graph and Blob databases through recovery.

Every shipped engine implements GetOpenDatabasesCore(). **A custom engine deriving from
DatabaseEngine must now implement it.** Return the current published snapshot without locks,
I/O or lifecycle work; the base filters and copies it.

The synchronous part of every create, open and drop core runs under one engine-wide reentrant
completion lock; lifecycle cores run one at a time per engine. A core must not block on another
thread that calls back into the engine's lifecycle methods. Deferred completion continuations
yield before taking the lock, so none runs inline under a custom engine's lock.

## FailedOpens

Lock-free, non-virtual read of an immutable [DatabaseOpenFailure](database-open-failure.md)
snapshot, empty after disposal. Failed stored opens retain safe metadata, including manual opens
with automatic reopen disabled. Successful open/create/drop or DatabaseNotFoundException clears
the name; disposal clears all records. A stored database never attempted by this process is absent.
The reopen service does not retry these records.

Name-validation ArgumentException with ParamName "name" (except ArgumentOutOfRangeException)
leaves existing records unchanged. An open that fails because the engine is disposed, or while
the caller's token is cancelled, records nothing; disposal itself clears every record.
Internal argument failures and another object's ObjectDisposedException are recorded. Structured
carriers/coded causes supply the code; CK06003 has precedence over additional model diagnostics.
Lifecycle completion ordering prevents an older deferred failure from replacing a later success.

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
