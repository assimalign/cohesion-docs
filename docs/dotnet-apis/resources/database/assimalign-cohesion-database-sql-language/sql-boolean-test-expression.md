# SqlBooleanTestExpression

Parsed IS [NOT] TRUE, FALSE or UNKNOWN test.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlBooleanTestExpression : SqlExpression
{
    public SqlExpression Operand { get; }
    public SqlTruthValue TruthValue { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

The parser owns construction. TruthValue selects [True, False or Unknown](sql-truth-value.md),
and IsNegated retains NOT. Depth is one plus Operand.Depth. Rendering and every tree walk
include the operand. Execution accepts BOOLEAN or NULL and returns a non-NULL BOOLEAN;
IS UNKNOWN is true only for NULL. SQL has no MISSING scalar.

Properties add no validation exceptions. Parse returns diagnostics; request/session factories
reject malformed text with DatabaseParseException. Missing/invalid truth keywords use SQL0003.
A known non-Boolean operand, including a supplied parameter, fails binding with
DatabaseException/COHSQLE003 (42804) before reading even an empty input.
See the [six-test truth table](../../../../database/sql/language/expressions/operators.md#boolean-tests).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT flag IS NOT UNKNOWN FROM t;");
var node = (SqlBooleanTestExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.TruthValue); // Unknown
Console.WriteLine(node.IsNegated); // True
```

For a NULL flag, execution returns FALSE; for either Boolean value it returns TRUE.

## Related reference

[Assembly](index.md) · [SqlTruthValue](sql-truth-value.md) · [SqlDistinctExpression](sql-distinct-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlBooleanTestExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPredicateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPredicateExecutionTests.cs`.
