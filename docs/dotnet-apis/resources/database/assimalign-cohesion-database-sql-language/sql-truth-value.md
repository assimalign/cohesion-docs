# SqlTruthValue

Selects the truth value tested by SqlBooleanTestExpression.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlTruthValue
{
    True,
    False,
    Unknown
}
```

## Behavior and exceptions

True and False select Boolean values; Unknown selects NULL. This enum describes a test rather
than a nullable result value. Negation lives on [SqlBooleanTestExpression](sql-boolean-test-expression.md).
The enum introduces no exceptions. Execution of a truth test over a non-Boolean operand uses
COHSQLE003 (42804).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("SELECT NULL IS UNKNOWN FROM t;");
var test = (SqlBooleanTestExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(test.TruthValue == SqlTruthValue.Unknown); // True
```

Each input row projects TRUE.

## Related reference

[Assembly](index.md) · [Boolean tests](../../../../database/sql/language/expressions/operators.md#boolean-tests)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlTruthValue.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPredicateParserTests.cs`.
