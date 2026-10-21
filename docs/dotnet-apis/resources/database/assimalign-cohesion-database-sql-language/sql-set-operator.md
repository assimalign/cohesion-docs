# SqlSetOperator

The binary operator of a SQL set-operation body.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlSetOperator { Union, Except, Intersect }
```

## Behavior and exceptions

Union combines rows, Except retains left-only rows, and Intersect retains common rows. The node's
IsAll flag selects bag multiplicities; false deduplicates. NULL cells compare equal. The enum
declares no exceptions. See [set operations](../../../../database/sql/language/clauses/set-operations.md)
for type compatibility and diagnostics.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT id FROM a EXCEPT ALL SELECT id FROM b;");
Console.WriteLine(((SqlSetOperationExpression)statement.SqlExpression).Operator == SqlSetOperator.Except);
// True
```

## Related reference

[SqlSetOperationExpression](sql-set-operation-expression.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlSetOperator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSetOperationParserTests.cs`.
