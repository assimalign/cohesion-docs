# Aggregate functions

Counts, summarizes or collects non-NULL values from each group.

> **Status:** Implemented.

## Syntax

```syntaxsql
count(*)
count([ DISTINCT | ALL ] expression)
{ sum | avg | min | max | collect_list | collect }([ DISTINCT | ALL ] expression)
```

## Arguments

- **`expression`** — One value expression evaluated for each input binding. NULL is skipped; GQL has no MISSING value.
- **`DISTINCT`** — Removes equal non-NULL argument values before aggregation. ALL is the default.
- **`*`** — Counts input rows, including an OPTIONAL MATCH's NULL-extended row; only bare count(*) accepts star.

## Results

| Function | Result | Empty or all-NULL input |
| --- | --- | --- |
| count(*) / count(expression) | Int64 row/non-NULL count; direct COUNT columns also report Int64 on the wire | Int64 0 |
| sum | Int64 for integral inputs; Double if any retained input is floating; Decimal if any is Decimal or UInt64, taking precedence over floating | Int64 0 |
| avg | Double for integral/floating inputs, including UInt64; Decimal if any retained input is Decimal | NULL |
| min / max | Least/greatest non-NULL input under ORDER BY's total order; preserves its runtime type | NULL |
| collect_list / collect | List of retained inputs in encounter order, with duplicates unless DISTINCT is used | Empty list |

Integral/Decimal subtotals remain exact until final promotion. A mixed integer/floating SUM converts
the integral subtotal to Double once. Decimal SUM preserves the largest input scale; integral
inputs have scale zero. Decimal AVG rounds once, ties to even, to available precision. Mixed
Decimal/floating aggregation converts floating inputs to Decimal and fails when unrepresentable.
AVG can stay finite even when summing its inputs directly would overflow. Numeric overflow,
non-finite numeric results and nonnumeric SUM/AVG inputs report CG02013.

## Grouping, NULL and ordering

Every RETURN item without an aggregate supplies an implicit grouping key. ISO
[GROUP BY](../clauses/group-by.md) can name those items explicitly, including RETURN aliases or
an empty grouping set. Without keys there is one group even when MATCH finds no rows; an empty
keyed match yields no groups. Grouping precedes projection, RETURN DISTINCT, ORDER BY and paging.

Grouping and aggregate DISTINCT use exact represented numeric equality: 1, 1.0 and Decimal 1
coalesce; adjacent Int64 values above 2^53 remain distinct, and Decimal 0.1 differs from Double 0.1.
Lists/maps compare structurally, including corresponding NULLs; entities compare by identity.
MIN/MAX use [ORDER BY's total order](../clauses/order-by.md), including composite values.
COLLECT_LIST skips top-level NULL but retains NULL inside an input list/map.

Aggregate results may compose expressions with grouping keys. Nested aggregates and aggregates in
WHERE or GROUP BY fail before execution. ORDER BY may reuse a projected aggregate or its alias;
an unprojected aggregate or ungrouped source reference is invalid. Aliases shadow input bindings
in ORDER BY. RETURN items themselves use input scope.

## Wire and bounds

COLLECT_LIST requires graph.composite-values on protocol 2.1, even for an empty result. Constructed
lists have depth 64, 65,536 total values including the root, and 16 MiB encoded-byte bounds, checked
during accumulation (CG02013). Exact Decimal bits and scale survive negotiated parameters and
results. Scalar COUNT needs no composite capability. Embedded aggregates may select or collect
entities; final lists of nodes/relationships remain outside wire Execute (CG05001; #1662), even
with composite negotiation. Intermediate [WITH](../clauses/with.md) aggregation can collect
entities and [FOR/UNWIND](../clauses/for.md) retains proven element kinds. ExecutePaths rejects
final grouping/aggregate projections with CG05001, but permits such intermediate stages when
the final column is one mandatory bare entity/path and the statement has no writes.

Grouping consumes input incrementally, retaining group/aggregate state within the shared memory
budget. LIMIT does not reduce the input needed to form complete groups. Aggregate result errors roll back
statement mutations. EXPLAIN describes GroupAggregate without evaluating arguments.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `CG00002` | Wrong arity, missing argument, SUM(*), COUNT(DISTINCT *) or COUNT(ALL *) |
| `CG05001` | Nested/misplaced aggregate, ungrouped reference, GROUP BY mismatch/aggregate alias, missing operand parameter or invalid path request |
| `CG02013` | SUM/AVG nonnumeric operand, overflow/non-finite result, unrepresentable Decimal conversion or collection bound |
| `CG07016` | Possible composite wire result without negotiated graph.composite-values |
| `CK00001` | HAVING, WITH composition, STDDEV_SAMP/POP or PERCENTILE_CONT/DISC |

These aggregate failures have no assigned SQLSTATE. Invalid parameters retain their
[normal diagnostics](../diagnostics.md), including CG03003 for non-finite CLR inputs.

## Examples

Execute separately in an empty graph:

```sql
INSERT (:Person {name:'Alice', age:42, active:TRUE}),
       (:Person {name:'Bob', age:17, active:FALSE}), (:Person {name:'Isolated', age:30});
MATCH (a:Person) RETURN count(*) AS rows, count(a.absent) AS absent,
    sum(a.age) AS total, avg(a.age) AS mean, min(a.age) AS youngest,
    max(a.age) AS oldest, collect_list(a.active) AS active;
```

One row: `(3, 0, 89, 89 / 3, 17, 42, [TRUE, FALSE])`, where mean is Double
(approximately 29.666666666666668) and both counts and total are Int64.

```sql
MATCH (a:Person), (b:Person) RETURN sum(DISTINCT a.age), collect(DISTINCT a.active);
MATCH (a:Person) RETURN sum(CASE WHEN a.age=42 THEN 40 WHEN a.age=17 THEN 60 ELSE 0.5 END);
MATCH (a:Nobody) RETURN count(*), sum(a.age), avg(a.age), min(a.age), max(a.age), collect(a.name);
```

Results are `(89, [TRUE, FALSE])`, Double `100.5`, and `(0, 0, NULL, NULL, NULL, [])`.
The last query also reports the unknown-label warning when Nobody has no catalog entry.

## See also

[Functions](index.md) · [GROUP BY](../clauses/group-by.md) · [RETURN](../clauses/return.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphAggregateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphAggregateAccumulator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphAggregateWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
