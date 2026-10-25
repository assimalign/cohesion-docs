# SqlLimitSyntaxKind

Identifies the written SELECT row-limit family.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlLimitSyntaxKind { Limit, Fetch, Top }
```

## Behavior and exceptions

[SqlSelectExpression.LimitSyntaxKind](sql-select-expression.md) is null without a limit.
Limit, Fetch and Top share the same normalized Limit expression and execution path. Top is a
SQL Server lexical alias. Rendering retains the family; FETCH renders FIRST with an explicit
count and TOP renders parentheses. This enum adds no validation exceptions. Parser diagnostics
reject malformed/mixed paging with CS00003, and excluded PERCENT/WITH TIES/DML TOP with CK00001.

## Example

```csharp
var query = (SqlSelectExpression)((SqlQueryStatement)new SqlQueryParser().Parse(
    "SELECT DISTINCT TOP (@n) name FROM t")).SqlExpression;
Console.WriteLine(query.LimitSyntaxKind); // Top
```

## Related reference

[Assembly](index.md) · [Paging](../../../../database/sql/language/clauses/limit-offset.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlLimitSyntaxKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlPagingParserTests.cs`.
