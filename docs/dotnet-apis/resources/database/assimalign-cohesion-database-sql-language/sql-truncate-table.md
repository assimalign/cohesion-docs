# SqlTruncateTable

One optionally namespace-qualified target in a TRUNCATE list.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlTruncateTable
{
    public SqlTableReference Table { get; }
    public bool Only { get; }
}
```

Construction is internal. The parser retains the table reference and whether ONLY was written;
SQL tables currently have no descendants. A target cannot have an alias. Repeated table references
remain in the parsed list but execution ignores duplicates. Getters have no validation exceptions.
Malformed targets report CS00003 in statement diagnostics; execution validates stored-table targets
and closure under the [TRUNCATE contract](../../../../database/sql/language/statements/truncate.md).

## Example

For the [SqlTruncateExpression example](sql-truncate-expression.md#example):

```csharp
var target = truncate.Tables[0];
Console.WriteLine(target.Table.SchemaName); // dbo
Console.WriteLine(target.Table.TableName);  // t
Console.WriteLine(target.Only);             // True
```

## Related reference

[Assembly](index.md) · [SqlTruncateExpression](sql-truncate-expression.md) · [SqlRelationReference](sql-relation-reference.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlTruncateTable.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTruncateParserTests.cs`.
