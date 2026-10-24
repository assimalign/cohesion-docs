# SqlDistinctExpression

NULL-safe IS [NOT] DISTINCT FROM predicate.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDistinctExpression : SqlExpression
{
    public SqlExpression Left { get; }
    public SqlExpression Right { get; }
    public bool IsNegated { get; }
}
```

## Behavior and exceptions

The parser owns construction. IsNegated identifies IS NOT DISTINCT FROM. Depth is one plus
the greater operand depth; rendering and persisted structural comparison include both children.
Execution returns a non-NULL BOOLEAN: two NULLs are not distinct; exactly one NULL is distinct.
Non-NULL operands use ordinary equality's type, collation, CHAR and NaN rules. T1 uses scans,
including when an index exists. SQL has no MISSING scalar.

Properties introduce no validation exceptions. Parse returns diagnostics; request/session
factories reject parser errors with DatabaseParseException. A missing FROM/comparand or an
unparenthesized non-associative chain uses CS00003. Incompatible comparands retain ordinary
comparison errors. See [predicate behavior](../../../../database/sql/language/expressions/operators.md#is-distinct-from).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT a IS NOT DISTINCT FROM @b FROM t;");
var node = (SqlDistinctExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.IsNegated); // True
Console.WriteLine(SqlExpressionRenderer.Render(node)); // a IS NOT DISTINCT FROM @b
```

With a NULL a and NULL b parameter, execution returns TRUE.

## Related reference

[Assembly](index.md) · [SqlBooleanTestExpression](sql-boolean-test-expression.md) · [SqlLikeExpression](sql-like-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDistinctExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPredicateParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlPredicateExecutionTests.cs`.
