# TRUNCATE (Cohesion SQL)

Empties a set of stored tables atomically while preserving their definitions and indexes.

> **Status:** Implemented for automatic statements; explicit transactions remain excluded.

## Syntax

```syntaxsql
TRUNCATE [ TABLE ] [ ONLY ] [ schema_name . ] table_name
    [ , [ ONLY ] [ schema_name . ] table_name ...n ]
    [ CONTINUE IDENTITY | RESTART IDENTITY ]
    [ CASCADE | RESTRICT ] [ ; ]
```

## Arguments

- **`table_name`** — A stored table in the session database; the default namespace is dbo.
- **`ONLY`** — Retained per target in parsing/rendering. There is no table inheritance.
- **`CONTINUE IDENTITY`** — The default. Preserves each identity allocator's exact in-process next
  value and unused reservation block. Reopen still skips reserved values under the identity contract.
- **`RESTART IDENTITY`** — Resets every sequence owned by tables in the resulting set to its configured
  START, retaining INCREMENT, including descending definitions.
- **`RESTRICT`** — The default. Requires every referencing table to be explicitly listed.
- **`CASCADE`** — Recursively includes tables referencing any table already in the set.

## Remarks

Repeated table references are ignored. The complete set is validated and published together;
success returns affected count **0**, regardless of the number of removed rows. There is no WHERE
or RETURNING clause. Views and system relations are read-only targets. Schema-owned stored tables
can be truncated because this changes their data, preserving ownership and definitions.

Columns, defaults, CHECKs, foreign keys, collation and indexes remain defined. TRUNCATE replaces
storage rather than scanning/deleting rows; it does not run row-level ON DELETE actions.

RESTART IDENTITY includes identity-owned and ordinary OWNED BY sequences. Restart endpoints and
replacement ownership publish atomically; CONTINUE preserves the existing sequence cursor.

### Foreign-key closure

RESTRICT refuses with CS03067 unless the written set includes every incoming foreign-key
reference, recursively, even if the referencing table is empty. Self-references do not require
another target. CASCADE computes that closure recursively, including cycles and references from
other SQL namespaces, independently of the foreign key's ON DELETE action.

The engine locks the set and checks the closure again at reservation and publication. A change to
the required table set fails with retryable CS04064; retry the complete statement. A cascaded
table disappearing during lock acquisition also uses CS04064. An explicitly named missing table
retains CS01051. No partial target set is published.

### Transactions and snapshots

TRUNCATE is self-committing. Inside an explicit transaction it is refused with CS04066 (25001),
leaving the transaction active and all earlier work usable. It neither commits nor discards that
work. This differs from PostgreSQL's transactional TRUNCATE.

A reader whose snapshot predates publication keeps seeing its original rows after TRUNCATE commits,
including index seeks, direct joins, views, joining views and joins with untouched tables. Fresh
snapshots see the empty replacements, or subsequent inserts into them. ReadCommitted pins a
statement horizon; a later statement refreshes it. Old row/index storage is reclaimed only after
all older transaction and statement pins end.

DML targeting retired tables or joining them as sources is refused before writes with retryable
CS04006, including INSERT SELECT and joins inside DML subqueries/views/WITH bodies. End that
transaction and retry against a fresh snapshot. Read-only statements retain the old view.

[CREATE TABLE AS](create-table.md#create-table-as) uses the same durable prepared-owner
reclamation machinery to publish a populated new table atomically. CTAS uses a single builder
commit for row visibility; TRUNCATE preserves the old table storage for older snapshots. Both
reuse SQL format 30. CTAS inside an explicit transaction uses CS04003 (25001); TRUNCATE retains
CS04066 (25001).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00003 | Missing table/policy operand, repeated or reordered policies, alias, WHERE or other trailing syntax | — |
| CS03067 | RESTRICT omits a required referencing table, even an empty one | 0A000 (`feature_not_supported`) |
| CS04066 | TRUNCATE inside an explicit transaction; prior work and active state are kept | 25001 (`active_sql_transaction`) |
| CS04064 | Concurrent closure change or replacement while waiting; retry the statement | 40001 (`serialization_failure`) |
| CS04006 | Retired table definition encountered by DML; retry in a fresh transaction | — |
| CS01051 | Explicitly named table is missing | 42P01 (`undefined_table`) |
| CS04021 | Target is a view or system relation | 25006 (`read_only_sql_transaction`) |

Other lock, timeout and storage failures retain their [normal diagnostics](../diagnostics.md).

## Examples

### Identity and foreign keys

Adapted from SqlTruncateWireTests; execute separately in a fresh database:

```sql
CREATE TABLE p (id BIGINT GENERATED BY DEFAULT AS IDENTITY (START 10 INCREMENT 3) PRIMARY KEY);
CREATE TABLE c (id INT PRIMARY KEY, pid BIGINT REFERENCES p(id));
INSERT INTO p DEFAULT VALUES;
INSERT INTO c VALUES (1,10);
TRUNCATE p RESTRICT;
```

TRUNCATE fails with CS03067 and both tables retain their rows. Continue:

```sql
TRUNCATE TABLE ONLY p, c CONTINUE IDENTITY RESTRICT;
INSERT INTO p DEFAULT VALUES;
SELECT id FROM p;
TRUNCATE p RESTART IDENTITY CASCADE;
INSERT INTO p DEFAULT VALUES;
SELECT id FROM p;
```

Each successful TRUNCATE reports affected count 0. The first SELECT returns 13; the second returns
10. The child table is empty, and its foreign key remains defined.

### Refusal keeps earlier work

With a separate stored `t (id INT)` table, execute each statement on the same session:

```sql
BEGIN;
INSERT INTO t VALUES (1);
TRUNCATE t;
SELECT * FROM t;
ROLLBACK;
```

TRUNCATE reports CS04066. SELECT still returns the inserted 1; ROLLBACK then discards it.
The [session API example](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-session.md#truncate-and-old-snapshots)
shows two sessions retaining an older indexed read across TRUNCATE and then reading the current view.

## See also

[Statements](index.md) · [DELETE](delete.md) · [IDENTITY](../clauses/identity.md) · [Constraints](../constraints.md) · [BEGIN](begin-transaction.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Truncate.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.Truncate.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTruncateTests.cs` and `SqlTruncateReviewTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTruncateReviewTests.Closures.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlTruncateWireTests.cs`.
