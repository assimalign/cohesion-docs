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

Unique indexes treat `NULL` as an equal key. String keys use the column's effective collation;
`invariant` is scan-only and cannot back an index or index-backed constraint. Predicates whose
collation differs from the index use a scan.

Index DDL is self-committing and is rejected inside an explicit transaction with `COHSQLT003`.

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
CREATE UNIQUE INDEX ix_name ON t(name);
SELECT id FROM t WHERE name = 'Ada';
```

A subsequent insert of another `Ada` name violates the unique index.

## See also

[DROP INDEX](drop-index.md) · [COLLATE](../clauses/collate.md) · [JOIN](../clauses/join.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.cs`

