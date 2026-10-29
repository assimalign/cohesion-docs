# CompiledSchemaConstraint

An immutable compiled foreign-key or CHECK declaration, including its referential actions.

Namespace: `Assimalign.Cohesion.Database.Sql.Schema`.

## Signature

```csharp
public enum CompiledSchemaReferentialAction : byte
{ Restrict = 0, Cascade, NoAction, SetNull, SetDefault }
public sealed record CompiledSchemaConstraint(
    string Name, CompiledSchemaConstraintKind Kind, IReadOnlyList<string> Columns,
    string? ReferencedObject, IReadOnlyList<string> ReferencedColumns,
    CompiledSchemaExpression? Expression = null,
    CompiledSchemaReferentialAction OnDelete = CompiledSchemaReferentialAction.Restrict,
    CompiledSchemaReferentialAction OnUpdate = CompiledSchemaReferentialAction.NoAction)
{
    public DatabaseObjectOwner Owner { get; }
}
```

The positional arguments expose properties with the same names. Columns and ReferencedColumns
are copied into immutable snapshots; Owner is Schema.

## Behavior and exceptions

OnUpdate accepts all five enum values and defaults to NoAction. OnDelete remains limited to
Restrict/Cascade for compiled foreign keys, with Restrict as its default. The typed References
builder keeps these defaults; set update metadata through this concrete model. CHECK requires
OnDelete Restrict and OnUpdate NoAction. SqlCompiledSchema validation raises
SqlSchemaValidationException with InvalidDocument at the constraint's `onUpdate` path for an
invalid enum or nondefault CHECK update action. Construction itself does not enforce these rules.

Compiled format `cohesion/database-schema/v9` serializes onUpdate, including its default.
Changing it changes the canonical document/hash and plans DropConstraint then AddConstraint.
Serialization round-trips the action; provisioning, reconciliation, compensating statements,
catalog rename and reopen retain it. Earlier artifacts, applied markers and pending targets
are refused at schema.format before comparison; no upgrade reader exists.

The action is a declaration in this release. Parent-key changes under Cascade, SetNull or
SetDefault raise DatabaseException/CK00001 before row writes. Restrict and NoAction retain
statement-end checks. See [ON UPDATE](../../../../database/sql/language/constraints.md#on-update-declarations).

## Example

Adapted from CompiledSchemaConstraintTests:

```csharp
var reference = new CompiledSchemaConstraint("fk", CompiledSchemaConstraintKind.Reference,
    ["id"], "parent", ["id"]);
var cascade = reference with { OnUpdate = CompiledSchemaReferentialAction.Cascade };
Console.WriteLine(reference.OnUpdate); // NoAction
Console.WriteLine(cascade.OnUpdate);   // Cascade
```

When the two declarations are compiled in otherwise identical schemas, migration planning
replaces the foreign key. The updated schema round-trips with the same hash and Cascade action.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Constraint tests](examples/compiled-schema-constraint-tests.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/CompiledSchemaConstraint.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/tests/CompiledSchemaConstraintTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlOnUpdateTests.cs`.
