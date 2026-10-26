# UPDATE (Cohesion SQL)

Changes one or more column values on rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
[ WITH common_table_expression [ , ...n ] ]
UPDATE [ schema_name . ] table_name [ [ AS ] table_alias ]
SET column_name = { expression | DEFAULT } [ , ...n ]
[ FROM source [ , ...n ] ]
[ WHERE search_condition ]
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table being updated.
- **`column_name = expression`** — A destination column and its executable scalar value.
- **`search_condition`** — An optional predicate restricting the affected rows.

## Remarks

[WITH](../clauses/with.md) binds non-recursive query definitions and materializes reachable
definitions once under the statement snapshot, before any write. Targets remain stored tables.

Qualified references use the target name, or its alias when supplied. An explicit alias hides
the base name; hidden or unknown qualifiers report located `CS01015` before any write.

Each SET target may appear only once, using case-insensitive column identity. A repeated target,
including DEFAULT, reports `CS01019` during planning even if WHERE matches no rows. No row changes.

`c = DEFAULT` uses the same [default](../clauses/default.md) as an omitted INSERT column.
No default means NULL where allowed, otherwise `CS03024` (SQLSTATE 23502).
For [BY DEFAULT identity](../clauses/identity.md), SET id = DEFAULT allocates a new value.
ALWAYS rejects every assignment to its identity column, including DEFAULT (`CS03025`);
updates to other columns still execute.

Without FROM, omitting `WHERE` targets every row. The result reports an affected-row count. Values pass through
destination coercion and constraint enforcement. A `CAST` runs before destination storage coercion.

