# GROUP BY

Partitions RETURN input bindings into groups for aggregation or duplicate-key elimination.

> **Status:** Implemented for implicit and ISO grouping.

## Syntax

```syntaxsql
RETURN projection [ , ...n ]
[ GROUP BY { grouping_name_or_expression [ , ...n ] | () } ]
[ ORDER BY sort_item [ , ...n ] ]
[ { OFFSET | SKIP } count ] [ LIMIT count ]
```

## Arguments

- **`grouping_name_or_expression`** — The name of a nonaggregate RETURN item, or an equivalent input expression.
- **`()`** — Explicit empty grouping set; requires no nonaggregate RETURN keys.

## Remarks

With an aggregate, every nonaggregate RETURN item is an implicit grouping key. Omit GROUP BY for
Cypher-style implicit grouping, or list exactly those distinct keys in any order. A plain name
resolves to a RETURN name first, including a shadowing alias. An aggregate item's alias is invalid
as a grouping name. Other key expressions retain input scope. Structurally duplicated RETURN keys
count once; duplicate explicit keys or a different key set fail COHDBG001.

GROUP BY without an aggregate eliminates duplicate keys. GROUP BY () creates one group even for
an empty match. Empty input with one or more keys returns no rows. Keys compare with exact numeric,
structural composite and entity-identity equality. NULL keys belong to one group; missing properties
yield NULL and GQL has no MISSING state. The first representative retains its runtime types.

A mixed aggregate expression may compose complete grouping-key subexpressions or read properties
of an entity grouped as a key. It cannot introduce an ungrouped variable. RETURN aliases apply to
plain GROUP BY names and later ORDER BY, where they shadow input bindings; RETURN expressions
retain input scope. Grouping precedes projection, RETURN DISTINCT, ordering and paging, and never
reduces match/candidate limits or mutation inputs.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `GQL0002` | Malformed GROUP BY list |
| `COHDBG001` | Key mismatch/duplicate key, aggregate grouping alias, nested/misplaced aggregate or ungrouped RETURN/ORDER BY reference |
| `COHDBL001` | HAVING or intermediate WITH composition |

No SQLSTATE is assigned to these grouping errors. Missing key parameters and invalid runtime
inputs retain their [normal diagnostics](../diagnostics.md). ExecutePaths rejects GROUP BY with
COHDBG001; grouped scalar/list/map results use Execute/QueryAsync.

## Examples

Using the [Person aggregate fixture](../functions/aggregate-functions.md#examples), these three
queries all return `(TRUE, 1)`, `(FALSE, 1)`, `(NULL, 1)`, with Int64 counts:

```sql
MATCH (a:Person) RETURN a.active AS active, count(*) AS n ORDER BY active DESC NULLS LAST;
MATCH (a:Person) RETURN a.active AS active, count(*) AS n GROUP BY active ORDER BY active DESC NULLS LAST;
MATCH (a:Person) RETURN a.active AS active, count(*) AS n GROUP BY a.active ORDER BY active DESC NULLS LAST;
```

```sql
MATCH (a:Person) RETURN count(*) GROUP BY ();
MATCH (a:Nobody) RETURN count(*) GROUP BY ();
MATCH (a:Person) RETURN a.active AS active GROUP BY active ORDER BY active DESC NULLS LAST;
```

The first two yield `3` and `0`; the third yields TRUE, FALSE and NULL once each.
The empty match also reports the unknown-label warning if Nobody is undeclared.

## See also

[Clauses](index.md) · [Aggregate functions](../functions/aggregate-functions.md) · [RETURN](return.md) · [ORDER BY](order-by.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphAggregateTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphGrouping.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlAggregateParserTests.cs`.
