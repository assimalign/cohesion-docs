# DocumentFunctionCollection

Builder-owned function registrations, mutable before construction begins.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentFunctionCollection : IReadOnlyCollection<DocumentFunction>
{
    public int Count { get; }
    public DocumentFunctionCollection Add(DocumentFunction function);
    public bool Contains(DocumentFunctionName name);
    public IEnumerator<DocumentFunction> GetEnumerator();
}
```

## Behavior and exceptions

Get this collection from the engine builder's Functions property; it has no public constructor.
Add returns the collection. Build freezes registration before storage/workers, including after
a failed build attempt. Later Add or duplicate signatures throw InvalidOperationException.
Null functions throw ArgumentNullException; reserved names throw ArgumentException.
Contains refuses a default name with ArgumentException.
Native, grammar, keyword and reserved nonexecuting call names are reserved. Application
overloads require disjoint arity ranges and one scalar/aggregate kind. The collection is not
thread-safe; one registered function instance must be stateless or thread-safe.

## Example

```csharp
var builder = DocumentDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(DocumentScalarFunction.Create<string, string>("slug", value => value.ToLowerInvariant()));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("SLUG").Count); // 1
```

## Related reference

[Assembly](index.md) · [DocumentFunctionCatalog](document-function-catalog.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunctionCollection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
