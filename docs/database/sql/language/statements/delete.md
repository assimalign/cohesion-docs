# DELETE (Cohesion SQL)

Removes rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
DELETE FROM [ schema_name . ] table_name [ [ AS ] table_alias ]
[ USING source [ , ...n ] ]
[ WHERE search_condition ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table from which rows are removed.
- **`search_condition`** — An optional predicate; without USING, omission targets all rows.

## Remarks

Existing row values used for lock hashes and tombstones receive no new-write size check, so
size limits do not prevent deletion. Referential SET NULL/SET DEFAULT actions still validate
their final child replacements: `CS05042` / SQLSTATE 54000 leaves parents and children unchanged
and an explicit transaction active. See [size limits](../diagnostics.md#row-and-index-key-limits).

[WITH](../clauses/with.md) binds non-recursive query definitions and materializes reachable
definitions once under the statement snapshot, before any write. Targets remain stored tables.

Qualified references use the target name, or its alias when supplied. An explicit alias hides
the base name; hidden or unknown qualifiers report located `CS01015` before any write.

Only rows for which `WHERE` is `TRUE` are selected. The statement reports an affected count.
Foreign keys default to `ON DELETE NO ACTION`; a declared `ON DELETE CASCADE` removes dependent
rows. Nullable foreign-key values are allowed.

DELETE shares [SELECT's index eligibility rules](select.md#index-access): leading key equalities
and supported range bounds (`<`, `<=`, `>`, `>=`, BETWEEN) can seek when comparands are non-NULL,
plan-time evaluable and losslessly coercible. Aliases and qualified columns work. Unsupported terms
remain residual predicates; without a usable bound, DELETE scans. NULL comparisons yield UNKNOWN
and select no row; IS NULL can filter rows but supplies no seek bound.

[EXPLAIN](explain.md) shows IndexSeek and bounds or TableScan below Delete. All targets, including
their WHERE subquery checks, materialize in one statement snapshot before any deletion. A dropped
or replaced bound index falls back to a residual-filtered scan. RETURNING order is unspecified.

Correlated and uncorrelated scalar, IN/NOT IN and EXISTS/NOT EXISTS [subqueries](../expressions/subqueries.md#update-and-delete-snapshots)
execute in WHERE. Independent children materialize once; correlated children evaluate for each
candidate row before any deletion. Both use pre-statement values, including self-reads; the session's earlier uncommitted changes are visible. Rollback restores rows and
cascades together. [RETURNING](../clauses/returning.md) projects old values of directly targeted
rows and excludes cascade/child-action rows. Deleting rows does not remove the
table definition; [DROP TABLE](drop-table.md) is the separate schema operation.

[SET NULL and SET DEFAULT](../constraints.md) update surviving children through cascade chains.
Defaults use the delete transaction clock; child validation and index changes are atomic with deletion.

A NO ACTION/RESTRICT refusal reports `CS03039` (23503), naming the foreign key and child
table without key values. Child SET actions can instead fail with `CS03037` (23505) for
uniqueness, `CS03038` (23514) for CHECK/domain checks, or `CS03024` (23502) for NOT NULL;
a missing replacement parent also uses CS03039. Each constraint exception carries one Error
diagnostic. The statement rolls back parent and child changes together, leaving an explicit
transaction usable. See [Diagnostics](../diagnostics.md).

## Lock waits and time limits

Foreign-key preparation reuses one [statement reference graph](../constraints.md#foreign-key-preparation-and-concurrent-ddl)
and bound key projections rather than rediscovering references per row. Adjacent definition changes
during an object-lock wait rebuild before reads. A changed target reports CS04006 (no assigned
SQLSTATE); roll back an explicit transaction and retry in a fresh transaction.

Cascade preparation releases new attempt grants on the first two refusals, waits and rebuilds,
retaining every completed wait grant. From the third refusal onward it retains the closure grants
and queues normally. This bounds release-and-retry so hot child inserts cannot starve the parent
delete. Genuine hard deadlocks still report `CS04065` / SQLSTATE `40P01`. The failed statement
preserves earlier explicit-transaction work and locks; the caller must roll back the victim
transaction before retrying it. See [cascade backoff](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/design.md#cascade-backoff-and-queue-repair).

[Statement timeout](../diagnostics.md#statement-and-lock-timeouts) reports CS05043 (57014);
an individual lock-wait timeout reports CS04044 (55P03). Failure restores the whole statement,
including child actions, releases its new resources and leaves earlier transaction work usable.
There is no cascade deletion-count ceiling; a configured execution deadline can bound its duration.


## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
DELETE FROM t WHERE id = 2;
SELECT id FROM t ORDER BY id;
```

The delete affects one row; the query returns `1` and `3`.

### Seek a composite-index prefix

Execute separately in an empty database, as in SqlDmlSeekTests:

```sql
CREATE TABLE t (id INT PRIMARY KEY, bucket INT, v INT);
CREATE INDEX ix_bucket_id ON t(bucket,id);
INSERT INTO t VALUES (1,1,10),(2,2,20),(3,2,30),(4,3,40);
DELETE FROM t WHERE bucket = 2 RETURNING id,v;
SELECT id FROM t ORDER BY id;
```

DELETE seeks ix_bucket_id using its leading bucket key, affects two rows and returns `(2,20)`
and `(3,30)` in unspecified order. SELECT then returns `1`, `4`.

## See also

[WHERE](../clauses/where.md) · [Constraints](../constraints.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## USING sources

DELETE ... USING executes with comma sources and supported join trees over stored tables,
views, system relations, CTEs and aliased derived queries. The target remains a stored table.
WHERE and RETURNING bind target and source columns. Aliases hide base names; self-joins need
distinct names. A source JOIN ON excludes the target and other comma items; target/source
conditions belong in WHERE.

The engine collects target/source matches under one statement snapshot before any deletion.
Only TRUE selects an association; NULL comparisons yield UNKNOWN, and LEFT sources can supply
NULL-extended columns. SQL has no MISSING value. Earlier uncommitted session changes are visible;
reachable CTEs materialize once. Each physical target is deleted once even if several sources
match. Equal unkeyed target rows remain distinct. Unlike UPDATE FROM, multiple matches do not
raise CS05068.

RETURNING reads old target values and one collected source association per target. Which source
is retained, and result order, are unspecified. Bare RETURNING * expands target columns then
visible sources in written USING order, retaining JOIN USING's merged-column ordering. Duplicate
output names are allowed. Cascades and child SET actions contribute no extra RETURNING rows or
direct affected count. Subqueries in RETURNING remain CK00001.

The target stays outer in the association join. Safe target-only predicates can seek it;
mandatory target/source equalities can drive a source index probe or budgeted hash join.
Equality-driven steps follow written USING order: list the source connected by equality to the
target first (USING b, a when only b connects to the target). General join-order search is
later work (#1629). [EXPLAIN](explain.md#source-dml) describes these paths without deleting rows.
Hash builds use StatementMemoryBudget (CS05067 / 53200 on exhaustion); target/association
collections are outside that hash-build budget.

A concurrently committed change to a collected target raises the existing first-updater-wins
DatabaseTransactionAbortedException, rather than following a replacement row, including after a
ReadCommitted row-lock wait. Source values remain from collection. Joined DML retains intent-shared
table locks on read sources, including tables read only by CTEs or subqueries, until transaction
end. Cascade retries reuse collected associations and reacquire source intents. Failure releases
new statement grants after undo and preserves earlier transaction grants.

Ordinary NO ACTION/RESTRICT, CASCADE, SET NULL and SET DEFAULT actions and their constraint checks
apply atomically. Constraint or RETURNING failures restore targets, child actions and indexes and
leave earlier explicit-transaction work usable. See [UPDATE FROM](update.md#from-sources) for
association planning and [constraints](../constraints.md) for child-action diagnostics.

### Example

Execute separately in a new database:

```sql
CREATE TABLE t (id INT, v INT);
CREATE TABLE s (id INT, v INT);
INSERT INTO t VALUES (1,0), (1,0);
INSERT INTO s VALUES (1,7), (1,7);
DELETE FROM t USING s WHERE t.id = s.id RETURNING t.v, s.v;
SELECT COUNT(*) FROM t;
```

DELETE affects two physical targets and returns two (0,7) rows. COUNT returns 0. The four
matching associations do not produce four deletions.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceReviewTests.cs`.

- **Timeout and FK conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`, `SqlTimeoutRegressionTests.cs`, `SqlCascadeCatalogTests.cs`, `SqlForeignKeyBindingTests.cs` and `SqlReferenceLockTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Constraint diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Child actions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`.
- **Indexed targets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSeekTests.cs`.
