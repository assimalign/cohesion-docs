# SqlJsonFunctionKind

Selects a SQL/JSON operation.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlJsonFunctionKind { Value, Query, Exists }
```

## Behavior and exceptions

Value selects JSON_VALUE (one SQL scalar, default TEXT). Query selects JSON_QUERY (JSON items,
default JSONB). Exists selects JSON_EXISTS (BOOLEAN). Reading members throws no exceptions.
The parser sets Kind on [SqlJsonFunctionExpression](sql-json-function-expression.md).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("SELECT JSON_EXISTS('null', '$');");
var node = (SqlJsonFunctionExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Kind == SqlJsonFunctionKind.Exists); // True
```

Execution returns TRUE, as checked by SqlJsonPathTests.

## Related reference

[Assembly](index.md) · [SQL/JSON functions](../../../../database/sql/language/functions/json-path.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlJsonFunctionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonPathTests.cs`.
