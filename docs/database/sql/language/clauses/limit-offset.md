# LIMIT, OFFSET, FETCH and TOP (Cohesion SQL)

Limits returned rows and skips an initial part of a result.

> **Status:** Implemented. Supports the forms below.

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
  A compound expression needs parentheses: `(1 + 1)` is accepted, `1 + 1` is `CS00003`.
  Parameters, function calls and CASE are primary expressions; signs on nonnumeric operands need parentheses.
- **`TOP`** — A SQL Server lexical alias for a row limit. Without parentheses it consumes one integer literal or parameter.

## Remarks

[Sequence calls](../functions/sequences.md#evaluation-order) in count expressions evaluate once at
execution before input rows. Qualifying OFFSET rows consume projection sequence values before
being discarded; independent projection calls beyond OFFSET + LIMIT are skipped. NEXT VALUE FOR
in a count expression is CS03076 (0A000).

FIRST/NEXT and ROW/ROWS have identical behavior. OFFSET may appear alone. Each SELECT accepts one
limit family and one OFFSET; LIMIT/OFFSET and FETCH/OFFSET accept either order. TOP cannot combine
with LIMIT, OFFSET or FETCH. These rules apply inside subqueries, derived tables, CTEs and INSERT
SELECT as well as at the top level.

Paging follows filtering, grouping, ordering and DISTINCT. A zero limit returns no rows; an offset
at or beyond the result returns no rows. Counts do not wrap through an Int32 cast. Use ORDER BY
with a sufficient tie-breaking key for deterministic selection. SQL has no MISSING count value;
NULL and other noninteger counts fail validation.

Without ORDER BY or DISTINCT, execution stops output evaluation after OFFSET + LIMIT qualifying
rows. OFFSET rows still evaluate projections; rows beyond the window do not. Stored-table and
system-view scans stop reading, and LIMIT 0 reads no input on those paths. Later projection errors
and volatile calls are skipped, as with PostgreSQL's Limit execution. Grouped queries consume
aggregate input but stop group projection and HAVING; correlated queries stop outer-row evaluation.
Join inputs/results and set operands still materialize before bounded final projection.

With ORDER BY, a positive limit and no DISTINCT, fixed ordering collations allow retention of at most
OFFSET + LIMIT candidates, with input order preserved for ties. Ordering still examines all
qualifying inputs. Deferred collations and independent volatile outputs keep full sorting;
[ORDER BY](order-by.md#evaluation-order) describes their evaluation rules. DISTINCT completes
before pagination. These rules also apply to FETCH and TOP. Results still materialize before
delivery. Requested counts never determine an initial allocation size; a sum beyond Int64.MaxValue
saturates without wrapping.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CS00003` | Mixed limit families, repeated clauses, FETCH without ONLY, or malformed count grammar |
| `CK00001` | TOP/FETCH PERCENT, WITH TIES, INSERT/UPDATE/DELETE TOP, or a subquery used as a count |
| `CS02018` | Negative, NULL, noninteger or out-of-range count; the message names LIMIT, OFFSET, FETCH or TOP |
| `CS01005` | Count refers to a column when no columns are in scope (SQLSTATE class 42) |

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

For early-stop examples and their CS02001 (22012) boundary, see
[SELECT](../statements/select.md#d-stop-before-a-later-projection-error).

The renderer retains the limit family, normalizing FETCH to OFFSET before FETCH FIRST and TOP
to `TOP (count)`. PERCENT and TIES are ordinary identifiers outside paging suffixes and render
quoted in persisted SQL (format 19); the current storage format is 30.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Current format 30 adds ON UPDATE action metadata
in table extension 8 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 9.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Early stop, retention and boundary counts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBoundedExecutionTests.cs` and `SqlBoundedSelectionRegressionTests.cs` in the same directory.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Paging parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPagingParserTests.cs`
- **Paging execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPagingExecutionTests.cs`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
