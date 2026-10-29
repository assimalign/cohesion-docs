# SqlFunctionCallExpression

A function call retaining its aggregate input modifiers for catalog-aware planning.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlFunctionCallExpression : SqlExpression
{
    public string FunctionName { get; }
    public IReadOnlyList<SqlExpression> Arguments { get; }
    public SqlSetQuantifier Quantifier { get; }
    public IReadOnlyList<SqlOrderByColumn> OrderBy { get; }
    public bool HasWithinGroupOrdering { get; }
    public SqlExpression? Filter { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain calls from SqlQueryParser. OrderBy holds input keys, including
LISTAGG WITHIN GROUP keys. HasWithinGroupOrdering preserves that syntax for rendering and
admission. Filter holds the optional FILTER condition; null means no suffix. Depth includes
arguments, ordering keys and the condition. The call's location extends through the suffix.
The parser retains modifiers on ordinary
calls without deciding whether they are aggregates.

The engine rejects scalar-call DISTINCT/ALL/ORDER BY, or DISTINCT ordering keys absent from the
argument list, with DatabaseException/CS05032 (no SQLSTATE). Invalid arity/types use CS01006
(42883). Quantifiers on star or malformed LISTAGG ordering use CS00003. Inspect parser diagnostics
before executing. See [Aggregate functions](../../../../database/sql/language/functions/aggregate-functions.md).

The catalog rejects FILTER on a scalar call with DatabaseException/CS05072 (42809), same-scope
aggregates inside its condition with CS05073 (42803), and non-Boolean conditions with CS02003
(42804). Child queries bind their aggregates independently. FALSE/NULL conditions skip arguments,
coercion, ordering and DISTINCT state. See [FILTER](../../../../database/sql/language/clauses/filter.md).

A call followed by OVER is wrapped in [SqlWindowExpression](sql-window-expression.md).
Supported window functions use CS00011 (42601) for invalid arity or modifiers.

## Example

Adapted from SqlAggregateFilterParserTests:

```csharp
var filteredSelect = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT COUNT(*) FILTER (WHERE flag) FROM t;")).SqlExpression;
var filteredCall = (SqlFunctionCallExpression)filteredSelect.Columns[0].Expression;
Console.WriteLine(filteredCall.Filter is not null); // True
Console.WriteLine(SqlExpressionRenderer.Render(filteredCall)); // COUNT(*) FILTER (WHERE flag)
```

```csharp
var select = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT STRING_AGG(DISTINCT name, ',' ORDER BY name) FROM t;")).SqlExpression;
var call = (SqlFunctionCallExpression)select.Columns[0].Expression;
Console.WriteLine(call.Quantifier); // Distinct
Console.WriteLine(call.OrderBy.Count); // 1
Console.WriteLine(call.HasWithinGroupOrdering); // False
```

LISTAGG(name, ',') WITHIN GROUP (ORDER BY id) has one key and HasWithinGroupOrdering true.
Over names c, a, b at IDs 3, 1, 2, it returns `a,b,c`.

## Related reference

[Assembly](index.md) · [SqlSetQuantifier](sql-set-quantifier.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlFunctionCallExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAggregateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlAggregateFilterParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`.
