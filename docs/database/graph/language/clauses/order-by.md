# ORDER BY

Orders RETURN rows or paths by one or more expression keys.

> **Status:** Implemented after RETURN.

## Syntax

```syntaxsql
RETURN [ DISTINCT | ALL ] projection [ , ...n ]
ORDER BY expression [ ASC | ASCENDING | DESC | DESCENDING ] [ NULLS FIRST | NULLS LAST ] [ , ...n ]
[ { OFFSET | SKIP } count ] [ LIMIT count ]
```

RETURN * also accepts this tail. ASC is the default; ASCENDING and DESCENDING are equivalent
spellings. ORDER BY precedes [OFFSET/SKIP and LIMIT](limit-offset.md).

## Remarks

Keys can use RETURN aliases and projected expressions. Without DISTINCT they may also use
unprojected source bindings. With DISTINCT, references must come from projected values: a projected
entity remains usable under its original binding or alias, and a structurally matching computed
expression is usable. Matching requires literal type and value to agree. A hidden source reference
fails with COHDBG001 even on empty input. An integer key is a constant expression, not a column
ordinal. Aliases do not become visible to neighboring RETURN items.

Missing properties yield NULL; there is no MISSING value. The default follows Cypher: NULL sorts
last ascending and first descending. NULLS FIRST/LAST overrides this for each key regardless of
direction. Nested NULL values use the total value order below, with NULL last before reversing
for DESC. Equal complete keys preserve their input order.

| Ascending comparison | Rule |
| --- | --- |
| Different value families | Map, node, relationship, list, path, string, Boolean, number, NULL |
| Numbers | Exact represented values across numeric types; agrees with equality, IN and DISTINCT |
| Strings | Ordinal, case-sensitive |
| Booleans | FALSE before TRUE |
| Nodes and relationships | Element identity |
| Lists | Lexicographic, then length when one is a prefix |
| Maps | Member count, sorted ordinal keys, then corresponding values |
| Paths | Starting node identity, relationship identity sequence, then length |

Exact comparison distinguishes adjacent large integers above 2^53 and Decimal 0.1m from Double
0.1d. Ordering comparisons in predicates keep their separate NULL/incomparable-value rules.

Mutations run for every matched binding before projection. DISTINCT then removes duplicate output
tuples; sorting, offset and limit follow. All matches and rows materialize, subject to the existing
100,000-match and 1,000,000-candidate bounds. Each sort key is evaluated once per retained row;
map keys, including nested maps, are cached in ordinal order. Sort preparation and comparisons
observe cancellation, which follows ordinary statement rollback behavior. No catalog or wire
format change is required.

With [grouping](group-by.md), ORDER BY sees computed aggregate results and RETURN aliases, which
shadow input variables even inside expressions. A reused input expression is recomputed if an alias
redefines one of its variables. Ungrouped references and aggregates absent from RETURN fail
COHDBG001. For example, `MATCH (a:Person) RETURN count(*) AS n ORDER BY n + 1` returns the
single group with count 3 from the [Person fixture](../functions/aggregate-functions.md#examples).

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| `GQL0002` | Malformed key/null placement, or ORDER BY after paging |
| `COHDBL001` | Standalone ordering before RETURN or intermediate WITH composition |
| `COHDBG001` | Invalid/unbound key or a DISTINCT key referring to a hidden binding; no SQLSTATE assigned |
| `COHDBG003` | Label test on a computed alias evaluates to a non-entity |
| `COHDBG004` | Existing match/candidate cap exceeded despite LIMIT |
| `COHDBG013` | Evaluated key value error, such as division by zero |

Other failures retain their [normal diagnostics](../diagnostics.md).

## Examples

Execute each statement separately in an empty graph:

```sql
INSERT (:Person {name:'Alice', age:42, active:TRUE}),
       (:Person {name:'Bob', age:17, active:FALSE}),
       (:Person {name:'Isolated', age:30});
MATCH (a:Person) RETURN a.name ORDER BY a.active;
MATCH (a:Person) RETURN a.name ORDER BY a.active DESCENDING NULLS LAST;
MATCH (a:Person) RETURN a.name AS person ORDER BY person DESC;
MATCH (a:Person) RETURN a.name ORDER BY a.age DESC SKIP 1 LIMIT 1;
```

The query sequences are Bob/Alice/Isolated, Alice/Bob/Isolated, Isolated/Bob/Alice, and Isolated.
The first uses default NULL-last ascending; the second explicitly keeps NULL last when descending.

## See also

[Clauses](index.md) · [RETURN](return.md) · [OFFSET and LIMIT](limit-offset.md) · [EXPLAIN](../statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlOrderPagingTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphOrderComparer.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphOrderKeyTests.cs`.
