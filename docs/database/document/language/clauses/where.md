# WHERE

Filters source documents using the common SQL++ truth conversion.

## Syntax

```syntaxsql
WHERE expression
```

## Remarks

WHERE keeps only converted TRUE. Nonzero numbers and nonempty strings, arrays and objects are
true; zero and empty values are false. NULL and MISSING do not retain a row. Absent paths,
incompatible shapes, navigation through NULL and out-of-range indices are [MISSING](../expressions/missing.md).
Use IS MISSING for absence and IS NULL for explicit null. Ordinary unknown comparisons propagate
MISSING before NULL. Equality includes structural values; ordering requires matching scalar kinds.

[IN, BETWEEN, LIKE ESCAPE](../expressions/operators.md), [CASE](../expressions/case.md), arithmetic,
Boolean operators and [parameters](../expressions/parameters.md) execute in predicates. Applicable
path equality/range bounds can seek an index; the complete predicate is reapplied to candidates.
The same filter and access selection applies to [DELETE](../statements/delete.md), which collects
all matching snapshot identities before deleting. Its NULL/MISSING predicates delete nothing.

## Examples

For items with `{"name":"beta","amount":6}`:

```sql
SELECT name FROM items WHERE 1;
SELECT name FROM items WHERE amount IN (6, NULL) AND name LIKE 'b%';
```

Both return beta. Replacing the condition with NULL or MISSING returns no rows.

## See also

[Clauses](index.md) · [Operators](../expressions/operators.md) · [HAVING](having.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Predicates.cs`.
- **Truth** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/DocumentExpressionEvaluator.Values.cs`.
