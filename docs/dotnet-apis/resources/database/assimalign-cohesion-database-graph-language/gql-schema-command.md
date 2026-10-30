# GqlSchemaCommand

Semantic standalone schema command attached to GqlQueryExpression.SchemaCommand.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlSchemaCommand
{
    public bool Drop { get; init; }
    public GqlSchemaObjectKind Kind { get; init; }
    public string? Name { get; init; }
    public bool IfExists { get; init; }
    public string? Definition { get; init; }
    public bool Relationship { get; init; }
    public string? Variable { get; init; }
    public string? Property { get; init; }
    public GqlSchemaPropertyType? Type { get; init; }
    public override string ToString();
}

public enum GqlSchemaObjectKind { Index, Constraint, Label, RelationshipType }
public enum GqlSchemaPropertyType
{
    Boolean, Int8, Int16, Int32, Int64, Float32, Float64, Decimal, String
}
```

## Behavior and exceptions

Drop selects CREATE/DROP; IfExists represents IF NOT EXISTS for creation and IF EXISTS for
dropping. Name may be null for generated index/constraint names. Definition is the label or
relationship-type name, Property the single property key and Variable the FOR binding.
Relationship selects a relationship constraint. A null Type denotes existence; a non-null
type denotes a type rule. Drops and label/type commands need Name and no rule fields.

ToString renders semantic fields with escaped double-quoted identifiers and canonical scalar
names. Relationship arrows normalize to an undirected pattern. Construction does not validate
the shape; session execution refuses malformed hand-built commands or combination with query,
catalog, transaction or EXPLAIN operands with CG05001. Path requests also refuse schema DDL.
Text mismatched property variables use CG01022. These codes have no assigned SQLSTATE.

See the [schema statement pages](../../../../database/graph/language/statements/index.md#schema-statements)
for generated names, type aliases, autocommit, CG04025 and limits. GqlCatalogSurface.Constraints
selects SHOW CONSTRAINTS; it is a read-only catalog subject rather than a schema command.

## Example

Following GqlSchemaDdlParserTests:

```csharp
var parsed = (GqlQueryStatement)new GqlQueryParser().Parse(
    "CREATE CONSTRAINT c FOR ()-[r:T]->() REQUIRE r.p IS NOT NULL");
var command = parsed.GqlExpression.SchemaCommand!;
Console.WriteLine(command.Relationship); // True
Console.WriteLine(command.Type is null); // True
Console.WriteLine(parsed.GqlExpression.Stages.Count); // 0
Console.WriteLine(command.ToString());
// CREATE CONSTRAINT "c" FOR ()-["r":"T"]-() REQUIRE "r"."p" IS NOT NULL
```

## Related reference

[Assembly](index.md) · [GqlQueryExpression](gql-query-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSchemaCommand.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlSchemaObjectKind.cs` and `GqlSchemaPropertyType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlSchemaDdlParserTests.cs`.
