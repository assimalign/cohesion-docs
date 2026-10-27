# Path selectors

Describe which admissible paths a MATCH search selects for each ordered endpoint pair.

Selectors execute in scalar and path requests, in process and over the wire. Counted ANY remains
refused with `CK00001`.

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

| Selector | Selection per endpoint pair |
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
path. All four modes execute, including selected unbounded WALK. Unbounded WALK without
a selector and modes/selectors inside a subpath retain CK00001.

### Prefilters and WHERE

Labels, relationship types/directions, property maps, and inline node/relationship WHERE stay
inside the search. A WHERE inside parenthesized path syntax is also a search prefilter:

```sql
MATCH p = ANY SHORTEST
    ((a)-[r:T WHERE r.w>0]->*(b) WHERE a.k<>b.k)
WHERE path_length(p)>2 RETURN path_length(p) AS hops;
```

The plan searches qualifying paths, selects a shortest path per endpoint pair, then tests its
length. The ordinary MATCH WHERE runs **after selection**, before OPTIONAL MATCH null extension;
it cannot supply an index anchor or an empty-plan shortcut for a selected clause. FILTER and
later WITH predicates remain downstream. NULL does not pass a predicate; GQL has no MISSING
value. If the selected shortest path has at most two hops, this query returns no row for that
pair; it does not replace the selected path with a longer one that passes WHERE.

A search prefilter, with or without a selector, can use its own path's variables, including its named path, and
bindings entering MATCH. Predicates inside a quantified body see singleton occurrences; those
variables are group lists outside it. A prefilter referencing a variable introduced only by a
peer comma-separated path in the same MATCH reports CG01016 at that reference, regardless of
comma order. For example, `EXPLAIN MATCH ANY ((a) WHERE b.k>0), (b) RETURN a;` reports CG01016.
A preceding `MATCH (b)` instead puts b in the incoming scope.

### Quantified patterns and search limits

Selection includes zero-hop paths, cycles, bound/anonymous endpoints and quantified edge or
parenthesized bodies. Repeated-body predicates run with singleton bindings before their group
lists are published. Multiple quantifier decompositions of the same relationship/node sequence
count as one physical path; the first source-ordered accepting decomposition supplies group bindings.

Search uses the statement work and retained-memory budgets (defaults: 1,000,000 work units and
256 MiB). Connectivity visits, repeated prefixes, comparison work, mode histories and output all
count. Work exhaustion or an unresolved search at the 64-hop backstop reports CG05004; retained
state/output exhaustion reports CG05020. No partial selection escapes, and statement writes roll
back on failure. LIMIT does not make an unresolved search safe. ANY over an unbounded WALK can
have no lexicographic minimum because of lower-identity cycles; it refuses instead of returning
an unstable witness. ALL WALK also refuses if qualifying paths extend beyond the backstop.
A completed shortest quota does not explore longer paths.

### Shortest search performance

For element-local predicates, ANY SHORTEST and SHORTEST k use reverse breadth-first distances,
then lazily follow relationship/node identity order toward the target. They retain the transition
graph rather than every predecessor history and do not enumerate all equal-length ties to choose
one or k paths. Longer-length reachability layers prune later searches when k exceeds the shortest
path count; mode history still determines admissibility. Path predicates and correlations outside
the current hop/endpoints retain iterative deepening and identity sorting. ALL SHORTEST and
SHORTEST k GROUPS must handle all selected ties, so combinatorial output can exhaust the budgets.
There is no memory guarantee independent of path histories or output.

### EXPLAIN

EXPLAIN exposes SelectPaths, PathSearch, ElementPrefilter, PathPrefilter and PostFilter. Detail
includes endpoint-pair partitioning, path versus distinct-length counts, resolved mode, order,
and configured work/state/output ceilings. EXPLAIN remains nonexecuting; actual_rows is NULL.
PATH SELECTOR is in the executable language profile. No disk or wire format changes.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00011 | SHORTEST count is missing or is not a positive Int64 literal | — |
| CG01016 | Search prefilter references a peer-path-only binding in the same MATCH | — |
| CK00001 | Counted ANY, nested selector/mode or unselected unbounded WALK | — |
| CG05001 | Invalid direct AST selector kind/count or insertion selector | — |
| CG05004 | Work budget exceeded or unresolved search at the 64-hop backstop | — |
| CG05020 | Retained search history/state or output exceeds the memory budget | — |

## Example

Adapted from GqlPathSelectorExecutionTests; execute separately in a fresh graph:

```sql
INSERT (a:Start)-[:T {w:1}]->(b)-[:T {w:1}]->(z:End),
       (a)-[:T {w:1}]->(c)-[:T {w:1}]->(z),
       (a)-[:T {w:0}]->(z);
MATCH p = ALL SHORTEST (a:Start)-[r:T WHERE r.w>0]->{0,4}(z:End)
RETURN path_length(p) AS hops;
```

The query returns two rows, both with hops = 2. ANY SHORTEST returns one such row; SHORTEST 2
returns both. The w=0 direct edge is excluded before selection. Adding ordinary
`WHERE path_length(p)>2` before RETURN returns no rows. Prefixing the query with EXPLAIN instead
describes SelectPaths, PathSearch and ElementPrefilter without searching.

## See also

[Language (GQL)](index.md) · [Patterns](patterns.md) · [MATCH](statements/match.md) · [EXPLAIN](statements/explain.md) · [WHERE](clauses/where.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Selectors.cs` and `tests/GqlPathSelectorParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.cs` and `tests/GqlPathSelectorPlanningTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlPathSelectorExecutionTests.cs` and `GqlShortestSelectorSearchTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
