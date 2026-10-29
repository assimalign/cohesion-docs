# SET (Cohesion SQL)

Assigns destination columns in UPDATE or INSERT ... ON CONFLICT DO UPDATE.

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
No default means NULL where allowed, otherwise `CS03024` (SQLSTATE 23502).
For either [identity mode](identity.md), SET id = DEFAULT allocates a new value. ALWAYS rejects
other explicit identity assignments with CS03025 (428C9); BY DEFAULT accepts them.

The measured `UPDATE` subset accepts multiple assignments and an optional `WHERE`.
Each target may appear only once, using case-insensitive column identity. Repeating a target,
including an assignment using DEFAULT, reports `CS01019` before writes, even when no row
matches. [ON CONFLICT DO UPDATE](on-conflict.md) applies the same duplicate-target rule.
Expressions read the original row values; assigning NULL stores NULL where allowed. SQL has no
MISSING value, and NULL does not request a default.

The documented `SET` belongs to `UPDATE`; it is not session configuration or transaction-isolation
syntax. SET subqueries are outside the executable subset; [RETURNING](returning.md) projects the
updated values. Correlated and uncorrelated
subqueries execute in the UPDATE WHERE predicate.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
UPDATE t SET age = age + 10 WHERE id = 1;
```

The conformance case changes Ada's age to `46`.

```sql
UPDATE t SET age = DEFAULT, AGE = 2 WHERE 1 = 0;
```

This reports CS01019 and changes no rows. DEFAULT still counts as an assignment.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Update.cs`
- **Duplicate targets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlInvalidStatementConformanceTests.cs`.
