# UPDATE (Cohesion SQL)

Changes one or more column values on rows selected from a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
UPDATE [ schema_name . ] table_name
SET column_name = expression [ , ...n ]
[ WHERE search_condition ]
[ ; ]
```

## Arguments

- **`table_name`** — The stored table being updated.
- **`column_name = expression`** — A destination column and its executable scalar value.
- **`search_condition`** — An optional predicate restricting the affected rows.

## Remarks

Omitting `WHERE` targets every row. The result reports an affected-row count. Values pass through
destination coercion and constraint enforcement. A `CAST` runs before destination storage coercion.

Subqueries in `UPDATE`, including its predicate, and `RETURNING` report `COHDBL001`.
Changing a referenced parent key may fail foreign-key enforcement; `ON UPDATE` actions are not
implemented. Row updates remain available on schema-owned tables even though session DDL is restricted.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
UPDATE t SET age = age + 10 WHERE id = 1;
SELECT age FROM t ORDER BY id;
```

The update affects one row; the query returns `46`, `45`, and `41`.

## See also

[SET](../clauses/set.md) · [WHERE](../clauses/where.md) · [CAST](../expressions/cast.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Update.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`

