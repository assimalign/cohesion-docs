# SqlJsonBehavior

Records an SQL/JSON empty-sequence or error policy.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlJsonBehavior
{
    Null, Error, Default, EmptyArray, EmptyObject, True, False, Unknown
}
```

## Behavior and exceptions

VALUE and QUERY accept Null, Error and Default on empty/error; QUERY also accepts EmptyArray
and EmptyObject. EXISTS accepts True, False, Unknown and Error on error only. Unknown returns SQL
NULL. Default evaluates the corresponding EmptyDefault or ErrorDefault expression. VALUE/QUERY
default to Null; EXISTS defaults to False on error. Reading members throws no exceptions;
incompatible clause choices produce CS00003 parser diagnostics.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT JSON_EXISTS('{}', 'strict $.a' UNKNOWN ON ERROR);");
var node = (SqlJsonFunctionExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.OnError == SqlJsonBehavior.Unknown); // True
```

Execution returns SQL NULL, as checked by SqlJsonPathTests. Behavior-value errors escape ON ERROR;
see the [error boundaries](../../../../database/sql/language/functions/json-path.md#null-and-error-handling).

## Related reference

[Assembly](index.md) · [SqlJsonFunctionExpression](sql-json-function-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlJsonFunctionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonPathTests.cs`.
