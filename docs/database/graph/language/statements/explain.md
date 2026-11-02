# EXPLAIN

Returns the chosen GQL plan as rows without executing the wrapped data statement.

## Syntax

```syntaxsql
EXPLAIN { match_statement | return_statement | insert_statement | create_statement | merge_statement | delete_statement } [ ; ]
```

## Remarks

[Composite queries](../clauses/union.md) show Union/UnionAll, Except/ExceptAll,
Intersect/IntersectAll and Otherwise nodes preserving binary grouping,
every operand's complete plan and correlated subplans. NEXT pipelines show those groups beside
Next and later stage nodes. All operands bind and validate without executing data or callbacks.

[MERGE](merge.md) shows Merge with its probe and OnCreate/OnMatch children. It validates both
branches and equality scope without probing or writing. Eligible row-dependent equalities show
an IndexSeek with the unevaluated expression as its bound.

[Path selectors](../path-selectors.md) execute and support nonexecuting EXPLAIN. Plans expose
SelectPaths, PathSearch, ElementPrefilter, PathPrefilter and PostFilter, with endpoint partition,
count unit, mode, ordering and configured ceilings. MATCH WHERE stays above selection; search
prefilters remain under PathSearch. Actual search uses the statement work/memory budgets.

EXPLAIN also accepts [linear composition](../clauses/next.md). Such plans have a Pipeline root
with stage count/write flag and source-ordered Let, Filter, For/Unwind, Match/OptionalMatch,
With, Return, Next and write-stage nodes as applicable. Each projection stage retains its own
GroupAggregate/Distinct/SortPage details. For example, `EXPLAIN LET x = 1 RETURN x;` describes
Pipeline → Let and Return, with a Project child named x, without evaluating rows.

DELETE statements include their preceding entity bindings. SHOW is not admitted under EXPLAIN.
Normal parameter checks, binding, planning and provisioning admission apply. Mutation plans change
neither nodes nor relationships, including if an explicit transaction subsequently commits.
Nested EXPLAIN and EXPLAIN ANALYZE report CK00001. Plans use the
[shared eight-column shape](../../../sql/language/statements/explain.md): node_id, parent_id,
operator, target, index, detail, estimated_rows, actual_rows. Nodes are depth-first pre-order;
available anchor estimates populate estimated_rows; actual_rows stays NULL. GQL uses ordinary row-result frames, including for an
explained statement whose normal projection is a graph entity.

Operators include NodeScan, LabelScan, IndexSeek and Expand for fixed or quantified segments.
LabelScan and IndexSeek report the actual chosen label in target and the zero-based pattern node
ordinal in detail, including anchors supplied by positive WHERE label conjunctions or later
pattern nodes. IndexSeek also reports the chosen index and property bound. LabelScan seeks
MVCC postings and decodes only selected visible nodes. After bound entities, the smallest estimate
chooses among label scans, property seeks and type anchors. Ties prefer equality seeks, then node
candidates, then relationship candidates, each in source order. A pure pattern union reports target `A|B`
and appends `combine=union` to detail. The full pattern and WHERE remain residual checks. Anchor selection
can change row order in queries without ORDER BY, which have no guaranteed row order.

