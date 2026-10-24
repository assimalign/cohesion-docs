# VALUES (Cohesion SQL)

Supplies one or more rows of scalar expressions to INSERT.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
VALUES ( { expression | DEFAULT } [ , ...n ] ) [ , ...n ]
```

## Arguments

- **`expression`** — A literal, parameter, or executable scalar expression for the corresponding
  destination column.

## Remarks

Rows map to the explicit insert column list or all destination columns. Destination defaults,
coercion, nullability, and constraints follow [INSERT](../statements/insert.md). Supplying `NULL`
stores a null where allowed; it does not request a default. A whole-item
[DEFAULT](default.md) requests the destination default. `INSERT ... DEFAULT VALUES` inserts one
row using defaults for every column. Column references in VALUES report `CS01005`.

This is an `INSERT` clause, not a standalone query statement. Subqueries in `VALUES` report
`CK00001`; use a query-source insert instead.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
INSERT INTO t VALUES (4, 'Barbara', 33), (5, 'Edsger', 42);
```

The conformance case verifies two affected rows and both inserted names.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
