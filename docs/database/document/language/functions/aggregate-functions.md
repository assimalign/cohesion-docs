# Aggregate functions

OQL aggregates summarize expression values within a group. Eleven native aggregate names and
single-argument application aggregates accept ALL and DISTINCT.

## Syntax

```syntaxsql
COUNT ( * )
aggregate_name ( [ ALL | DISTINCT ] expression )
```

## Supported forms

| Function | Admitted inputs | Empty or no admitted inputs | Result |
| --- | --- | --- | --- |
| `COUNT(*)` | Every input row, including expanded UNNEST rows | 0 | Int64 |
| `COUNT(expression)` | Every non-NULL, non-MISSING value | 0 | Int64 |
| `SUM`, `AVG` | Numbers; NULL/MISSING skipped; other valued inputs fail CD02008 | NULL | Decimal |
| `MIN`, `MAX` | Every non-NULL, non-MISSING value, using grouping order | NULL | Selected value type |
| `ARRAY_AGG` | Every non-MISSING value, including NULL, arrays and objects | NULL | Owned JSON array |
| `BOOL_AND`, `EVERY(expression)` | Booleans only; other kinds, NULL and MISSING skipped | NULL | Boolean conjunction |
| `BOOL_OR` | Booleans only; other kinds, NULL and MISSING skipped | NULL | Boolean disjunction |
| `STDDEV`, `VARIANCE` | Numbers only; other kinds, NULL and MISSING skipped | NULL | Decimal sample statistic |

## Arguments

- **`aggregate_name`** — COUNT, SUM, AVG, MIN, MAX, ARRAY_AGG, BOOL_AND, BOOL_OR, EVERY, STDDEV,
  VARIANCE, or a registered single-argument aggregate. Names are case-insensitive.
- **`expression`** — Evaluated over each row in the group.
- **`ALL`** — Includes every admitted input; omission has the same behavior.
- **`DISTINCT`** — Includes each admitted value once, using grouping equality. Numeric 1 and 1.0
  compare equal, strings compare ordinally, arrays compare by elements, and objects compare by
  ordinal member names and values independently of member order, including nested NULL.

## Remarks

Calls execute in SELECT projections, HAVING and ORDER BY. An aggregate query without GROUP BY
has one group even for an empty source. Aggregates cannot occur in WHERE, GROUP BY, RETURNING,
comprehensions or another aggregate's argument. Each call appearance has independent state.
Single-argument [application aggregates](index.md#application-functions) retain their declared
NULL/MISSING policy when using ALL or DISTINCT.

COUNT returns Int64 metadata and CLR long values locally and over the wire. ARRAY_AGG retains
explicit NULL but skips [MISSING](../expressions/missing.md); DISTINCT includes at most one
explicit NULL. Its result owns its JSON and promises no input order. An all-MISSING group
returns NULL rather than an empty array.

Boolean aggregates do not apply SQL++ Truth conversion: BOOL_AND(1) returns NULL.
Aggregate EVERY aliases BOOL_AND. The [quantified EVERY predicate](../expressions/quantified-predicates.md)
keeps its own syntax and returns TRUE for an empty array. A quantified collection or SATISFIES
predicate may contain an aggregate where its enclosing clause permits aggregates; those calls
use the enclosing group. A quantified predicate may also be an aggregate argument, but that
does not permit an aggregate nested inside that argument.

STDDEV and VARIANCE use sample semantics: divide by n - 1 for more than one numeric input,
and return 0 for a singleton. Exact integer moments at decimal scale 28 preserve small differences
at large offsets without intermediate overflow or underflow. VARIANCE rounds the final rational
to decimal using nearest-even rounding. STDDEV uses the BCL square root and shortest round-trip
decimal conversion, using the unrounded rational when variance is not exactly representable as
decimal. The exact rational decides root overflow; if floating conversion crosses the decimal
boundary, exact midpoint comparison rounds the integral root to nearest-even, including
decimal.MaxValue. Final out-of-range results use CD02007 (22003).

### Statement memory

[StatementMemoryBudget](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-database-engine-options.md#statementmemorybudget)
defaults to 64 MiB. Retained SELECT sources, catalog identities and sort staging, group keys and
membership, native accumulators, DISTINCT sets, ARRAY_AGG input/result storage and projected
output share the counter. Input and owned result representations are charged separately while
both live. INSERT/UPSERT SELECT sources and RETURNING share it too.

Exhaustion reports CD05018 before any result is published; aggregate failures retain the call span.
Reservations release with their owning staging, scratch or result lifetime and on failure or
cancellation. Mutation pending-write buffers and application callback-owned state are outside
the estimate. Explicit transaction failures require rollback before further work. EXPLAIN lists
aggregate names and quantifiers without reading documents or evaluating arguments.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00006 | Invalid argument count or star on a function other than COUNT | — |
| CD00011 | ALL/DISTINCT on a scalar call or star, including COUNT(ALL *) | — |
| CD01029 | Invalid enum quantifier, scalar quantifier or quantified star in a directly constructed call AST | — |
| CD02008 | SUM/AVG receives a valued nonnumber | — |
| CD02007 | Numeric overflow or final statistic outside its decimal result range | 22003 |
| CD05001 | Invalid aggregate placement or grouped reference | — |
| CD05018 | Accounted retained statement memory exceeds StatementMemoryBudget | — |

## Examples

In a fresh database, adapted from DocumentQueryTests.Aggregates:

```sql
CREATE COLLECTION items;
INSERT INTO items (KEY, VALUE) VALUES
  ('a',{v:1}),('b',{v:1.0}),('c',{v:3}),('d',{v:NULL}),('e',{});
SELECT COUNT(DISTINCT v) AS n, SUM(ALL v) AS total,
       VARIANCE(DISTINCT v) AS variance, STDDEV(DISTINCT v) AS deviation,
       ARRAY_AGG(DISTINCT v) AS values_array FROM items;
```

The row has n = 2, total = 5, variance = 2 and deviation approximately 1.4142135623730951.
values_array contains 1, 3 and NULL in unspecified order. With these separate Boolean inputs:

```sql
CREATE COLLECTION flags;
INSERT INTO flags (KEY, VALUE) VALUES
  ('a',{flag:TRUE,tags:[TRUE]}),('b',{flag:TRUE,tags:[]});
SELECT EVERY(EVERY x IN tags SATISFIES x END) AS all_documents FROM flags;
SELECT EVERY x IN ARRAY_AGG(flag) SATISFIES x AND EVERY(flag) END AS all_flags
FROM flags;
```

Each SELECT returns one row with TRUE. The first nests the predicate inside the aggregate;
the second uses aggregates in a quantified collection and predicate. Putting EVERY(flag) inside
the first query's SATISFIES would nest aggregates and fail with CD05001.

## See also

[Functions](index.md) · [GROUP BY](../clauses/group-by.md) · [HAVING](../clauses/having.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlAggregateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Aggregates.cs` and `DocumentQueryTests.AggregateFixes.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentStatementMemoryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Functions/DocumentSampleStatistics.cs`.
