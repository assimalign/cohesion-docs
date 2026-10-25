# FOR and UNWIND

Expand a list into one row per element while retaining the incoming bindings.

> **Status:** Implemented. UNWIND is the Cypher compatibility spelling.

## Syntax

```syntaxsql
FOR variable IN list_expression
UNWIND list_expression AS variable
```

## Remarks

The list expression evaluates for each incoming row. Elements become the new variable's values
in list order; duplicate elements produce duplicate rows. NULL or an empty list produces zero
rows. A non-list, including a string or byte array, raises CG02013 rather than becoming one row.
A NULL element in a list still produces a row. GQL has no separate MISSING value.

The variable must be new. Proven homogeneous node/relationship lists, including collect results,
retain their entity kind through FOR/UNWIND and subsequent WITH/NEXT projections. Those variables
can anchor MATCH or DELETE. Mixed/unproven elements remain value bindings; using them as entities
is CG03003. List/map values remain transient and cannot be stored as graph properties.

FOR and matching stream under shared physical-work and retained-memory budgets. Unordered
read LIMIT can stop upstream expansion. Paging does not reduce an admitted mutation target set. Scalar/list/map final wire
results use normal composite negotiation. Lists of nodes or relationships cannot yet be returned
over the wire, even with graph.composite-values (CG05001; #1662). UNWIND them and project scalar
properties, or use the path API for one mandatory entity/path final column.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG05001 | Unbound input/parameter, invalid stage shape, or final entity-list wire result |
| CG03003 | Rebinding the loop variable or using a value variable as an entity |
| CG02013 | Non-list input or runtime element incompatible with a proven entity kind |
| CG05004 | Physical-work budget exceeded |
| CK00001 | WITH ORDINALITY or WITH OFFSET suffix, FOREACH or unsupported scalar call |

No SQLSTATE is assigned to these codes. See [Diagnostics](../diagnostics.md).

## Examples

```sql
FOR x IN [1,2] RETURN x;
UNWIND [1,2] AS x RETURN x;
```

Each returns `1`, `2`. `FOR x IN NULL RETURN x;` returns no rows.
`FOR x IN 1 RETURN x;` fails with CG02013.

On a graph with P nodes Alice and Bob:

```sql
MATCH (a:P) WITH collect(a) AS nodes UNWIND nodes AS n RETURN n.name ORDER BY n.name;
```

Result: `Alice`, `Bob`. The intermediate node list stays inside the engine.

## See also

[LET](let.md) · [WITH](with.md) · [NEXT](next.md) · [Lists](../expressions/list-values.md) · [Aggregates](../functions/aggregate-functions.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphLinearCompositionWireTests.cs`.
