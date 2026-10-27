# SqlDatabase

A SQL database with database-bound sessions, schema provisioning and physical table maintenance.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

The table-maintenance member is:

```csharp
public sealed partial class SqlDatabase : DatabaseInstance
{
    public ValueTask RewriteTableAsync(SqlObjectName schemaName, SqlObjectName tableName,
        CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

RewriteTableAsync resolves the current table from two decoded [SqlObjectName](sql-object-name.md)
components and runs maintenance on the engine's provisioning worker. It copies currently live
rows into a fresh physical owner with dense column ordinals, removes dropped-column ordinals,
and rebuilds every index. Publication atomically replaces the complete layout and index set.
Empty tables and tables without dropped columns also receive a fresh owner.

The operation preserves logical definitions, schema ownership, applied-schema hash, identity
allocation, constraints, defaults, frozen missing values, collation and dependent views.
Schema-owned tables are supported. This is a C# maintenance API; it adds no SQL statement or
wire command. SQL format 26 and compiled schema v8 remain unchanged.

Schema application, session DDL and maintenance share fail-fast admission. The operation acquires
an exclusive lock on the source table **before queueing**; a blocked rewrite does not occupy the
shared provisioning worker. The engine's nonzero LockTimeout bounds this acquisition. Zero means
an unlimited wait unless the caller cancels. Session timeout overrides do not configure this
database-level operation. A caller holding a conflicting transaction must release it or arrange
cancellation/a nonzero engine timeout.

### Snapshots and retry

Older read-only snapshots keep the original rows, indexes and physical layout until they end.
Retired storage is reclaimed afterwards. Writers that bound the old table before publication wait
for its lock and then abort with DatabaseTransactionAbortedException carrying CS04006. Roll back
and retry the whole transaction in a fresh snapshot. DML that reads a retired table as a source
also receives this refusal.

A snapshot begun before publication that later writes to a parent of the rewritten table gets
the retryable abort when its constraint work needs the retired child layout. Conversely, a stale
snapshot inserting/updating a child whose foreign-key probe needs a rewritten parent receives
CS04006 rather than a false FK violation. The probe uses its actual snapshot, including refreshed
ReadCommitted ON CONFLICT visibility. Empty input and MATCH SIMPLE NULL keys that need no parent
probe can still commit.

### Cancellation and recovery

Cancellation or failure before publication leaves the original table current. Interrupted
preparation is abandoned; retry it explicitly. Once durable publication succeeds, cancellation
cannot undo the replacement and mandatory cleanup finishes. Durable reclamation intents cover
prepared and retired owners. Recovery at copy, publication and cleanup boundaries exposes either
the complete original layout/index set or the complete compact replacement. Old snapshots retain
their owner until safe reclamation; abandoned owners are reclaimed by maintenance.

| Exception / diagnostic | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| ArgumentException | Default or invalid name component, before maintenance admission | — |
| ObjectDisposedException | Database is closed | — |
| OperationCanceledException | Caller cancellation before publication | — |
| DatabaseOfflineException | Database is offline | — |
| DatabaseObjectLockedException | Session DDL, schema application or maintenance owns admission | — |
| SqlCatalogException / CS01051 | Named table is absent | 42P01 |
| SqlCatalogException / CS04064 | Resolved table description changed before reservation; retry | 40001 |
| SqlTimeoutException / CS04044 | Exclusive source-table lock wait exceeded engine LockTimeout | 55P03 |
| DatabaseTransactionAbortedException / CS04006 | A concurrent writer needs a retired definition/layout; retry its whole transaction | — |

Storage and cleanup failures retain the normal SQL diagnostics and original causes. CS04006
describes affected concurrent DML, rather than successful completion of the maintenance call.

## Example

Adapted from SqlTableRewriteTests.Lifecycle's public decoded-name case:

```csharp
using System.Threading;
using Assimalign.Cohesion.Database.Sql;

await using var engine = SqlDatabaseEngine.Create("rewrite-public", new());
var database = await engine.CreateDatabaseAsync("db");
await using var session = await database.CreateSessionAsync();
await session.ExecuteAsync("CREATE TABLE \"a.b\" (gone INT, id INT PRIMARY KEY)");
await session.ExecuteAsync("INSERT INTO \"a.b\" VALUES (99, 1)");
await session.ExecuteAsync("ALTER TABLE \"a.b\" DROP COLUMN gone");

await database.RewriteTableAsync("DBO", "A.B", CancellationToken.None);
await using var rows = await session.ExecuteAsync("SELECT id FROM \"a.b\" WHERE id = 1");
// One row: id = 1. The dropped ordinal is gone and the primary-key index is rebuilt.
```

The API treats `A.B` as one decoded table name; SQL needs delimiters for that same name.
The lifecycle, snapshot, scheduling and crash-image tests cover publication, rollback/retry,
old indexed reads and reclamation without changing query results.

## Related reference

[Assembly](index.md) · [SqlObjectName](sql-object-name.md) · [SqlDatabaseSession](sql-database-session.md) · [Engine options](sql-database-engine-options.md) · [Design](design.md#table-rewrite)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabase.Rewrite.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Rewrite.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTableRewriteTests.cs` and its `.Lifecycle.cs`, `.Scheduling.cs`, `.Snapshots.cs` and `.References.cs` partials.
