# List comprehensions

Filters and projects an ordered list with a local iterator.

## Syntax

```syntaxsql
[ variable IN list_expression [ WHERE predicate ] [ | projection ] ]
```

The outer square brackets and projection bar are literal syntax.

## Remarks

The source evaluates once in the enclosing scope. The iterator is visible only in WHERE and
projection, where it can shadow an outer name. Nested comprehensions have independent scopes;
the outer binding is restored afterward. An iterator is not introduced into its own source or
the surrounding query. Function names such as sum can be iterator names.

Elements retain source order. Without WHERE, every element passes. Only TRUE passes a filter;
FALSE and NULL discard the element and skip its projection. Without a projection, accepted
elements are retained unchanged. NULL elements and NULL projected values remain list elements.
A NULL source returns NULL; an empty source returns `[]` without evaluating the body.
GQL has no MISSING value. Non-list sources, including strings, maps and binary scalars, use CG02014.

Names and parameters validate even for NULL/empty input and under EXPLAIN. Known non-Boolean
filter types fail planning with CG02013; unknown types are checked only when an element is visited.
An aggregate may supply the source, such as collect(v). Aggregates inside WHERE or projection
use CG05024. [reduce](reduce.md) and the [all/any/none/single list predicates](list-predicates.md)
also execute with local bindings. Pattern comprehensions remain CK00001.

### Properties and MERGE

Property positions parse the complete expression before admission. A scalar result such as
`[x IN [1] | x+10][0]` is allowed in MATCH, INSERT/CREATE and SET property expressions, including
whole-map SET values. A list-valued property, including a parenthesized comprehension or
unindexed slice, still uses CK00001. A dynamic container result uses CG03003 for ordinary
properties and CG03004 for MERGE equalities.

[MERGE](../statements/merge.md) equalities evaluate once per incoming row; comprehensions can
supply their scalar result. ON CREATE SET and ON MATCH SET evaluate the selected branch per row.

List results use the existing [value limits and wire capability](list-values.md#limits-and-wire-capability).
Visited elements share the statement work budget (CG05004); temporary sources, output buffers
and nested results share its memory budget (CG05020). Failures roll back statement writes.
Scalar projections from entity lists work; entity-producing final wire results remain refused
with CG05001, even for empty input or a FALSE filter.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00002 | Malformed comprehension syntax or missing operand | — |
| CG01017 | Missing/empty iterator declaration in a hand-built AST | — |
| CG02014 | Non-NULL source is not a list | — |
| CG02013 | Non-Boolean/non-NULL filter or constructed-value limit | — |
| CG05024 | Aggregate in the filter or projection | — |
| CG05001 | Unbound name, including an iterator used outside its scope | — |
| CK00001 | List-valued property or pattern comprehension | — |

## Examples

Execute separately:

```sql
RETURN [x IN [1,2,3] WHERE x > 1 | x*2];
RETURN [x IN [] | 1/0];
RETURN [x IN [1,2] WHERE NULL | 1/0];
LET x = [1,2] RETURN [x IN x | [x IN [x+10] | x][0]];
MERGE (n:N {k:[x IN [1] | x*2][0]}) RETURN n.k;
```

The results are `[4,6]`, `[]`, `[]`, `[11,12]` and scalar `2`, respectively.

## See also

[Expressions](index.md) · [List values](list-values.md) · [List slices](list-slices.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution and scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphListComprehensionTests.cs`.
- **Filter admission** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCollectionFilterAdmissionTests.cs`.
- **Properties and MERGE** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCollectionPropertyAdmissionTests.cs`.
