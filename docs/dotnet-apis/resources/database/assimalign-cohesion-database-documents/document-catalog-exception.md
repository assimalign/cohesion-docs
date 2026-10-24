# DocumentCatalogException

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

[Model catalog](catalog-design.md) · [Assembly](index.md)

## Structured diagnostics

```csharp
public sealed class DocumentCatalogException : Exception
{
    public DocumentCatalogException(string message);
    public DocumentCatalogException(string message, Exception innerException);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
}
```

Diagnostics is non-null and read-only. Public message constructors leave it empty; model-owned coded producers populate it with the condition identity separately from the unprefixed message, retaining an original cause when present. Direct catalog consumers receive this exception; model boundaries translate failures into their DatabaseException contracts. Branch on complete diagnostic codes, not message text.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/Exceptions/DocumentCatalogException.cs`.
