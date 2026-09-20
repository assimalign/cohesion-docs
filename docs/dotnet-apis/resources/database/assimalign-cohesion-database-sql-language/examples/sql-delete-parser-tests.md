# Sql Delete Parser Tests

This example exercises `Assimalign.Cohesion.Database.Sql.Language` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDeleteParserTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Parse_DeleteWithWhere_ParsesTableAndWhere.
- **Case 2** — Parse_DeleteWithoutWhere_HasNullWhere.
- **Case 3** — Parse_DeleteFromSchemaQualifiedTable_ParsesSchema.

## Source example

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Sql.Language.Tests;

public class SqlDeleteParserTests
{
    private readonly SqlQueryParser _parser = new();

    [Fact]
    public void Parse_DeleteWithWhere_ParsesTableAndWhere()
    {
        var statement = (SqlQueryStatement)_parser.Parse(
            "DELETE FROM Users WHERE Id = 1;");
        var delete = statement.SqlExpression.ShouldBeOfType<SqlDeleteExpression>();

        delete.CommandType.ShouldBe(SqlQueryCommandType.Delete);
        delete.Table.TableName.ShouldBe("Users");
        delete.Where.ShouldNotBeNull();

        var binary = delete.Where.ShouldBeOfType<SqlBinaryExpression>();
        binary.Operator.ShouldBe(SqlBinaryOperator.Equal);
    }

    [Fact]
    public void Parse_DeleteWithoutWhere_HasNullWhere()
    {
        var statement = (SqlQueryStatement)_parser.Parse(
            "DELETE FROM Users;");
        var delete = statement.SqlExpression.ShouldBeOfType<SqlDeleteExpression>();

        delete.Table.TableName.ShouldBe("Users");
        delete.Where.ShouldBeNull();
    }

    [Fact]
    public void Parse_DeleteFromSchemaQualifiedTable_ParsesSchema()
    {
        var statement = (SqlQueryStatement)_parser.Parse(
            "DELETE FROM dbo.Users WHERE Id = 1;");
        var delete = statement.SqlExpression.ShouldBeOfType<SqlDeleteExpression>();

        delete.Table.SchemaName.ShouldBe("dbo");
        delete.Table.TableName.ShouldBe("Users");
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlDeleteParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/Assimalign.Cohesion.Database.Sql.Language.Tests.csproj`.
