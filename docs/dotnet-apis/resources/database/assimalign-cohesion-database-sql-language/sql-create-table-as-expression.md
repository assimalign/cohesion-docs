# SqlCreateTableAsExpression

The parsed CREATE TABLE AS destination, complete optional output names and population policy.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCreateTableAsExpression : SqlQueryExpression
{
    public SqlTableReference Table { get; }
    public IReadOnlyList<string> Columns { get; }
    public SqlQueryBodyExpression Query { get; }
    public bool IfNotExists { get; }
    public bool WithData { get; }
}
```

## Behavior and exceptions

Construction is internal; obtain the node through SqlQueryParser. Columns is empty when no rename
list was written. WithData defaults to true; WITH NO DATA sets it to false. CommandType is Create.
Parsing does not bind output names or execute the query; malformed forms produce parser diagnostics.
Execution infers nullable columns and publishes the new table atomically. Invalid output names use
CS03068 (42601); explicit transactions use CS04003 (25001). See the
[statement contract](../../../../database/sql/language/statements/create-table.md#create-table-as)
for domain defaults, memory, affected counts and snapshot visibility.

## Example

From SqlCreateTableAsParserTests; no catalog is needed:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE TABLE t (n) AS WITH q AS (SELECT 1 AS n) SELECT n FROM q WITH NO DATA");
var create = (SqlCreateTableAsExpression)statement.SqlExpression;
Console.WriteLine(create.Table.TableName); // t
Console.WriteLine(create.Columns[0]);      // n
Console.WriteLine(create.WithData);        // False
```

## Related reference

[Assembly](index.md) · [SqlQueryBodyExpression](sql-query-body-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCreateTableAsExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCreateTableAsParserTests.cs`.
