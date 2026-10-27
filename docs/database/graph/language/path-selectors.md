# Path selectors

Describe which admissible paths a MATCH search selects for each ordered endpoint pair.

> **Status:** Parsing, binding and EXPLAIN only (run 2 of 3). Executing a selector is refused
> with `CK00001` in batch 5; batch 6 adds execution. Counted ANY remains refused.

## Syntax

```syntaxsql
MATCH [ path_variable = ] <selector> [ mode ] [ PATH | PATHS ]
    [ GROUP | GROUPS ] path_body
[ WHERE condition ] RETURN projection [ ; ]

<selector> ::= ANY | ALL | ANY SHORTEST | ALL SHORTEST | SHORTEST k
<mode> ::= WALK | TRAIL | SIMPLE | ACYCLIC
```

GROUP/GROUPS applies only to counted SHORTEST. The prefix order is selector/count, mode,
PATH/PATHS, then GROUP/GROUPS. Canonical rendering uses GROUPS. `k` must be a positive Int64
literal; omitted, signed, zero, non-integer, parameter and overflowing counts report CG00011.
`ANY k`, including a parameter count, retains CK00001.

## Remarks

The plan partitions by actual ordered start/end node identities **for each incoming binding**.
Anonymous endpoints, already bound endpoints, equal endpoints, zero-hop paths, interior anchors
and quantified bodies keep this partition. Omitting a selector preserves exhaustive matching;
explicit ALL retains a selection plan.

| Selector | Planned selection per endpoint pair |
| --- | --- |
| ANY | One admissible path in stable identity order |
| ALL | Every admissible path |
| ANY SHORTEST | One path with minimum hop count |
| ALL SHORTEST | Every minimum-hop path; equivalent to SHORTEST 1 GROUPS |
| SHORTEST k | Up to k paths ordered by length, then identity; may split equal-length ties |
| SHORTEST k GROUPS | Every path at up to k distinct shortest lengths; retains all ties |

Stable identity order compares the complete start-to-end relationship-identity sequence
lexicographically, then the node-identity sequence. Length is hop count, not a property weight.
The default mode is **TRAIL**. [Mode identity rules](patterns.md#path-modes) apply to the whole
path. All four modes can be planned, including selected unbounded WALK. Unbounded WALK without
a selector and modes/selectors inside a subpath retain CK00001.

### Prefilters and WHERE

Labels, relationship types/directions, property maps, and inline node/relationship WHERE stay
inside the search. A WHERE inside parenthesized path syntax is also a search prefilter:

```sql
EXPLAIN MATCH p = ANY SHORTEST
    ((a)-[r:T WHERE r.w>0]->*(b) WHERE a.k<>b.k)
WHERE path_length(p)>2 RETURN path_length(p) AS hops;
```

The plan searches qualifying paths, selects a shortest path per endpoint pair, then tests its
length. The ordinary MATCH WHERE runs **after selection**, before OPTIONAL MATCH null extension;
it cannot supply an index anchor or an empty-plan shortcut for a selected clause. FILTER and
later WITH predicates remain downstream. NULL does not pass a predicate; GQL has no MISSING
value. This example returns plan rows, without searching or returning a hop count.

A selected search prefilter can use its own path's variables, including its named path, and
bindings entering MATCH. Predicates inside a quantified body see singleton occurrences; those
variables are group lists outside it. A prefilter referencing a variable introduced only by a
peer comma-separated path in the same MATCH reports CG01016 at that reference, regardless of
comma order. For example, `EXPLAIN MATCH ANY ((a) WHERE b.k>0), (b) RETURN a;` reports CG01016.
A preceding `MATCH (b)` instead puts b in the incoming scope.

### EXPLAIN and execution boundary

EXPLAIN exposes SelectPaths, PathSearch, ElementPrefilter, PathPrefilter and PostFilter. Detail
includes endpoint-pair partitioning, path versus distinct-length counts, resolved mode, order,
and configured work/state/output ceilings. The existing 64-hop, work (CG05004) and retained-memory
(CG05020) contracts remain the search limits; this step does not execute the search.

Every selector **or inline WHERE prefilter**, including one without an explicit selector, is
refused with CK00001 after binding and before reads or writes. This includes empty input,
LIMIT 0 and a pipeline with an earlier write stage. Invalid names or parameters can report their
binding errors first. Wire execution reports ExecutionFailure and keeps the session usable.
No executable-profile clause, disk format or wire format is added.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00011 | SHORTEST count is missing or is not a positive Int64 literal | — |
| CG01016 | Selected prefilter references a peer-path-only binding in the same MATCH | — |
| CK00001 | Selector/inline WHERE execution in this step; counted ANY or nested selector | — |
| CG05001 | Invalid direct AST selector kind/count or insertion selector | — |

## Example

Adapted from GqlPathSelectorPlanningTests; no graph data is required:

```sql
EXPLAIN MATCH p = SHORTEST 2 WALK GROUPS
    ((a WHERE a.k>0)-[r:T WHERE r.w>0]->*(b) WHERE a.k<>b.k)
WHERE path_length(p)>2 RETURN path_length(p) AS hops;
```

The result contains PostFilter above SelectPaths, then PathSearch with ElementPrefilter and
PathPrefilter children. Selection detail includes `partition=endpoint_pair`,
`count=2; count_unit=distinct_lengths; order=length,identity` and `mode=Walk`.
actual_rows is NULL and no data is read. Removing EXPLAIN reports CK00001.

## See also

[Language (GQL)](index.md) · [Patterns](patterns.md) · [MATCH](statements/match.md) · [EXPLAIN](statements/explain.md) · [WHERE](clauses/where.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Selectors.cs` and `tests/GqlPathSelectorParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.cs` and `tests/GqlPathSelectorPlanningTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
