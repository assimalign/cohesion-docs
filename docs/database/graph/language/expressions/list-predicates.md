# List predicates

Tests a list with all, any, none or single using a local iterator and three-valued logic.

## Syntax

```syntaxsql
{ all | any | none | single } (variable IN list_expression WHERE predicate)
```

WHERE is required. These binding forms are separate from ALL as a projection or aggregate set
quantifier: `RETURN ALL (1+2)` retains its existing meaning.

## Remarks

The list evaluates once in the enclosing scope. The iterator shadows an outer name only in the
predicate; nested bindings restore it afterward. A NULL source returns NULL without testing any
element. A NULL element is passed to the predicate, so `x IS NULL` can be TRUE. GQL has no MISSING.

Elements are tested in source order, stopping only on the decisive result below. NULL is not
decisive; single can still return FALSE after seeing NULL if a second TRUE occurs.

| Form | Empty list | Decisive result | After all other elements |
| --- | --- | --- | --- |
| all | TRUE | First FALSE returns FALSE | NULL if any test is NULL, otherwise TRUE |
| any | FALSE | First TRUE returns TRUE | NULL if any test is NULL, otherwise FALSE |
| none | TRUE | First TRUE returns FALSE | NULL if any test is NULL, otherwise TRUE |
| single | FALSE | Second TRUE returns FALSE | NULL if any test is NULL, otherwise TRUE for exactly one TRUE |

Tests must return Boolean or NULL. Statically non-Boolean predicates and non-list sources fail
planning even for empty/NULL input and EXPLAIN; unknown kinds are checked only when visited.
Names and parameters validate regardless of execution. An aggregate may supply the source or
consume the result, but cannot appear inside the predicate. The forms execute in value positions,
including WHERE, RETURN, comprehensions, scalar properties and MERGE actions.

Visited elements share execution work (CG05004) and nested values share memory (CG05020).
[Reduce shape/type analysis](reduce.md) shares a separate 65,536-node/closure-step planning cap;
failure uses CG05026. Statement failures roll back writes. Pattern comprehensions remain CK00001.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00002 | Malformed binding syntax, including missing WHERE | — |
| CG01018 | Missing/empty iterator declaration in a hand-built AST | — |
| CG02013 | Test is neither Boolean nor NULL; known incompatible kinds fail planning | — |
| CG02015 | Source is neither a list nor NULL; known incompatible kinds fail planning | — |
| CG05025 | Malformed body/quantifier AST or aggregate inside the predicate | — |
| CG05026 | Reduce shape/type planning exceeds 65,536 visited nodes or closure steps | — |
| CG05001 | Unbound name or parameter, including an iterator used outside its scope | — |

## Examples

```sql
RETURN all(x IN [] WHERE x), any(x IN [] WHERE x),
       none(x IN [] WHERE x), single(x IN [] WHERE x);
```

Returns `(TRUE, FALSE, TRUE, FALSE)`. Replacing each source with `[TRUE,NULL]` returns
`(NULL, TRUE, FALSE, NULL)`; with `[NULL,TRUE,TRUE]`, it returns `(NULL, TRUE, FALSE, FALSE)`.
With a NULL source all four results are NULL.

## See also

[Expressions](index.md) · [reduce](reduce.md) · [List comprehensions](list-comprehensions.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReduceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReducePlanningTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlReduceParserTests.cs`.
