# CREATE INDEX (Cohesion SQL)

Creates a table-scoped secondary index over a plain column list.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
CREATE [ UNIQUE ] INDEX [ IF NOT EXISTS ] index_name
ON [ schema_name . ] table_name ( column_name [ , ...n ] )
[ ; ]
```

## Arguments

- **`index_name`** — The index identity, scoped to its table.
- **`column_name`** — A key column; list order defines composite-key order.
- **`UNIQUE`** — Rejects duplicate encoded keys.
- **`IF NOT EXISTS`** — Allows an index with that name to exist.

## Remarks

Only plain column lists are in this dialect. Per-column `ASC`/`DESC`, expression keys, and
`INCLUDE` are outside the contract. Index construction and writes use the shared index kernel.
An applicable secondary-index prefix can assist the supported inner join.
SELECT, UPDATE and DELETE also use [eligible leading key bounds](select.md#index-access);
[EXPLAIN](explain.md) reports the selected seek.

Unique indexes treat `NULL` as an equal key. String keys use the column's effective collation;
`invariant` is scan-only and cannot back an index or index-backed constraint. Predicates whose
collation differs from the index use a scan.

Duplicate keys during a unique-index build or a later write report `CS03037` (SQLSTATE
23505). The constraint exception carries one Error diagnostic naming the index and table
without key values; a failed build leaves the index unpublished. See [Diagnostics](../diagnostics.md).

Index DDL is self-committing and is rejected inside an explicit transaction with `CS04003`.
An existing index name reports CS03049 (42P07); a conflicting constraint name reports
CS03050 (42710). CREATE INDEX under `invariant` uses CS05013 (no assigned SQLSTATE);
index-backed constraint admission instead uses CS05057 (0A000).
Schema-owned object admission can report CS03066 (42501). Execution/DDL failures retain the
statement span and expose the code separately from Message.

### Building over existing data

The build holds the table's Exclusive lock through construction, validation and publication;
concurrent writers wait. It includes stored historical versions so older snapshots can use the
index, but checks uniqueness against current live keys even if a writer committed after the
DDL captured its initial snapshot. Deleted history does not cause false duplicate failures.

Unique builds construct the whole tree, then compare adjacent live keys in bounded cursor
batches, retaining one previous key. Duplicate-detection memory is independent of row count;
the build uses the existing buffer-pool and spillable pre-image budgets separately. The extra
tree pass delays duplicate detection until construction finishes. Inline builds, deferred
provisioning builds and PRIMARY KEY/UNIQUE backing indexes use this validation.

An inline CREATE UNIQUE INDEX reports the smallest duplicated live key in encoded index order,
rather than the first duplicate in storage order. Its in-process exception's safe scalar
OffendingValue, when available, follows that key; messages and wire diagnostics omit key values.
A duplicate, cancellation or storage failure before publication cleans up the unpublished tree
and registration. After fixing the cause, the same index name can be retried.

Unique and nonunique builds enforce the same **1,024-byte encoded key limit** as later writes.
The count includes NULL markers, component delimiters and collation transforms. A key of exactly
1,024 bytes is accepted; a larger key reports `CS05042` / SQLSTATE 54000, leaves the index
unpublished and cleans up the unpublished tree. The table and connection remain usable. Historical
versions are included in construction. See [size limits](../diagnostics.md#row-and-index-key-limits).
Because construction precedes duplicate validation, CS05042 can take precedence over a
CS03037 duplicate that occurs earlier in storage order.

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
CREATE UNIQUE INDEX ix_name ON t(name);
SELECT id FROM t WHERE name = 'Ada';
```

A subsequent insert of another `Ada` name reports CS03037 (23505) without inserting a row.

In an empty database, bind `v` to `new string('x', 2000)` and execute separately:

```sql
CREATE TABLE wide_keys (id INT, v TEXT);
INSERT INTO wide_keys VALUES (1, @v);
CREATE INDEX ix_v ON wide_keys(v);
SELECT id FROM wide_keys;
```

CREATE INDEX fails with CS05042; SELECT returns `1`, and no index is published.

Execute separately in a fresh database:

```sql
CREATE TABLE duplicate_keys (a INT, b INT);
INSERT INTO duplicate_keys VALUES (1, NULL), (2, NULL), (1, NULL);
CREATE UNIQUE INDEX ix_ab ON duplicate_keys(a, b);
SELECT a, b FROM duplicate_keys ORDER BY a;
```

The build fails with CS03037 (23505) because the two (1, NULL) keys compare equal. No index
is published; SELECT returns (1, NULL), (1, NULL), (2, NULL). SQL has no MISSING key value.

## See also

[DROP INDEX](drop-index.md) · [COLLATE](../clauses/collate.md) · [JOIN](../clauses/join.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Bounded builds, history and failure cleanup** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDdlMemoryTests.cs` and `SqlUniqueIndexBuildTests.cs` in the same directory.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitConformanceTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCollationIndexTests.cs`.

