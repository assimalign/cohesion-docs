# ANY, SOME and EVERY

Tests an array with one variable scoped to a SATISFIES predicate.

## Syntax

```syntaxsql
{ ANY | SOME | EVERY } variable IN array_expression SATISFIES predicate END
```

## Remarks

SOME is a synonym for ANY. The collection evaluates in the enclosing scope, then each element
binds variable for SATISFIES. The innermost variable wins and may shadow a document field,
FROM alias or outer UNNEST/quantifier variable. Nested quantifiers retain independent scopes.

Each predicate result undergoes SQL++ Truth conversion. ANY returns TRUE at the first true
result and otherwise FALSE. EVERY returns FALSE at the first result that is not true and
otherwise TRUE. NULL and MISSING predicate results are not true; array evaluation returns a
Boolean, never NULL merely because an element predicate is unknown.

| Collection and predicate results | ANY/SOME | EVERY |
| --- | --- | --- |
| Empty array | FALSE | TRUE |
| Nonempty array, all results true | TRUE | TRUE |
| Both true and not-true results | TRUE | FALSE |
| Nonempty array, no true results, including NULL/MISSING results | FALSE | FALSE |
| MISSING collection | MISSING | MISSING |
| NULL or non-array collection | NULL | NULL |

NOT negates Boolean results and preserves collection NULL/MISSING. Quantifiers work in
projections, filters and UPDATE/DELETE predicates or values. Top-level conjunctive ANY/SOME
predicates over document paths can use [DISTINCT array indexes](../statements/create-index.md#array-access-and-fetch-bounds).
The full predicate still evaluates on each candidate, preserving same-element semantics and
independent quantifier witnesses. EVERY, NOT ANY and OR-only element predicates do not supply
array seeks.

[Aggregate EVERY(expression)](../functions/aggregate-functions.md) aliases BOOL_AND and skips
NULL, MISSING and non-Booleans; it is distinct from this predicate. In SELECT, HAVING and ORDER BY,
collections and SATISFIES predicates may use aggregates over the enclosing group. A predicate
may also be an aggregate argument, but an aggregate nested anywhere in that argument remains
invalid. See the [nesting examples](../functions/aggregate-functions.md#examples).

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CD05001 | META(variable) on a quantified variable, including one shadowing the FROM alias; aggregate in a forbidden enclosing context or nested inside another aggregate |
| CK00001 | Multi-binding quantifiers, ANY AND EVERY, ODMG FOR ALL/EXISTS |
| CD00010 | Duplicate names within one parsed binding list |

No SQLSTATE is assigned to these codes. A nested quantifier may reuse an outer name.

## Example

With one document in items:

```sql
SELECT ANY x IN [] SATISFIES x = 1 END AS a,
       SOME x IN [1, NULL] SATISFIES x = 1 END AS s,
       EVERY x IN [] SATISFIES x = 1 END AS e,
       EVERY x IN [1, NULL] SATISFIES x = 1 END AS n,
       NOT ANY x IN [NULL] SATISFIES x = 1 END AS negated
FROM items;
```

The row is {"a":false,"s":true,"e":true,"n":false,"negated":true}.
An absent collection path yields a MISSING projected member; a NULL collection yields a null member.

## See also

[Expressions](index.md) · [ARRAY/FIRST](comprehensions.md) · [UNNEST](../clauses/unnest.md) · [Operators](operators.md) · [MISSING](missing.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Arrays.cs`.
