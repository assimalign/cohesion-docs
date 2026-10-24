# OFFSET, SKIP and LIMIT

Skips and bounds RETURN output after projection, DISTINCT and ordering.

> **Status:** Implemented. Applies after RETURN.

## Syntax

```syntaxsql
RETURN projection [ , ...n ]
[ ORDER BY sort_item [ , ...n ] ]
[ { OFFSET | SKIP } count ]
[ LIMIT count ]

<count> ::= unsigned_integer_literal | parameter
```

## Remarks

OFFSET and the Cypher spelling SKIP share one slot. OFFSET/SKIP must precede LIMIT in both the
ISO and Cypher forms. Either stage can appear alone; each can appear at most once. Counts are
nonnegative Int64 literals or [named parameters](../expressions/parameters.md), with no arithmetic,
column reference or floating literal. Dynamic counts require an integral CLR type in the range
0 through Int64.MaxValue. NULL, floating values (including 1.0), negatives, strings and Booleans
are invalid. GQL has no MISSING value.

OFFSET drops rows first, then LIMIT truncates. LIMIT 0 returns no rows, and an offset beyond the
row count returns none without overflow. Without ORDER BY there is no ordering guarantee suitable
for pagination. Tied sort keys preserve input order; include further keys when the desired page
depends on distinguishing ties.

Paging applies to scalar/composite rows and supported path projections. It never reduces the
mutation set: insertion executes for every matched binding even when LIMIT is zero. For reads, LIMIT without ORDER BY can stop upstream expansion, including LIMIT 0 before
the first move. Final results remain materialized under the statement memory budget. Ordered
finite pages retain a top set bounded by OFFSET + LIMIT; the physical-work budget still applies. EXPLAIN validates counts before returning SortPage plan rows.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CG00002` | Negative, floating or nonnumeric literal, repeated paging clause, OFFSET/SKIP after LIMIT, or ordering after paging |
| `CG00004` | Integer literal exceeds Int64 |
| `CK00001` | Standalone paging before RETURN |
| `CG05001` | Missing parameter; checked before reads or writes |
| `CG03003` | Non-finite or unsupported CLR parameter during binding |
| `CG02013` | Bound graph value is not an integral CLR count in range; rejected before reads or writer locking, including EXPLAIN and empty matches |

No new diagnostic code or SQLSTATE is introduced. See [diagnostics](../diagnostics.md).

## Examples

Using the [Person fixture](order-by.md#examples):

```sql
MATCH (a:Person) RETURN a.name ORDER BY a.name OFFSET 1 LIMIT 2;
MATCH (a:Person) RETURN a.name ORDER BY a.name SKIP $start LIMIT @rows;
MATCH (a:Person) RETURN a.name LIMIT 0;
```

The first returns Bob and Isolated. With start = integral 1 and rows = integral 2, the second
returns the same sequence. The third returns zero rows. `RETURN 1 LIMIT 2 OFFSET 1` is CG00002.

## See also

[Clauses](index.md) · [ORDER BY](order-by.md) · [RETURN](return.md) · [Parameters](../expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOrderPagingParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlOrderPagingTests.cs`.
