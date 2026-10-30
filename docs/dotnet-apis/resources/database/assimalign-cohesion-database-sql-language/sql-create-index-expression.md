# SqlCreateIndexExpression

Parsed secondary-index declaration, including per-column order and an optional partial predicate.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlCreateIndexExpression : SqlQueryExpression
{
    public string IndexName { get; }
    public SqlTableReference Table { get; }
    public IReadOnlyList<string> Columns { get; }
    public IReadOnlyList<SqlIndexKeyOrder> KeyOrders { get; }
    public bool IsUnique { get; }
    public bool IfNotExists { get; }
    public SqlExpression? Predicate { get; }
}

public enum SqlIndexKeyOrder
{
    AscendingNullsLast = 0,
    AscendingNullsFirst = 1,
    DescendingNullsLast = 2,
    DescendingNullsFirst = 3
}
```

## Behavior and exceptions

The constructor is internal; obtain the node from SqlQueryParser.Parse. KeyOrders contains
one effective order per column, resolving omitted options to ASC NULLS LAST or DESC NULLS
FIRST. Predicate is null for an unconditional index. Rendering omits ASC and each direction's
default NULL placement. Parse returns diagnostics; request factories reject parser errors
with DatabaseParseException. PRIMARY KEY/UNIQUE constraint-list options report CS00003.
Engine execution validates predicates and metadata as described by [CREATE INDEX](../../../../database/sql/language/statements/create-index.md).

## Example

Following SqlIndexKeyOrderParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "CREATE UNIQUE INDEX ix ON t(a ASC NULLS LAST,b DESC NULLS FIRST) WHERE a > 0");
var index = (SqlCreateIndexExpression)statement.SqlExpression;
Console.WriteLine(index.KeyOrders[1]); // DescendingNullsFirst
Console.WriteLine(SqlExpressionRenderer.Render(index));
// CREATE UNIQUE INDEX ix ON t (a, b DESC) WHERE a > 0
```

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlCreateIndexExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlIndexKeyOrder.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIndexKeyOrderParserTests.cs`.
