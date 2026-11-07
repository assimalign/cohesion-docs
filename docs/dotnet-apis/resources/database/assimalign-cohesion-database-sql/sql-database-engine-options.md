# SqlDatabaseEngineOptions

Configures SQL engine execution budgets and storage/worker settings.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

Execution-budget members include:

```csharp
public sealed class SqlDatabaseEngineOptions
{
    public TimeSpan StatementTimeout { get; set; }
    public TimeSpan LockTimeout { get; set; }
    public long StatementMemoryBudget { get; set; }
    public FileSystemPath? TempDirectory { get; set; }
    public long? TempSpaceQuota { get; set; }
    public long? StatementTempSpaceQuota { get; set; }
}
```

## Behavior and exceptions

StatementTimeout and LockTimeout default to TimeSpan.Zero, disabling their timers. StatementTimeout
bounds cooperative execution after parsing; LockTimeout bounds each contended lock wait separately,
including unique-index waits.
SqlDatabaseEngine.Create snapshots options and throws ArgumentOutOfRangeException for either value
below zero or above TimeSpan.FromMilliseconds(int.MaxValue). Setters store the values; later changes
to the caller's options do not change the engine snapshot.

[Session overrides](sql-database-session.md) inherit when null and disable when zero. Started
commits and mandatory post-self-commit cleanup complete without these timers. SQL transaction
control arms neither timer; schema-provisioning sessions explicitly disable both. Other options
are described in [design](design.md#engine-owned-background-workers).

LockTimeout also bounds the source-table exclusive lock for
[SqlDatabase.RewriteTableAsync](sql-database.md), before work is queued. Zero leaves this wait
unlimited unless canceled; the database-level maintenance call uses the engine setting, rather
than a session override. Expiration throws SqlTimeoutException with CS04044 / 55P03.

For a [joint migration](design.md#joint-migrations), including every deferred migration, nonzero engine LockTimeout
also bounds the touched-table reservation before queueing. A reservation timeout reports
`CS03402`, preserving the lock-manager cause. Imperative cancellation/timeout clears Provisioning;
deferred failures stay sticky.
Untouched tables remain usable; definition DDL is refused database-wide while queued/running.

Journal admission pauses new brackets at `ceil(JournalSizeLimit/2)`, even with the size trigger
off. A SQL deadline during this wait reports CK04014 / 57014. With the default StatementTimeout
of zero, there is no statement deadline. Session DDL retaining metadata refuses immediately;
migrations wait outside physical/publication gates and resume prepared work. See the
[shared policy](../assimalign-cohesion-database/design.md#journal-admission).

## Example

Adapted from SqlTimeoutConformanceTests; engine defaults are copied before the caller changes them:

```csharp
var options = new SqlDatabaseEngineOptions { LockTimeout = TimeSpan.FromMilliseconds(80) };
await using var engine = SqlDatabaseEngine.Create("lock-timeout", options);
options.LockTimeout = TimeSpan.Zero;
var database = await engine.CreateDatabaseAsync("db");
await using var session = await database.CreateSessionAsync();
Console.WriteLine(session.LockTimeout is null); // True: inherits the engine's 80 ms
session.LockTimeout = TimeSpan.Zero; // disable for this session
session.LockTimeout = null;          // inherit the engine's 80 ms again
```

## Related reference

[Assembly](index.md) · [Session](sql-database-session.md) · [Timeout exception](sql-timeout-exception.md)

## Statement memory

StatementMemoryBudget limits accounted bytes retained by query buffers and writes in one
statement. It defaults to 67,108,864 bytes (64 MiB), must be positive and is copied into the
engine snapshot. Engine creation rejects nonpositive values with ArgumentOutOfRangeException.

Plain SELECT charges nested-loop and normalized join buffers, staged matches, group keys and
aggregate state, sorting and top-N entries, output rows, DISTINCT state, relation and subquery
caches, and set operands and state. Every buffer reserves before retaining a row and releases on
consumption or scope exit. Consumers re-admit materialized results after child execution releases
its charge. Costs follow retained rows, never collection capacity.

A cell of at least 256 bytes charges its payload once per buffer owner by reference identity,
plus a fixed 64-byte entry per distinct cell. Further references pay only array slots; smaller
cells pay per reference. Two live buffers each charge their wide identities. Hash builds still
charge each reference. Ordered sequence deferral retains its source buffers; unordered sequence
projection streams without retaining inputs. Kept outputs and group state still charge.

Joined DML charges retained inputs, associations and selected writes. Grouping sets charge group
and aggregate state. CTAS sources charge like any SELECT; CTAS itself admits only output rows
kept after pagination and its batch buffers. CTAS publishes only after all copying succeeds.
INSERT SELECT and DML subquery sources keep their existing policy.

Window buffers outside the window ledger also charge; ledger-owned rows and subquery vectors
keep that owner. Independent caches created in a window remain charged for the statement. When
plain bytes fill the grant first, refusal reports CS05067, including where the window or JSON
ledger would later refuse with CS05079 or CS05094. With no plain bytes live, those grants retain
their specific diagnostic. Reservations unwind on success, error and cancellation.

Eligible root ORDER BY and ordered DISTINCT, including CTAS source roots, spill at CS05067.
Other shapes and retained results still need a sufficient grant; an unordered CTAS refuses
where the same source with an eligible ORDER BY spills. A session allows eight live spilled
results. Dispose them after enumeration or cancellation.
See [query performance](../../../../database/sql/performance.md#statement-memory).

Every [referential closure](../../../../database/sql/language/constraints.md#referential-closure-memory), including plain ON DELETE CASCADE, charges
StatementMemoryBudget. About 840 bytes per two-column row limits a default 64 MiB closure to
about 79,900 rows, less when other statement work is retained. There is no cascade depth limit.
CS05067 (53200) refuses exhaustion before writes; cleanup releases abandoned attempts.

[Windows](../../../../database/sql/language/clauses/window.md#memory) share the grant for retained
rows, keys, sort/peer indexes, argument caches, transition state, outputs and ownership bookkeeping.
Aliased payloads are charged once; partition scratch releases at its boundary. Exhaustion uses
CS05079 (54000) with no plain SELECT bytes live; when plain bytes fill the grant first it uses
CS05067 (53200). There is no spill.
The default-grant fixture admits 138,961 two-window rows or 162,448 moving-AVG rows; widths and
other retained state change these limits.

## SQL/JSON memory and work

[SQL/JSON path functions](../../../../database/sql/language/functions/json-path.md#limits) also
charge transient DOM, result references and selected-result serialization to StatementMemoryBudget.
Exhaustion reports CS05094 (54000), outside ON ERROR handling, or CS05067 when plain SELECT
bytes fill the grant first. Their 1,000,000-visit work cap is
shared with window-frame recomputation and is independent of the configured byte grant.

## Temporary space

These three options configure the disposable-storage substrate. They exist only on the SQL
engine options on this branch. Eligible root ORDER BY and ordered DISTINCT, including CTAS
source roots, use it when their memory reservation would refuse. Excluded shapes retain
memory refusals; see [eligibility and capacity](../../../../database/sql/performance.md#root-sort-spilling).

| Member | Default and validation |
| --- | --- |
| TempDirectory | Null: file engines use RootPath/.cohesion-temp; in-memory engines have no temp space unless set. An override for a file engine must be a descendant of RootPath with no link components and a first segment no database could take; a leading dot is safe. Empty/default paths are refused. |
| TempSpaceQuota | Null: min(10 GiB, floor(available volume bytes / 4)), fixed after startup sweeping. An unavailable probe uses zero until the first run retries successfully. Explicit values must be nonnegative; zero refuses growth. |
| StatementTempSpaceQuota | Null: four times StatementMemoryBudget, saturating at Int64.MaxValue (256 MiB at defaults). Explicit values must be nonnegative; zero refuses growth. |

Options are snapshotted with the engine. Invalid paths or negative quotas throw
ArgumentOutOfRangeException naming the engine and option. A first path segment that is itself
a valid database name, including a run-shaped name, refuses configuration before directory
creation. Creating database `.cohesion-temp` instead fails the database-name guard with
ArgumentException. An empty RootPath still selects in-memory storage; a default FileSystemPath
value is refused.

One directory belongs to one spilling engine. Choose a different TempDirectory for each engine.
A lease/setup failure warns and keeps databases available; a later run retries and uses CK05005
if still unavailable. Quotas count physical 8 KiB pages, including framing, unwritten tails and
simultaneous merge input/output files. Disposal releases reservations.

The free-space floor always applies. For N = max(1, open databases), let C = 1 MiB +
N × (BufferPoolCapacity + 128 × 8192 + 2 × (JournalSizeLimit + reclamation bracket bytes for
32 rows)). The floor is C + max(1 MiB, ceil(C / 4)), saturating on overflow; default file options
reserve about 2.54 GiB per open database. An in-memory engine reserves only 1 MiB on this volume.
This is admission headroom, not an OS disk reservation.

Unix creates directories 0700 and run/lease files 0600, and refuses group/other-writable
directories. Windows explicit directories keep inherited ACLs. Temp data is unencrypted;
Unix kills and power loss on any OS can leave runs until a later sweep. Windows process exit
deletes its runs. OS backups of RootPath can capture live temp files. See
[temporary storage](../assimalign-cohesion-database-storage/design.md#disposable-query-storage).

## Recursive query memory

[WITH RECURSIVE](../../../../database/sql/language/clauses/with.md#on-demand-evaluation-and-memory) reclaims single-pass consumed rows. Rescans and DISTINCT retain
history or membership; selected results still materialize before delivery. The default 64 MiB grant
holds roughly 500,000 single-BIGINT UNION ALL counter results by executor accounting; payloads and
other retained state change this capacity. COUNT(*) over the tested million-row counter instead
retains less than 32 KiB. Bounded-state infinite consumers require cancellation; retaining consumers
exhaust the grant with CS05067 (53200). There is no fixed row/depth limit or spill.

## Sources

- **Recursive accounting and reclamation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlRecursiveReclamationTests.cs`; row-returning capacity follows `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.RecursiveTables.cs` and `SqlWindowMemory.cs` in the same directory.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
