# GraphFunctionType

Immutable scalar, entity and recursive container shape metadata.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphFunctionType
{
    public GraphFunctionValueKind ValueKind { get; }
    public DatabaseType ScalarType { get; }
    public GraphFunctionType? Element { get; }
    public bool IsNullable { get; }
    public static GraphFunctionType Any { get; }
    public static GraphFunctionType Number { get; }
    public static GraphFunctionType Node { get; }
    public static GraphFunctionType Relationship { get; }
    public static GraphFunctionType Path { get; }
    public static GraphFunctionType String { get; }
    public static GraphFunctionType Int64 { get; }
    public static GraphFunctionType Float64 { get; }
    public static GraphFunctionType Boolean { get; }
    public static GraphFunctionType List(GraphFunctionType element);
    public static GraphFunctionType Map(GraphFunctionType value);
    public static GraphFunctionType Of<T>();
    public GraphFunctionType NotNull();
}
```

## Behavior and exceptions

Of<T> accepts object, GraphNode, GraphRelationship, GraphPath, object[],
IReadOnlyDictionary<string, object?>, string, bool, long, double, decimal, DateOnly and
DateTimeOffset, including nullable value types. Unsupported CLR types throw ArgumentException.
Factories produce nullable shapes; NotNull returns a new nonnullable top-level shape. Element
holds list element/map value metadata. ScalarType is Null for unions, entities and composites.

List/Map reject null metadata with ArgumentNullException and excessive codec nesting with
ArgumentException. Binding uses declared shapes for scalar/entity/composite admission even
for empty results. Runtime results must honor nested shapes and contain finite numeric leaves;
violations use CG05021. Strict scalar invocation may make the top result nullable without
changing nested nullability.

## Example

```csharp
var shape = GraphFunctionType.List(GraphFunctionType.Node.NotNull());
Console.WriteLine(shape.ValueKind); // List
Console.WriteLine(shape.Element!.IsNullable); // False
```

## Related reference

[Assembly](index.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunctionType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
