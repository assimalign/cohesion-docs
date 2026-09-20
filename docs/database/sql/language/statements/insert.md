# INSERT (Cohesion SQL)

Adds rows from scalar values or a query to a stored table.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
INSERT INTO [ schema_name . ] table_name
[ ( column_name [ , ...n ] ) ]
{ VALUES ( expression [ , ...n ] ) [ , ...n ] | select_statement }
[ ; ]
```

## Arguments

- **`table_name`** — The destination stored table in the current database.
- **`column_name`** — Destination columns in the order of the supplied values or query outputs.
- **`expression`** — A literal, parameter, or executable scalar expression without a subquery.
- **`select_statement`** — A query within the supported [SELECT](select.md) subset.

## Remarks

Without a column list, values map to all destination columns. Omitted columns use their literal
defaults or nullable `NULL`; an explicit `NULL` does not request the default. Destination coercion,
nullability, checks, primary and unique keys, and foreign keys apply to every row.

`INSERT ... SELECT` validates output width and declared type compatibility even on empty input.
It materializes the entire source under the statement snapshot before writing. Reading from the
destination is therefore supported without feeding newly inserted rows back into the source.
Any failure leaves none of the query-source statement's rows inserted.

Subqueries inside `VALUES` and data manipulation language (DML) `RETURNING` report `COHDBL001`.
Use `INSERT ... SELECT` when input comes from a query.

## Examples

Using the [conformance fixture](select.md#a-create-the-conformance-fixture), this inserts two rows:

```sql
INSERT INTO t VALUES (4, 'Barbara', 33), (5, 'Edsger', 42);
```

This minimally adapted query-source example copies the fixture's older people into a separate table:

```sql
CREATE TABLE selected_people (id INT, name TEXT);
INSERT INTO selected_people SELECT id, name FROM t WHERE age > 40;
```

Execute each statement separately; the insert reports the number of inserted rows.

## See also

[VALUES](../clauses/values.md) · [SELECT](select.md) · [Constraints](../constraints.md)

[Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/samples/Assimalign.Cohesion.Database.Demo/Program.cs`