UPDATE uses the same [index eligibility rules as SELECT](select.md#index-access): direct key
equalities on a leading index prefix and optional supported range bounds, including BETWEEN,
can seek. Bounds must be non-NULL, plan-time evaluable and losslessly coercible; aliases and
qualified columns work. Unsupported terms remain residual predicates, and no usable bound means
a scan. Only WHERE TRUE selects a row; FALSE/UNKNOWN, including `id = NULL`, do not.
[EXPLAIN](explain.md) shows IndexSeek and bounds or TableScan below Update.

All targets are materialized through one statement snapshot before row/key locks and writes.
Changing an indexed key cannot revisit a row that moves within the sought range. A dropped or
replaced bound index falls back to a residual-filtered scan. Constraint checks and conflict behavior
remain unchanged; RETURNING order is unspecified.

Correlated and uncorrelated scalar, IN/NOT IN, EXISTS/NOT EXISTS and ANY/SOME/ALL [subqueries](../expressions/subqueries.md#update-and-delete-snapshots)
execute in WHERE over the pre-statement snapshot, including the target table and the session's
earlier uncommitted changes. Independent children materialize once; correlated children evaluate
for each candidate row before writes. Subqueries in SET and RETURNING report CK00001.
[RETURNING](../clauses/returning.md) projects the new values; SET expressions read original values.
Foreign keys and incoming NO ACTION/RESTRICT checks use the statement's final replacements, rather
than old rows that are being replaced. A self-reference may move with its key and mutually referenced
keys may swap; a final dangling reference fails atomically. See the
[final-row examples](../constraints.md#update-final-row-examples). ON UPDATE actions are not implemented.
Row updates remain available on schema-owned tables even though session DDL is restricted.

Constraint failures carry one Error diagnostic: `CS03037` (23505) for duplicate primary,
unique or unique-index keys, `CS03038` (23514) for CHECK/domain checks, `CS03039` (23503)
for a missing parent or a parent-key change that leaves references, and `CS03024` (23502)
for NOT NULL. Messages name the constraint and table without key values. Failure rolls back
the statement and leaves an explicit transaction usable. See [Diagnostics](../diagnostics.md).

## Size limits

UPDATE checks the final encoded row and all replacement index keys before applying any changes.
Exceeding [8,092 row bytes or 1,024 index-key bytes](../diagnostics.md#row-and-index-key-limits)
reports `CS05042` / SQLSTATE 54000, leaves the statement's rows unchanged, and keeps an
explicit transaction active. Existing values used for locking or tombstones receive no new-write
size check; updating an existing row to a fitting replacement remains supported.

For a table `large_rows (id INT PRIMARY KEY, value TEXT)` containing `(1, 'old')`, bind `value`
to `new string('x', 9000)` and execute:

```sql
UPDATE large_rows SET value = @value WHERE id = 1;
```

It fails with CS05042; the row remains `(1, 'old')`, as in SqlProgramLimitConformanceTests.

## Lock waits and time limits

Foreign-key preparation reuses one [statement reference graph](../constraints.md#foreign-key-preparation-and-concurrent-ddl)
and bound key projections rather than rediscovering references per row. Adjacent definition changes
during an object-lock wait rebuild before reads. A changed target reports CS04006 (no assigned
SQLSTATE); roll back an explicit transaction and retry in a fresh transaction.

[Statement timeout](../diagnostics.md#statement-and-lock-timeouts) reports CS05043 (57014);
an individual lock-wait timeout reports CS04044 (55P03). Failure restores the whole statement,
including child actions, releases its new resources and leaves earlier transaction work usable.
There is no cascade deletion-count ceiling; a configured execution deadline can bound its duration.


## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
UPDATE t SET age = age + 10 WHERE id = 1;
SELECT age FROM t ORDER BY id;
```

The update affects one row; the query returns `46`, `45`, and `41`.

`UPDATE t SET age = DEFAULT, AGE = 2 WHERE 1 = 0;` instead fails with CS01019;
the table remains unchanged even though the predicate matches no rows.

### Move keys within an indexed range

Execute separately in an empty database, as in SqlDmlSeekTests:

```sql
CREATE TABLE t (id INT PRIMARY KEY, v INT);
INSERT INTO t VALUES (1,0),(2,0),(3,0),(100,0);
UPDATE t SET id = id + 1, v = v + 1 WHERE id BETWEEN 1 AND 10 RETURNING id,v;
SELECT id,v FROM t ORDER BY id;
```

UPDATE seeks the primary-key index, affects three rows and returns `(2,1)`, `(3,1)`, `(4,1)`
in unspecified order. SELECT returns those rows in key order, then `(100,0)`. Each original target
is changed once, even though its replacement key still lies in the sought range.

## See also

[SET](../clauses/set.md) · [WHERE](../clauses/where.md) · [CAST](../expressions/cast.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## FROM sources

UPDATE ... FROM executes over stored tables, views, system relations, CTEs and aliased derived
queries, with the supported INNER, LEFT and CROSS joins and comma sources. The target remains
a stored table. SET destinations are target columns; SET values, WHERE and RETURNING may read
both target and source columns. An alias hides the original name; self-joins need distinct
visible names. A source item's JOIN ON sees that comma item's namespace, excluding the target
and other comma items; put target/source conditions in WHERE.

The engine collects every matching target/source association in one statement snapshot before
SET evaluation or writes. Only TRUE predicates match; FALSE and UNKNOWN do not. LEFT sources
can supply NULL-extended values; SQL has no MISSING value. Earlier uncommitted session writes
are visible. A reachable CTE materializes once, even when used more than once or containing
volatile expressions. Source values remain those collected for this statement.

Each physical target must have at most one matching source association. A second match raises
CS05068 (21000, cardinality_violation) before any SET expression or write, even if its values
are identical. This deliberately differs from PostgreSQL, which applies an arbitrary matching
source row. Equal unkeyed target rows remain distinct targets. This statement failure leaves
earlier explicit-transaction work and the connection usable.

SET reads original target values and collected source values; RETURNING reads new target values
and collected source values. Bare RETURNING * expands target columns, then each visible source
in written FROM order. JOIN USING contributes merged keys followed by remaining left/right
columns, omitting hidden duplicate keys. Duplicate output names are allowed; ordinal access
separates them and client name lookup returns the first. Output order is unspecified. Subqueries
in SET and RETURNING remain CK00001.

### Planning and concurrency

The association is a join with the target pinned as its outer input. Safe target-only WHERE
predicates can seek the target; a mandatory target/source equality can select a source index
probe or hash join. [EXPLAIN](explain.md#source-dml) shows the access paths and algorithms.
Hash-build memory counts against StatementMemoryBudget; exhaustion raises CS05067 (53200)
before writes, without spilling. Target and association collections are not covered by that
hash-build budget.

Equality-driven steps follow written FROM order. With t joined to b by equality and b joined
to a, write `FROM b, a`, rather than `FROM a, b`: the latter first visits the t × a product.
For example, `UPDATE t SET v = b.v FROM b, a WHERE t.id = b.tid AND b.aid = a.id`
puts the source connected to the target first. Both orders give the same result; general
join-order search is deferred to #1629. See [performance](../../performance.md#joined-mutations).

Before applying writes, the engine locks targets and checks their original physical location
and writer stamp. A concurrently committed target change raises the existing first-updater-wins
DatabaseTransactionAbortedException instead of following a replacement row, including after a
ReadCommitted row-lock wait. Collected source values do not refresh during that wait.

Joined DML holds intent-shared table locks on read sources, including tables read only by its
CTEs or subqueries, until the transaction ends. Failure/cancellation undoes the statement and
releases its new grants while preserving earlier transaction grants. Ordinary constraint checks,
final-row foreign-key validation, index maintenance and statement rollback still apply; a
RETURNING failure also undoes the statement. ON UPDATE actions remain unsupported.

### Example

Execute separately in a new database:

```sql
CREATE TABLE t (id INT PRIMARY KEY, v INT);
CREATE TABLE s (id INT, v INT);
INSERT INTO t VALUES (1,0), (2,0);
INSERT INTO s VALUES (1,10), (2,20);
UPDATE t SET v = s.v FROM s WHERE t.id = s.id RETURNING *;
```

The update affects two rows and returns (1,10,1,10) and (2,20,2,20) in unspecified order, with
columns id, v, id, v. After inserting a second source for id 2:

```sql
INSERT INTO s VALUES (2,20);
UPDATE t SET v = s.v + 1 FROM s WHERE t.id = s.id;
SELECT id, v FROM t ORDER BY id;
```

UPDATE reports CS05068. SELECT still returns (1,10) and (2,20); no target was changed.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSourceReviewTests.cs`.

- **Timeout and FK conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`, `SqlTimeoutRegressionTests.cs`, `SqlCascadeCatalogTests.cs`, `SqlForeignKeyBindingTests.cs` and `SqlReferenceLockTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Update.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Constraint diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- **Indexed targets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDmlSeekTests.cs`.
