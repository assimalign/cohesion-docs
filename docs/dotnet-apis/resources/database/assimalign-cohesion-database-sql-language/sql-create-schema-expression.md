# SqlCreateSchemaExpression

Parsed CREATE SCHEMA for a SQL namespace, distinct from a compiled schema.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCreateSchemaExpression : SqlQueryExpression
{
    public string Name { get; }
    public bool IfNotExists { get; }
}
```

## Behavior and exceptions

The parser owns construction; Name is unqualified and retains spelling. CommandType is Create.
The accessors add no exceptions. AUTHORIZATION/nested elements report COHDBL001. Execution
rejects parser errors with DatabaseParseException. Existing namespaces use COHSQLE029 (42P06)
unless guarded; reserved virtual namespaces use COHSQLE021 (25006), and explicit transactions
COHSQLT003. Creating a namespace does not redirect code-first provisioning from dbo.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("CREATE SCHEMA IF NOT EXISTS sales;");
var schema = (SqlCreateSchemaExpression)statement.SqlExpression;
Console.WriteLine(schema.Name); // sales
Console.WriteLine(schema.IfNotExists); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlDropSchemaExpression](sql-drop-schema-expression.md) · [CREATE SCHEMA](../../../../database/sql/language/statements/create-schema.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCreateSchemaExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSchemaParsingTests.cs`.
