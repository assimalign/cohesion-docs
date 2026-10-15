# DROP INDEX (Cohesion SQL)

Removes a named index from its specified stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
DROP INDEX [ IF EXISTS ] index_name
ON [ schema_name . ] table_name
[ ; ]
```

## Arguments

- **`index_name`** — The table-scoped index name.
- **`table_name`** — The owning table; the `ON` qualifier is required.
- **`IF EXISTS`** — Allows the index to be absent.

## Remarks

Index names are not database-global, so `DROP INDEX index_name` is not the complete statement.
Fresh system-view snapshots reflect removal of the index and its key-column rows.

DDL is self-committing and rejected inside explicit transactions with `COHSQLT003`.
Ordinary sessions cannot drop indexes owned by a compiled schema.

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture), execute separately:

```sql
CREATE INDEX ix_age ON t(age);
DROP INDEX ix_age ON t;
DROP INDEX IF EXISTS ix_age ON t;
SELECT id FROM t WHERE age = 36;
```

The final query still returns `1`; removing an index does not remove the table's rows.

## See also

[CREATE INDEX](create-index.md) · [COHESION_SCHEMA](../system-views/cohesion-schema.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`

