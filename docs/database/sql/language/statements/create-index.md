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

Duplicate keys during a unique-index build or a later write report `COHSQLE037` (SQLSTATE
23505). The constraint exception carries one Error diagnostic naming the index and table
without key values; a failed build leaves the index unpublished. See [Diagnostics](../diagnostics.md).

Index DDL is self-committing and is rejected inside an explicit transaction with `COHSQLT003`.

Unique and nonunique builds enforce the same **1,024-byte encoded key limit** as later writes.
The count includes NULL markers, component delimiters and collation transforms. A key of exactly
1,024 bytes is accepted; a larger key reports `COHSQLE042` / SQLSTATE 54000, leaves the index
unpublished and cleans up the unpublished tree. The table and connection remain usable. Historical
versions are included in construction. See [size limits](../diagnostics.md#row-and-index-key-limits).

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
CREATE UNIQUE INDEX ix_name ON t(name);
SELECT id FROM t WHERE name = 'Ada';
```

A subsequent insert of another `Ada` name reports COHSQLE037 (23505) without inserting a row.

In an empty database, bind `v` to `new string('x', 2000)` and execute separately:

```sql
CREATE TABLE wide_keys (id INT, v TEXT);
INSERT INTO wide_keys VALUES (1, @v);
CREATE INDEX ix_v ON wide_keys(v);
SELECT id FROM wide_keys;
```

CREATE INDEX fails with COHSQLE042; SELECT returns `1`, and no index is published.

## See also

[DROP INDEX](drop-index.md) · [COLLATE](../clauses/collate.md) · [JOIN](../clauses/join.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`
- **Duplicate keys** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCollationIndexTests.cs`.

