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
}
```

## Behavior and exceptions

Both default to TimeSpan.Zero, disabling the timer. StatementTimeout bounds cooperative execution
after parsing; LockTimeout bounds each contended lock wait separately, including unique-index waits.
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

For a [joint typed TYPE migration](design.md#joint-type-migrations), nonzero engine LockTimeout
also bounds the touched-table reservation before queueing. A reservation timeout reports
`CS03402` in both the thrown diagnostic and Provisioning, preserving the lock-manager cause.
Untouched tables remain usable; definition DDL is refused database-wide while queued/running.

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

StatementMemoryBudget is the maximum accounted bytes retained by query buffers, write sets and
CREATE TABLE AS copying in one statement. It defaults to 67,108,864 bytes (64 MiB) and must be
positive; engine creation rejects a nonpositive value with ArgumentOutOfRangeException. The engine
snapshots the value.

SELECT charges retained nested-loop inners, joined matches, GROUP BY and aggregate DISTINCT state,
sorts and bounded top-N heaps, DISTINCT sets, materialized output construction, WITH and derived
relation caches, subquery value caches and set-operation state. Aggregate transitions consume
scan/join input as a stream; eligible joins stop with LIMIT. Unordered DISTINCT with fixed collations
streams input and can stop after OFFSET + LIMIT distinct rows. EXISTS stops after one qualifying
row and scalar subqueries after two, preserving OFFSET and tighter explicit limits; a second
scalar row retains the cardinality error.

Joined DML charges retained inputs, associations and selected writes. INSERT SELECT readmits its
kept source and charges copied values and encoded writes through insertion. CREATE TABLE AS
charges retained query output and batch buffers; discarded OFFSET outputs add no CTAS source
charge. Ordered sequence deferral shares ordinary sorting charges; unordered sequence projection
streams without retaining inputs. Recursive queries also charge retained results and scratch.
Planning and transient scalar evaluation are excluded. Accounted bytes describe retained storage,
not total GC allocations. Aliased payloads share a charge for their required lifetime.

Exceeding the budget reports CS05067 / SQLSTATE 53200 before writes or publication. Reservations
release on success, failure, cancellation and early LIMIT. Large sorts and results can therefore
refuse even without a hash join. No spill is implemented. See [query performance](../../../../database/sql/performance.md).

Every [referential closure](../../../../database/sql/language/constraints.md#referential-closure-memory), including plain ON DELETE CASCADE, charges
StatementMemoryBudget. About 840 bytes per two-column row limits a default 64 MiB closure to
about 79,900 rows, less when other statement work is retained. There is no cascade depth limit.
CS05067 (53200) refuses exhaustion before writes; cleanup releases abandoned attempts.

[Windows](../../../../database/sql/language/clauses/window.md#memory) share the grant for retained
rows, keys, sort/peer indexes, argument caches, transition state, outputs and ownership bookkeeping.
Aliased payloads are charged once; partition scratch releases at its boundary. Exhaustion uses
CS05079 (54000), or CS05067 (53200) when admitting earlier CTE/subquery caches. There is no spill.
The default-grant fixture admits 138,961 two-window rows or 162,448 moving-AVG rows; widths and
other retained state change these limits.

## SQL/JSON memory and work

[SQL/JSON path functions](../../../../database/sql/language/functions/json-path.md#limits) also
charge transient DOM, result references and selected-result serialization to StatementMemoryBudget.
Exhaustion reports CS05094 (54000), outside ON ERROR handling. Their 1,000,000-visit work cap is
shared with window-frame recomputation and is independent of the configured byte grant.

## Recursive query memory

[WITH RECURSIVE](../../../../database/sql/language/clauses/with.md#on-demand-evaluation-and-memory) reclaims single-pass consumed rows. Rescans and DISTINCT retain
history or membership; selected results still materialize before delivery. The default 64 MiB grant
holds roughly 500,000 single-BIGINT UNION ALL counter results by executor accounting; payloads and
other retained state change this capacity. COUNT(*) over the tested million-row counter instead
retains less than 32 KiB. Bounded-state infinite consumers require cancellation; retaining consumers
exhaust the grant with CS05067 (53200). There is no fixed row/depth limit or spill.

## Sources

- **SELECT retention and early stop** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSelectBufferBudgetTests.cs`, `SqlSelectLifetimeRegressionTests.cs` and `SqlSelectSecondFixTests.cs` in the same test directory.

- **Recursive accounting and reclamation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlRecursiveReclamationTests.cs`; row-returning capacity follows `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.RecursiveTables.cs` and `SqlWindowMemory.cs` in the same directory.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/SqlDatabaseEngine.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
