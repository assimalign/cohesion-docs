# COALESCE

Returns the first non-NULL operand, evaluated from left to right.

> **Status:** Implemented.

## Syntax

```syntaxsql
COALESCE ( expression, expression [ , ...n ] )
```

## Remarks

At least two operands are required. Later operands do not evaluate once a non-NULL value is found;
all bindings and AST shapes still validate during planning. All NULL operands produce NULL.
Missing properties are NULL; GQL has no MISSING value. The selected runtime type is preserved,
including lists, maps and entity values. No common-type coercion occurs; metadata remains unknown.
COALESCE is a case abbreviation, separate from aggregate function calls.

Malformed arity/syntax reports GQL0002; invalid programmatic shape or unbound names report
COHDBG001. Evaluated value failures/limits use COHDBG013. A possible composite result over an
unnegotiated wire session reports COHDBG016 before reads or writes, including skipped operands;
no SQLSTATE is assigned. See [limits and capability](list-values.md#limits-and-wire-capability).

## Examples

```sql
RETURN COALESCE(NULL, 2, 1/0);
RETURN COALESCE(NULL, NULL);
```

The results are Int64 2 and NULL. With an optional match, `COALESCE(b,a).name` reads a's property
when b is NULL; see [property access](property-access.md).

## See also

[Expressions](index.md) · [CASE](case.md) · [NULLIF](nullif.md)

## Sources

- **Execution and results** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlValueParserTests.cs`.
- **Value evaluation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Evaluation/GraphExpressionEvaluator.Values.cs`.