RelationshipTypeScan reports selected type names in target, a NULL index, and detail containing
`edge=0; direction=Outgoing; postings=(type_id,relationship); combine=union;
physical=visited_postings; viable=accepted_hops; physical_limit=<effective-work-ceiling>`.
The edge ordinal and direction reflect the selected edge. Its posting edge has no Expand row.
Type estimates sum retained publications and possible orientations; label estimates count retained
publications; property seek estimates use a bounded index count. See [anchor selection](match.md#remarks).

Expand also reports
`adjacency=(node,direction,type_id,relationship); seek_direction=Outgoing; type_filter=KNOWS;
physical=visited_candidates; viable=accepted_hops; physical_limit=<effective-work-ceiling>` for an outgoing KNOWS
hop expanded from its left endpoint. seek_direction reverses when expanding left of an anchor;
type_filter is the type expression/name, or % for all types. Quantified bodies describe their hops
with the same fields. These describe [counting rules](../patterns.md#remarks), not runtime totals:
EXPLAIN executes no hops; actual_rows remains NULL.
A test guard requires every plan record's mapping.
Standalone RETURN uses a Return root; projected expressions have Project nodes, with a Distinct
parent when requested. Duplicate final names still fail planning with CG01015 before execution.

Repeated matching clauses have source-ordered Match/OptionalMatch nodes. OptionalMatch detail
reports whether a filter is present, whether the clause matches nothing, and new variables that
would receive NULL. EXPLAIN itself does not perform matching or null extension.

ORDER BY and paging add a SortPage node with `offset` and `limit` (ALL when absent), plus a
SortKey child per key with expression, direction and NULL placement. For example,
`EXPLAIN RETURN 1 AS n ORDER BY n DESC NULLS LAST OFFSET 0 LIMIT 1` reports
`offset=0; limit=1` and a descending, NULL-last key. Counts and key bindings are validated
without sorting or executing mutations.

[Grouping](../clauses/group-by.md) adds GroupAggregate with key count and aggregate expressions.
`EXPLAIN RETURN sum(1 / 0) AS n ORDER BY n` includes `keys=0; aggregates=sum(...)`
without evaluating the division. Invalid placement/grouped references still fail CG05001.

[Quantified expansion](../patterns.md#quantified-paths) describes `quantifier={min,max}; body_hops=n;
trail=True` on Expand (empty max is unbounded). For example, `EXPLAIN MATCH (a)-[:T]->{1,3}(b)
RETURN b` includes `quantifier={1,3}; body_hops=1; trail=True`. Scalar calls and CAST receive normal
signature checks without evaluation: `EXPLAIN RETURN sqrt(-1) AS n` does not evaluate the domain
error, while proven incompatible `EXPLAIN RETURN abs('x') AS n` fails CG03003.

[SET](../clauses/set.md)/[REMOVE](../clauses/remove.md) add Set/Remove stage nodes under Pipeline,
with UpdateEntity children naming each target and property/map/label action. For example,
`EXPLAIN MATCH (n) SET n.x = 1 REMOVE n.old RETURN n.x` describes both actions without writes.

## Examples

On the [MATCH fixture](match.md#examples):

```sql
EXPLAIN MATCH (a)->(b) WHERE b:Keep RETURN a.k, b.k;
```

The LabelScan row has target `Keep`, NULL index and detail `node=1; property=; value=NULL`.
Using `(b:Keep)` instead of the WHERE predicate chooses the same anchor. No matching runs.

On the union fixture in [MATCH](match.md#examples):

```sql
EXPLAIN MATCH (n:A|B) RETURN n.k;
```

The LabelScan row has target `A|B`, NULL index and detail
`node=0; property=; value=NULL; combine=union`. With just the four inserted nodes, estimated_rows
is 4 (the sum of both label populations), and actual_rows is NULL. The query is not executed.

For a path Ada—KNOWS→Bob—KNOWS→Cy and an index by_name on Person.name:

```sql
EXPLAIN MATCH (a:Person {name:'Ada'})-[:KNOWS]->(b)-[:KNOWS]->(c) RETURN a;
EXPLAIN INSERT (:Person {name:'Dee'});
```

The first returns IndexSeek with target Person, index by_name, detail `node=0; property=name;
value='Ada'` and two Expand nodes when its estimate wins. Without that index, labels and fixed
types compete by estimate. The second describes
insertion and creates no Dee node.

```sql
EXPLAIN RETURN DISTINCT 1 + 1 AS two;
```

This returns plan rows with a Return root, one Distinct node and a Project whose target is `two`.

## See also

[Statements](index.md) · [MATCH](match.md) · [DatabasePlanDescription](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-execution/database-plan-description.md)

## Sources

- **Type plans and anchor estimates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphRelationshipTypeAnchorTests.cs`.

- **Union plan rows** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.

- **Label and node ordinal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphWhereLabelAnchorTests.cs`.

- **Exact adjacency details** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs` and `GraphAdjacencyFilterTests.cs`.
- **Plans and no execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphExplainTests.cs`.
- **Wire** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphExplainWireTests.cs`.
- **RETURN plans** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
