# NULLIF

Returns NULL when two operands compare TRUE for equality; otherwise returns the first operand.

> **Status:** Implemented.

## Syntax

```syntaxsql
NULLIF ( expression, expression )
```

## Remarks

Both operands evaluate once. Equality uses the exact numeric and [structural comparison](list-values.md#comparison)
rules; UNKNOWN equality does not replace the first operand. The returned value keeps its runtime
type, with no common-type coercion. GQL has NULL and no MISSING value. NULLIF is a case abbreviation,
independent of the function registry.

Malformed arity/syntax reports GQL0002; invalid programmatic shapes or unbound names use COHDBG001.
Evaluated value failures/limits use COHDBG013. A possibly composite first operand returned over a
wire session without graph.composite-values reports COHDBG016 before reads/writes (no SQLSTATE).
See [limits and capability](list-values.md#limits-and-wire-capability).

## Examples

```sql
RETURN NULLIF(1, 1.0);
RETURN NULLIF(1, NULL);
RETURN NULLIF([NULL], [NULL]);
```

Results are NULL, Int64 1 and the list [NULL]. The last equality is UNKNOWN, so the first list survives.

## See also

[Expressions](index.md) · [CASE](case.md) · [COALESCE](coalesce.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphExpressionEvaluator.Values.cs`.
