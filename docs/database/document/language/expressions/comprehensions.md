# ARRAY and FIRST comprehensions

Maps an array in input order, optionally filtering elements with WHEN.

## Syntax

```syntaxsql
ARRAY expression FOR variable IN array_expression [ WHEN predicate ] END
FIRST expression FOR variable IN array_expression [ WHEN predicate ] END
```

## Remarks

Both forms accept one IN binding. The input evaluates in the enclosing scope. The mapping and
WHEN predicate see the new variable, which can shadow an outer variable, source alias or field.
Nested comprehensions and ANY/SOME/EVERY have independent scopes; END restores the outer scope.
Mappings may be parenthesized and may contain CASE, constructors or nested comprehensions.

WHEN keeps elements whose result converts to SQL++ Truth true. NULL and MISSING conditions
exclude the element. Only qualifying elements have their mapping evaluated.

| Input or mapping | ARRAY | FIRST |
| --- | --- | --- |
| Array with qualifying elements | Ordered mappings, omitting MISSING and retaining NULL | Mapping of the first qualifying element, including MISSING or NULL |
| Empty array or no qualifying element | `[]` | MISSING |
| All qualifying mappings are MISSING | `[]` | MISSING from the first qualifying element |
| MISSING input | MISSING | MISSING |
| NULL or non-array input | NULL | NULL |

FIRST stops after the first qualifying mapping. It never skips a MISSING or NULL mapping to
look for a later value, and does not evaluate later elements. Named projected MISSING members
are omitted; RAW/VALUE converts top-level MISSING to JSON null.

Comprehensions work in scalar expression positions: SELECT projections, predicates, grouping,
HAVING, ordering, function and aggregate arguments, UNNEST operands, UPDATE values and predicates,
DELETE predicates, and [RETURNING](../clauses/returning.md). Grouped expressions retain ordinary
group-key validation. Closed expressions whose paths refer only to local variables also work in
INSERT/UPSERT VALUES; function calls there keep their existing refusal. Aggregates inside any
child of a comprehension are refused, even for an empty input. META accepts the stored source
binding, never an inner variable shadowing it.

ARRAY uses the existing constructor depth limits; retained RETURNING arrays consume its statement
memory budget. Evaluation checks cancellation per element. This adds no wire or storage format.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CD00002 | Malformed mapping, binding, input, WHEN or missing END | — |
| CD00005 | More than 128 recursive expression parse levels | — |
| CD01027 | Empty variable name in a directly constructed comprehension AST | — |
| CD01005 | Missing parameter in any child, including with empty input | — |
| CD05001 | Aggregate inside a comprehension, invalid grouped reference or META on an inner binding | — |
| CK00001 | Retired ARRAY()/FIRST() calls, WITHIN, position variables, multiple bindings, OBJECT transformation | — |
| CD05015 | Constructed result exceeds the 256-level result depth, including a named wrapper | — |
| CD05018 | Retained RETURNING result exceeds StatementMemoryBudget; writes roll back | — |

Scalar failures retain their usual codes, including CD02006 / 22012 for division by zero.
All children undergo parameter and context validation before data access; an empty array does
not hide invalid expressions.

## Examples

With one document in items:

```sql
SELECT ARRAY x.a FOR x IN [{},{a:NULL},{a:2}] END AS a,
       FIRST x.a FOR x IN [{},{a:2}] END AS absent,
       FIRST x FOR x IN [NULL,2] END AS firstNull,
       ARRAY x * 2 FOR x IN [3,1,2] WHEN x > 1 END AS mapped
FROM items;
```

The row is `{"a":[null,2],"firstNull":null,"mapped":[6,4]}`; absent is MISSING and omitted.
`SELECT ARRAY [x, FIRST x FOR x IN [2] END, x] FOR x IN [1] END AS a FROM items`
returns `{"a":[[1,2,1]]}`, demonstrating restored outer scope.

After creating an empty items collection, execute separately:

```sql
INSERT INTO items (KEY, VALUE) VALUES ('a', {tags:[1,2,3]});
UPDATE items SET mapped = ARRAY x * 2 FOR x IN tags WHEN x > 1 END
RETURNING VALUE FIRST x FOR x IN mapped END;
```

The UPDATE returns one bare `4` with affected count 1.

## See also

[Expressions](index.md) · [Array constructors](array-constructors.md) · [ANY/SOME/EVERY](quantified-predicates.md) · [MISSING](missing.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Comprehensions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlComprehensionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentComprehensionWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.cs`.
