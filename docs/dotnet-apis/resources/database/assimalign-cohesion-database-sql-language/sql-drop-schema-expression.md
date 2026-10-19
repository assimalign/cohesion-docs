# SqlDropSchemaExpression

Parsed DROP SCHEMA and its dependency policy.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public sealed class SqlDropSchemaExpression : SqlQueryExpression
{
    public string Name { get; }
    public bool IfExists { get; }
    public bool Cascade { get; }
}
```

## Behavior and exceptions

Construction is internal to the parser; accessors add no exceptions. CommandType is Drop.
Cascade false means RESTRICT, including when omitted. CASCADE atomically drops contained tables/
indexes and removes incoming FKs while preserving outside tables. IF EXISTS suppresses absence,
not dbo/virtual-namespace or ownership protection. Codes are COHSQLE028 (3F000) for absence,
COHSQLE030 (2BP01) for RESTRICT dependencies and COHSQLE031 (no SQLSTATE) for dbo. Reserved
virtual namespaces use COHSQLE021 (25006), explicit transactions COHSQLT003, and changed
dependency sets COHSQLE013. Ownership refusals use DatabaseObjectLockedException.

## Example

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse("DROP SCHEMA IF EXISTS sales CASCADE;");
var schema = (SqlDropSchemaExpression)statement.SqlExpression;
Console.WriteLine(schema.IfExists); // True
Console.WriteLine(schema.Cascade); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [SqlCreateSchemaExpression](sql-create-schema-expression.md) · [DROP SCHEMA](../../../../database/sql/language/statements/drop-schema.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Expressions/SqlDropSchemaExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlSchemaParsingTests.cs`.
