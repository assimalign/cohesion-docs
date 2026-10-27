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

Typed index creation uses CD03025 for multiple array keys, UNIQUE array indexes or IncludeMissing
with a leading array key, and CD03026 for a definition above 8,092 encoded bytes before posting
extraction/tree creation. V1/v2 index records are refused before recovery writes with CD06105;
the supported version is 3. None has an assigned SQLSTATE. Typed WHEN argument guards retain
ArgumentException; see [predicate contracts](document-index-predicate.md).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/Exceptions/DocumentCatalogException.cs`.
