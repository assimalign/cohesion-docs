# SET (Cohesion SQL)

Assigns one or more destination columns in an UPDATE statement.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
SET column_name = { expression | DEFAULT } [ , ...n ]
```

## Arguments

- **`column_name`** — An existing destination column.
- **`expression`** — Its new scalar value, subject to destination conversion and constraints.

## Remarks

`c = DEFAULT` uses the same [default](default.md) as an omitted INSERT column.
No default means NULL where allowed, otherwise `COHSQLE024` (SQLSTATE 23502).
For [BY DEFAULT identity](identity.md), SET id = DEFAULT allocates a new value. ALWAYS rejects
all assignments to its identity column, including DEFAULT, with `COHSQLE025`.

The measured `UPDATE` subset accepts multiple assignments and an optional `WHERE`.
The documented `SET` belongs to `UPDATE`; it is not session configuration or transaction-isolation
syntax. SET subqueries and DML `RETURNING` are outside the executable subset; correlated and uncorrelated
subqueries execute in the UPDATE WHERE predicate.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
UPDATE t SET age = age + 10 WHERE id = 1;
```

The conformance case changes Ada's age to `46`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Update.cs`
