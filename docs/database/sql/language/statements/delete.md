# DELETE (Cohesion SQL)

Removes rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
DELETE FROM [ schema_name . ] table_name [ [ AS ] table_alias ]
[ WHERE search_condition ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table from which rows are removed.
- **`search_condition`** — An optional predicate; omission targets all rows.

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

## USING sources: parsing and binding only

DELETE FROM ... USING accepts comma-separated sources and join trees for parsing and binding.
The target remains a stored table. Execution is deferred to the next batch and reports CK00001
before evaluating source or RETURNING callbacks or changing rows, including when the source
would be empty. The refusal leaves an explicit SQL transaction usable.

Bare RETURNING * binds the target columns first, then the visible source columns in namespace
order, following PostgreSQL. JOIN USING contributes merged visible keys rather than both
physical keys. [EXPLAIN](explain.md) describes the source plan without executing the mutation.

With tables t(id INT, v INT) and s(id INT, v INT), this statement is recognized and bound:

```sql
EXPLAIN DELETE FROM t USING s WHERE t.id = s.id RETURNING *;
```

EXPLAIN includes the source plan and a four-column RETURNING description (id, v, id, v).
Removing EXPLAIN reports CK00001 and changes no rows. Ordinary NULL predicate rules apply
during binding; SQL has no MISSING value. No source-DML execution behavior is promised here.

## Sources

- **Timeout and FK conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`, `SqlTimeoutRegressionTests.cs`, `SqlCascadeCatalogTests.cs`, `SqlForeignKeyBindingTests.cs` and `SqlReferenceLockTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Constraint diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Child actions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlReferentialActionTests.cs`.
- **Indexed targets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSeekTests.cs`.
