# LIMIT and OFFSET (Cohesion SQL)

Limits the number of returned rows and skips an initial part of the result.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
[ LIMIT nonnegative_integer ] [ OFFSET nonnegative_integer ]
```

## Arguments

- **`LIMIT`** — The maximum number of rows to return.
- **`OFFSET`** — The number of rows to skip before returning rows.

## Remarks

Each clause can appear alone. When both are present, write `LIMIT` before `OFFSET`.
Values must be nonnegative integers. The clauses apply to completed query results, including groups.

Use a sufficient `ORDER BY` key for deterministic paging. Without ordering, no particular row order
is promised. Pagination values are not select-list ordinals. Subqueries as pagination expressions
are excluded with `COHDBL001`; ordinary numeric pagination on a query containing subqueries works.
`TOP` and `FETCH` are recognized but unsupported.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t ORDER BY id LIMIT 2;
SELECT id FROM t ORDER BY id OFFSET 1;
```

The first query returns `1`, `2`; the second returns `2`, `3`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

