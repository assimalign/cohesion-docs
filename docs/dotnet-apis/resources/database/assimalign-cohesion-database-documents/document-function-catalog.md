# DocumentFunctionCatalog

Frozen engine-local registrations visible to all engine sessions.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentFunctionCatalog : IReadOnlyCollection<DocumentFunction>
{
    public int Count { get; }
    public IReadOnlyList<DocumentFunction> GetOverloads(DocumentFunctionName name);
    public IEnumerator<DocumentFunction> GetEnumerator();
}
```

## Behavior and exceptions

The engine exposes Functions after build. Lookup compares ordinally ignoring case and returns
immutable overloads in registration order, or an empty list, never null. Count includes overloads, including native registrations.
There is no public constructor or mutation. Default/invalid lookup names throw ArgumentException.

The identifier type lives in Assimalign.Cohesion.Database.Documents.

## Example

```csharp
var builder = DocumentDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(DocumentScalarFunction.Create<string, string>("slug", value => value.ToLowerInvariant()));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("SLUG").Count); // 1
```

## Related reference

[Assembly](index.md) · [DocumentFunctionCollection](document-function-collection.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunctionCatalog.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
