# CompiledSchemaIndex

Immutable compiled secondary-index metadata with an optional predicate and effective key orders.

Namespace: `Assimalign.Cohesion.Database.Sql.Schema`.

## Signature

```csharp
public sealed record CompiledSchemaIndex(
    string Name, IReadOnlyList<string> Columns, bool IsUnique = false,
    string? Predicate = null, IReadOnlyList<CompiledSchemaIndexKeyOrder>? KeyOrders = null);

public enum CompiledSchemaIndexKeyOrder
{
    AscendingNullsLast = 0,
    AscendingNullsFirst = 1,
    DescendingNullsLast = 2,
    DescendingNullsFirst = 3
}
```

## Behavior and exceptions

Columns and the effective KeyOrders property are copied into immutable collections. Omitting
KeyOrders supplies AscendingNullsLast for every key. Predicate is SQL retained as authored;
the engine parses and validates it before application. Owner is DatabaseObjectOwner.Schema.
The typed single-member Index builder remains unconditional and ascending; construct this
metadata to declare composite, partial or directed indexes.

Schema v11 serializes KeyOrders into canonical JSON, affecting the hash and migration equality.
An order change plans DropIndex/AddIndex, including for partial indexes. Invalid vector widths
or enum values fail schema validation with SqlSchemaValidationException, InvalidDocument at
`indexes.<name>.keyOrders`. Earlier artifacts, applied markers and pending targets are refused
at `schema.format`; no conversion reader is supplied.

## Example

Following CompiledIndexKeyOrderTests:

```csharp
CompiledSchemaIndexKeyOrder[] orders = [CompiledSchemaIndexKeyOrder.DescendingNullsLast];
var index = new CompiledSchemaIndex("ix", ["a"], Predicate: "a > 0", KeyOrders: orders);
orders[0] = CompiledSchemaIndexKeyOrder.AscendingNullsLast;
Console.WriteLine(index.KeyOrders[0]); // DescendingNullsLast
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Format and rebuild](../../../../database/sql/index.md#sql-format-32-and-schema-v11)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/CompiledSchemaIndex.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/src/CompiledSchemaIndexKeyOrder.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Schema/tests/CompiledIndexKeyOrderTests.cs`.
