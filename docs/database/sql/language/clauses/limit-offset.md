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
Counts accept 0 through `9223372036854775807` (`Int64.MaxValue`). The clauses apply after
filtering, grouping and ordering, on table, join and system-relation plans, both in process and
over the wire. A limit larger than the remaining result keeps every remaining row; an offset
larger than the result returns no rows. Counts never wrap through an `Int32` cast.
Invalid counts report `COHSQLE018`; a count expression referring to a column reports `COHSQLE005`.

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

```sql
SELECT id FROM t ORDER BY id LIMIT 9223372036854775807;
SELECT id FROM t ORDER BY id OFFSET 2147483648;
```

These return `1`, `2`, `3` and no rows, respectively.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
