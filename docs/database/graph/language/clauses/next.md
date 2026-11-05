# NEXT

Starts the next segment of one source-ordered GQL statement.

> **Status:** Implemented. Applies after RETURN or a modifying segment.

## Syntax

```syntaxsql
RETURN projection [ return_tail ] NEXT next_segment
modifying_segment NEXT next_segment
```

## Remarks

All stages run in source order in one statement transaction. LET/FILTER/FOR, MATCH and writes can
compose; RETURN … NEXT passes its projected rows to the following segment. Only aliases and
bare-variable projections carry referenceable names; star carries every current name in order.
Unaliased computed outputs cannot be named later. NULL values remain rows. GQL has no MISSING value.
See [WITH](with.md) for projection scope and duplicate carried bindings.

NEXT after a modifying segment without RETURN starts with **one empty binding and no scope**.
It does not carry the previous write's per-row bindings. A statement sees its own earlier writes
under Snapshot and ReadCommitted; an unknown label empties its MATCH stage, rather than skipping
unrelated later stages. A write anywhere takes the writer lock before the first stage. Later
paging does not reduce earlier mutations. Failure rolls back the statement under session
transaction rules.

For linear pipelines, only final columns govern wire result admission. Read-only composed queries can use ExecutePaths
for one mandatory entity/path final column; any write stage is refused by that path API. Intermediate
collect/UNWIND is permitted. Final lists of entities remain unsupported over the wire (CG05001; #1662).

[UNION, EXCEPT, INTERSECT and OTHERWISE](union.md) all bind more tightly than NEXT. Each segment can be a composite, whose operands
receive the preceding segment's complete projected binding table in isolated scopes. A NEXT
inside parentheses stays in its operand. For example,
`RETURN 1 AS x UNION ALL RETURN 2 AS x NEXT RETURN x*10 AS x` returns rows 10 and 20.
Composite pipelines must be read-only (CG05029). Every operand is statically wire-admitted.

## Inline CALL subqueries

```syntaxsql
[ OPTIONAL ] CALL [ (variable [ , ...n ]) | (*) | () ] { read_only_query_with_RETURN }
```

CALL invokes its body once per incoming row and joins every returned row to that input.
An empty invocation removes the input row. OPTIONAL CALL instead emits one row with NULL for
each returned binding; an actual NULL result still counts as a real row. Empty outer input
invokes nothing. Even an uncorrelated body runs for each input row, without caching. Grouping,
DISTINCT, ordering and paging apply separately to each invocation.

| Scope | Imports |
| --- | --- |
| Omitted | None, except for the supported leading importing WITH translation below |
| () | None explicitly |
| (*) | All current bindings |
| (a, b) | Exactly the named bindings |

Imports retain their scalar/entity/path kinds and remain visible across inner WITH, NEXT and
composite boundaries. They cannot be rebound (CG03003). A missing import uses CG05001; importing
a deleted variable uses CG03003. Inner declarations stay local and escape only through the final
RETURN. Returned bare variables or aliased expressions add names to the outer row; computed
results require aliases (CG05001).

Every returned name must be absent from the outer scope (CG01024), including imported names and
RETURN * expansions. RETURN * still expands imports after an inner WITH, so it is conservatively
refused with CG01024 when imports exist, even if WITH projects only local names. A returning CALL
cannot conclude the enclosing query by itself (CG05001); follow it with the ordinary RETURN or
mutation conclusion.

With an omitted scope, a leading importing WITH translates to imports only when it contains bare
variables without aliases, or star, and no DISTINCT, GROUP BY, ORDER BY, paging or WHERE. Explicit
ALL is accepted. No later WITH, NEXT or composite stage may follow that importing WITH; composite
operands must import the same set (order may differ). Otherwise this translation refuses CK00001.
With an explicit scope clause, WITH is an ordinary inner projection.

The read-only UNION, EXCEPT, INTERSECT, OTHERWISE and NEXT rules apply inside the braces.
All operands validate even if empty or skipped. The body shares the outer GraphOperation,
snapshot, parameters, transaction clock, cancellation and work/memory budgets. Each invocation
and emitted CALL row adds one work unit beyond inner operator work. Grants and cursors release
on completion, early exit, refusal or cancellation. EXPLAIN shows Apply/OptionalApply and nested
plans without execution. A read-only CALL after an outer write sees that statement's update overlay.

Writing and unit bodies (without RETURN) remain CK00001 for a later run. Named procedures,
FOREACH and CALL IN TRANSACTIONS also remain CK00001. No disk or wire format changes apply.

Execute each query separately:

```sql
CALL { RETURN 1 AS y } RETURN y;
FOR x IN [1,2] CALL (x) { RETURN x+1 AS y } RETURN x,y;
FOR x IN [1,2] OPTIONAL CALL (x) { FILTER x=1 RETURN x+1 AS y } RETURN x,y;
```

The results are `1`; `(1,2), (2,3)`; and `(1,2), (2,NULL)` respectively.
`LET x=1 CALL (x) { RETURN x } RETURN x` instead refuses CG01024 before execution.
The public node is [GqlCallStage](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-call-stage.md).

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG00002 | Leading/trailing NEXT, NEXT after a bare MATCH, or WITH immediately followed by NEXT |
| CG05001 | Reference to a name omitted by a projection or invalid path/wire result shape |
| CG03003 | Reference to a deleted variable or a value used as an entity |
| CG01015 | Duplicate carried name |
| CG01024 | CALL result shadows an outer binding, including imports and RETURN * expansion |
| CK00001 | NEXT YIELD, named CALL, writing/unit CALL bodies, FOREACH, standalone ordering/paging, or immediate RETURN after DELETE |

These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Examples

```sql
LET x = 1 RETURN x AS next NEXT LET y = next + 1 RETURN y;
```

Result: `2`.

```sql
INSERT (:Fresh {k:1}) NEXT MATCH (a:Fresh) RETURN a.k;
```

On an initially empty graph, result: `1`. The insert commits with the statement. The equivalent
INSERT followed directly by MATCH also executes in source order and sees the inserted node.

[SET](set.md) and [REMOVE](remove.md) participate in the same stage order. Later MATCH stages
see their working copies and introduced labels; carried entity references refresh through lists,
maps and paths. One final version publishes per updated entity.

[MERGE](../statements/merge.md) participates in this stage order. Later rows and stages see its
creations and SET changes. NEXT after a write without RETURN starts one empty binding with no scope.

## See also

[WITH](with.md) · [LET](let.md) · [FOR/UNWIND](for.md) · [RETURN](../statements/return.md) · [Clauses](index.md) · [GqlStage .NET API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-stage.md)

## Sources

- **Inline CALL scope and execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlCallSubqueryTests.cs`, `GqlCallReviewTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DIALECT.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphLinearCompositionWireTests.cs`.
