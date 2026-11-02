# SqlJsonWrapper

Controls JSON_QUERY array wrapping.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlJsonWrapper { Without, Conditional, Unconditional }
```

## Behavior and exceptions

Without requires at most one item and is the default. Conditional wraps multiple items only;
Unconditional wraps every nonempty sequence. Empty sequences still invoke ON EMPTY. Reading an
enum member throws no exceptions. OMIT QUOTES with a wrapper is a parser error (CS00003).

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT JSON_QUERY('[1]', '$[*]' RETURNING TEXT WITH CONDITIONAL WRAPPER);");
var node = (SqlJsonFunctionExpression)((SqlSelectExpression)statement.SqlExpression).Columns[0].Expression;
Console.WriteLine(node.Wrapper == SqlJsonWrapper.Conditional); // True
```

Execution returns TEXT `1`, as checked by SqlJsonPathTests.

## Related reference

[Assembly](index.md) · [SqlJsonFunctionExpression](sql-json-function-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlJsonFunctionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJsonPathTests.cs`.
