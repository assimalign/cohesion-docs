# DISTINCT (Cohesion SQL)

Removes duplicate projected rows from a SELECT result.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
SELECT DISTINCT expression [ , ...n ]
FROM table_source
```

## Arguments

- **`expression`** — An output expression; duplication is determined from projected values.
- **`table_source`** — A source in the supported `SELECT` grammar.

## Remarks

Top-level `DISTINCT` executes with the supported query surface. String equality and hashing use
the same effective collation. `NULL` values form one distinct class. Numeric equality uses the shared
value comparator, so signed zeros form one class and all floating-point NaNs form one class.

`DISTINCT` inside an aggregate is a different feature and reports `COHDBL001`.
Explicit `SELECT ALL` is also unsupported; omit the modifier to retain duplicates.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT DISTINCT age > 40 FROM t ORDER BY age > 40;
```

This adaptation of the grouping fixture returns `FALSE` then `TRUE`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

