# DROP TABLE (Cohesion SQL)

Removes a stored table from the current database.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
DROP TABLE [ IF EXISTS ] [ schema_name . ] table_name [ ; ]
```

## Arguments

- **`table_name`** — The stored table to remove.
- **`IF EXISTS`** — Allows the named table to be absent.

## Remarks

A fresh catalog snapshot no longer includes the dropped table's columns, constraints, indexes, or
ownership rows. System relations remain read-only, including with `IF EXISTS`.
`DROP TABLE ... CASCADE` is not supported; the supported `CASCADE` is a foreign-key deletion action.

This data definition language (DDL) statement commits independently and is rejected within an
explicit transaction with `COHSQLT003`. Ordinary sessions cannot drop schema-owned tables.
A later `ROLLBACK` cannot undo
a completed table drop.

## Examples

Using a disposable copy of the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
DROP TABLE t;
DROP TABLE IF EXISTS t;
```

The second statement succeeds although the table is already absent.

## See also

[CREATE TABLE](create-table.md) · [Transactions](begin-transaction.md) · [System views](../system-views/index.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
