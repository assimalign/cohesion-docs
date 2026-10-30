# GraphConstraintMetadata

A named single-property type or existence constraint on a label or relationship type.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public readonly record struct GraphConstraintMetadata(
    Guid DefinitionId, string Name, string PropertyKey, DatabaseType? Type = null);
```

## Behavior and exceptions

DefinitionId is the owning label or relationship-type GUID. Name shares the database-wide,
ordinal, case-sensitive namespace with index names. PropertyKey is the constrained property.
A non-NULL Type is a type rule: absent/null values pass, and present non-NULL values must have
that DatabaseType. Type = null is an existence rule requiring a non-NULL property.

Construction stores metadata without validating it. [GraphSchema.CreateConstraintAsync](graph-schema.md)
checks names, owner identity, supported types and existing data. It throws ArgumentException for
blank names/keys or an empty GUID; DatabaseException carries CG03003 for a missing owner or
unsupported type, CG03008 for an occupied name, CG03009 for equivalent/incompatible rules,
CG03010 for invalid existing data and CG02009 for a record exceeding 8,092 encoded bytes.
No SQLSTATE is assigned to these codes. Ownership follows the parent; schema-owned mutation
throws DatabaseObjectLockedException.

Constraint creation and drops require autocommit. An admitted schema write in an explicit
transaction reports CG04025 and aborts it; typed argument checks precede admission. Validation
uses statement work and retained-memory budgets (CG05004/CG05020), rolling the operation back
on exhaustion. These codes have no assigned SQLSTATE.

## Example

Given a GraphSchema and the GUID of an existing ad-hoc label, as in GraphSchemaConstraintTests:

```csharp
await schema.CreateConstraintAsync(new GraphConstraintMetadata(id, "p_type", "p", DatabaseType.Int64));
await schema.CreateConstraintAsync(new GraphConstraintMetadata(id, "p_exists", "p"));
Console.WriteLine((await schema.GetConstraintsAsync()).Count); // 2 in a fresh database
await schema.DropConstraintAsync("p_exists");
Console.WriteLine((await schema.FindConstraintAsync("p_type"))?.Type); // Int64
```

The type rule remains after the independent existence rule is dropped. See the
[complete session example](graph-schema.md#example).

## Related reference

[Assembly](index.md) · [GraphSchema](graph-schema.md) · [GraphPropertyKeyMetadata](graph-property-key-metadata.md)

## Sources

- **Type** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Catalog/ValueObjects/GraphConstraintMetadata.cs`.
- **Behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaConstraintTests.cs`.
