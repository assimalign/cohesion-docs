# SqlCreateViewExpression

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCreateViewExpression : SqlQueryExpression
{
    public SqlTableReference View { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlQueryBodyExpression Query { get; }
    public bool OrReplace { get; }
}
```

## Behavior and exceptions

View identifies the namespace/name, Columns is the optional exact output list (empty when omitted),
Query is a SELECT/set body, and OrReplace records the modifier. Construction is internal.
Execution freezes bound columns and function identities. Incompatible replacements use COHSQLE033;
a list-width mismatch uses COHSQLE013. Parameters and unsupported view forms are refused.
See [CREATE VIEW](../../../../database/sql/language/statements/create-view.md).

Accessors add no validation exceptions. Parse returns diagnostics for malformed text;
SqlQueryRequest.FromSql and engine text execution reject parser errors with DatabaseParseException.
View DDL inside an explicit transaction reports COHSQLT003.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE OR REPLACE VIEW dbo.v (a) AS SELECT id FROM t;");
var node = (SqlCreateViewExpression)statement.SqlExpression;
Console.WriteLine(node.View.TableName); // v
Console.WriteLine(node.OrReplace); // True
```

## Related reference

[Assembly](index.md) · [SqlQueryBodyExpression](sql-query-body-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCreateViewExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlViewParserTests.cs`.
