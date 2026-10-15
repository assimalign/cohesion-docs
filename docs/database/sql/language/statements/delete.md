# DELETE (Cohesion SQL)

Removes rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
DELETE FROM [ schema_name . ] table_name
[ WHERE search_condition ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table from which rows are removed.
- **`search_condition`** — An optional predicate; omission targets all rows.

## Remarks

Only rows for which `WHERE` is `TRUE` are selected. The statement reports an affected count.
Foreign keys default to `ON DELETE RESTRICT`; a declared `ON DELETE CASCADE` removes dependent
rows. Nullable foreign-key values are allowed.

Subqueries in `DELETE` and `RETURNING` report `COHDBL001`. Deleting rows does not remove the
table definition; [DROP TABLE](drop-table.md) is the separate schema operation.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
DELETE FROM t WHERE id = 2;
SELECT id FROM t ORDER BY id;
```

The delete affects one row; the query returns `1` and `3`.

## See also

[WHERE](../clauses/where.md) · [Constraints](../constraints.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

