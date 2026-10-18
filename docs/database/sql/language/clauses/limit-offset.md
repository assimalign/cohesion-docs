# LIMIT, OFFSET, FETCH and TOP (Cohesion SQL)

Limits returned rows and skips an initial part of an ordered result.

> **Status:** Implemented for the forms below.

## Syntax

```syntaxsql
[ LIMIT count ] [ OFFSET count [ ROW | ROWS ] ]
[ OFFSET count [ ROW | ROWS ] ] [ LIMIT count ]
[ OFFSET count [ ROW | ROWS ] ] FETCH { FIRST | NEXT } [ fetch_count ] { ROW | ROWS } ONLY
FETCH { FIRST | NEXT } [ fetch_count ] { ROW | ROWS } ONLY [ OFFSET count [ ROW | ROWS ] ]
SELECT [ DISTINCT ] TOP { ( expression ) | integer_literal | parameter } select_list FROM ...
```

## Arguments

- **`count`** — A column-free expression yielding a nonnegative integer through `9223372036854775807`.
- **`fetch_count`** — A PostgreSQL primary expression (`c_expr`) or signed numeric constant; omission means 1.
  A compound expression needs parentheses: `(1 + 1)` is accepted, `1 + 1` is `SQL0003`.
  Parameters, function calls and CASE are primary expressions; signs on nonnumeric operands need parentheses.
- **`TOP`** — A SQL Server lexical alias for a row limit. Without parentheses it consumes one integer literal or parameter.

## Remarks

FIRST/NEXT and ROW/ROWS have identical behavior. OFFSET may appear alone. Each SELECT accepts one
limit family and one OFFSET; LIMIT/OFFSET and FETCH/OFFSET accept either order. TOP cannot combine
with LIMIT, OFFSET or FETCH. These rules apply inside subqueries, derived tables, CTEs and INSERT
SELECT as well as at the top level.

Paging follows filtering, grouping, ordering and DISTINCT. A zero limit returns no rows; an offset
at or beyond the result returns no rows. Counts do not wrap through an Int32 cast. Use ORDER BY
with a sufficient tie-breaking key for deterministic selection. SQL has no MISSING count value;
NULL and other noninteger counts fail validation.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `SQL0003` | Mixed limit families, repeated clauses, FETCH without ONLY, or malformed count grammar |
| `COHDBL001` | TOP/FETCH PERCENT, WITH TIES, INSERT/UPDATE/DELETE TOP, or a subquery used as a count |
| `COHSQLE018` | Negative, NULL, noninteger or out-of-range count; the message names LIMIT, OFFSET, FETCH or TOP |
| `COHSQLE005` | Count refers to a column when no columns are in scope (SQLSTATE class 42) |

## Examples

Using the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture), each
of the first three statements returns `2`, `3`; the last returns `1`:

```sql
SELECT id FROM t ORDER BY id OFFSET 1 ROW FETCH NEXT (1 + 1) ROWS ONLY;
SELECT id FROM t ORDER BY id FETCH FIRST 2 ROWS ONLY OFFSET 1 ROW;
SELECT id FROM t ORDER BY id OFFSET 1 LIMIT 2;
SELECT TOP (1) id FROM t ORDER BY id;
```

`SELECT id FROM t ORDER BY id FETCH FIRST ROW ONLY;` also returns `1`.

The renderer retains the limit family, normalizing FETCH to OFFSET before FETCH FIRST and TOP
to `TOP (count)`. PERCENT and TIES are ordinary identifiers outside paging suffixes and render
quoted in persisted SQL (format 19); the current storage format is 24.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 adds catalog views; current format 24 persists explicit NULL placement. Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Paging parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPagingParserTests.cs`
- **Paging execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPagingExecutionTests.cs`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
