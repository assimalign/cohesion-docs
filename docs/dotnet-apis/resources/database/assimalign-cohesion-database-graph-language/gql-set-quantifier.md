# GqlSetQuantifier

Controls duplicate rows in a RETURN result.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlSetQuantifier
{
    All,
    Distinct
}
```

## Behavior and exceptions

All retains every projected row and is the default [GqlQueryExpression.SetQuantifier](gql-query-expression.md).
Distinct retains the first representative of each complete tuple, preserving its runtime types.
NULLs group together; strings compare ordinally; numbers compare exact represented values across
runtime types. Decimal `0.1m` and Double `0.1d` stay separate, while Int64 `1` and Double `1.0`
collapse. Signed zeros agree; subnormals and large integers retain their values. Comparisons and
IN share this equality; arithmetic keeps its own promotion. Entity/path results compare identities,
including ordered path traversal. DISTINCT does not reduce the number of mutations.

The enum itself throws no exceptions. An undefined value or Distinct without star/items in a
programmatic query fails planning with CG05001. Duplicate final column names fail CG01015,
regardless of quantifier; no SQLSTATE is assigned. Non-finite programmatic AST literals or referenced
parameters fail CG03003 before equality or DISTINCT execution; out-of-range numeric text uses
CG00004 during parsing.

## Example

```csharp
var expression = new GqlBinaryExpression(
    new GqlLiteralExpression(1L), "+", new GqlLiteralExpression(1L));
var query = new GqlQueryExpression([], null, [], [], false,
    [new GqlProjection(expression, "two")]) { SetQuantifier = GqlSetQuantifier.Distinct };
// Executing this standalone query returns one row containing 2L, with column name "two".
```

## Related reference

[Assembly](index.md) · [GqlProjection](gql-projection.md) · [RETURN](../../../../database/graph/language/clauses/return.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSetQuantifier.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphNumericComparisonTests.cs`.
