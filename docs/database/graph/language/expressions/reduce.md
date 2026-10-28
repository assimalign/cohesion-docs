# reduce

Folds an ordered list into one value using a local accumulator and iterator.

## Syntax

```syntaxsql
reduce(accumulator = initial, variable IN list_expression | expression)
```

The bar is literal syntax. This is a binding expression, rather than an ordinary scalar call.

## Remarks

The source evaluates once in the enclosing scope, before the initial value. For a non-NULL list,
the initial value evaluates once; each element then evaluates the body in source order using the
current accumulator and iterator. The body result becomes the next accumulator. The accumulator
may change type, including to NULL, a list or a map. An empty list returns the initial value
without evaluating the body. A NULL source returns NULL without evaluating either initial value
or body. GQL has no MISSING value.

Accumulator and iterator names must be distinct under case-sensitive comparison. They shadow
outer names only in the body; the source and initial value use the enclosing bindings. Nested
binding expressions restore the outer values afterward. Neither declaration escapes into the
surrounding query. All names, parameters and aggregate placement validate even on empty/NULL
input and under EXPLAIN. Aggregates may supply a source or initial value, or consume the final
reduction; aggregates inside its body are refused.

The expression executes in value positions, including predicates, aggregate operands,
comprehensions, scalar properties and MERGE actions. Stored properties must still be scalar.
Scalar/list/map wire results use the existing composite-value capability; possible entity
results retain the existing Execute refusal (CG05001), even with an empty source.

Each visited element consumes statement work (CG05004). The source, current accumulator and
nested results share statement memory (CG05020); replacement releases the old accumulator.
Failures roll back statement writes. Planning conservatively considers both seed and repeated
body results: proven non-Boolean kinds with no Boolean alternative are invalid in a predicate;
proven non-list kinds with no list alternative are invalid as a list source. NULL remains allowed.
Unknown kinds keep runtime validation. Shape/type planning has a
separate cancellable cap of 65,536 node visits or closure steps (CG05026).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00002 | Malformed binding syntax or missing operand | — |
| CG01018 | Missing/empty declaration, or identical accumulator and iterator names | — |
| CG02015 | Source is neither a list nor NULL; planning rejects known incompatible kinds | — |
| CG02013 | Non-Boolean predicate result or invalid evaluated value | — |
| CG05025 | Malformed body/quantifier AST or aggregate inside the body | — |
| CG05026 | Shape/type planning exceeds 65,536 visited nodes or closure steps | — |
| CG05001 | Unbound name or parameter, including a local used outside its scope | — |

## Examples

Execute separately:

```sql
RETURN reduce(a=0,x IN [1,2,3] | a*10+x);
RETURN reduce(a=42,x IN [] | 1/0);
RETURN reduce(a=1/0,x IN NULL | a);
LET a=10,x=[1,2] RETURN reduce(a=a,x IN x | a+x)+a;
```

Returns scalar `123`, `42`, NULL and `23`, respectively.

## See also

[Expressions](index.md) · [List predicates](list-predicates.md) · [List comprehensions](list-comprehensions.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReduceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReducePlanningTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlReduceExpression.cs`.
