# SqlDropViewExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDropViewExpression : SqlQueryExpression
{
    public SqlTableReference View { get; }
    public bool IfExists { get; }
    public bool Cascade { get; }
}
```

## Behavior and exceptions

View identifies the target; IfExists accepts absence and Cascade requests dependent-view removal.
Omitted or explicit RESTRICT gives Cascade false. Construction is internal. DROP never binds the
body. Execution refuses a table even with IfExists (CS03033); dependent views under RESTRICT
use CS03302. See [DROP VIEW](../../../../database/sql/language/statements/drop-view.md).

Accessors add no validation exceptions. Parse returns diagnostics for malformed text;
SqlQueryRequest.FromSql and engine text execution reject parser errors with DatabaseParseException.
View DDL inside an explicit transaction reports CS04003.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "DROP VIEW IF EXISTS dbo.v CASCADE;");
var node = (SqlDropViewExpression)statement.SqlExpression;
Console.WriteLine(node.View.TableName); // v
Console.WriteLine(node.Cascade); // True
```

## Related reference

[Assembly](index.md) · [SqlQueryBodyExpression](sql-query-body-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDropViewExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlViewParserTests.cs`.
