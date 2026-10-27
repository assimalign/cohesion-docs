# DocumentIndexMetadata

An immutable index definition with ordered scalar/array keys, UNIQUE and IncludeMissing flags.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Documents.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public readonly record struct DocumentIndexMetadata
{
    public DocumentIndexMetadata(Guid collectionId, string name, string path, ulong objectId);
    public DocumentIndexMetadata(Guid collectionId, string name, IReadOnlyList<string> paths,
        bool isUnique, ulong objectId);
    public DocumentIndexMetadata(Guid collectionId, string name, IReadOnlyList<DocumentIndexKey> keys,
        bool isUnique, bool includeMissing, ulong objectId);
    public Guid CollectionId { get; }
    public string Name { get; }
    public string Path { get; }
    public IReadOnlyList<string> Paths { get; }
    public IReadOnlyList<DocumentIndexKey> Keys { get; }
    public bool IsUnique { get; }
    public bool IncludeMissing { get; }
    public ulong ObjectId { get; }
    public bool Equals(DocumentIndexMetadata other);
    public override int GetHashCode();
}
```

## Behavior and exceptions

The single-path overload creates a nonunique sparse definition. Both scalar-path overloads validate
path syntax and throw ArgumentException for invalid/null/empty scalar paths. The list overload
rejects a null list with ArgumentNullException and a count outside 1..32 with
ArgumentOutOfRangeException. They set IncludeMissing to false.

The typed overload snapshots one to 32 [DocumentIndexKey](document-index-key.md) descriptors into
a read-only Keys list. A null list/key throws ArgumentNullException; invalid count throws
ArgumentOutOfRangeException. Multiple array keys, UNIQUE with an array key, or IncludeMissing
with a leading array key throw DocumentCatalogException/CD03025 (no SQLSTATE). One array key may
appear at any position. IncludeMissing includes a MISSING leading scalar key; it does not invent
postings for an empty or fully filtered array.

For scalar-only definitions, Paths contains the ordered scalar paths and Path is the first.
For **any array index**, Paths is empty and Path is the empty string, even with a leading scalar
key. Array-aware callers must use Keys. A default struct has empty Path and null Paths/Keys.
Equality and hashing include ordered key values (including WHEN), flags and all other definition
fields. Construction writes no storage; the catalog build checks the 8,092-byte definition limit
with CD03026 before posting extraction or tree creation.

Every persisted index uses **v3** definition/registration records. V1/v2 are refused at open before
recovery writes with CD06105 and a recreate remedy; there is no upgrade path. See [Design](catalog-design.md).

## Example

```csharp
string[] paths = ["a", "b[0]", "['x.y']"];
var metadata = new DocumentIndexMetadata(Guid.NewGuid(), "uq", paths, true, 1);
paths[0] = "changed";
Console.WriteLine(metadata.Path);        // a
Console.WriteLine(metadata.Paths.Count); // 3
Console.WriteLine(metadata.IsUnique);    // True
```

The typed array form, adapted from DocumentCatalogTests.Arrays:

```csharp
var when = DocumentIndexPredicate.And(
    DocumentIndexPredicate.Compare("price", DocumentIndexComparison.GreaterThan, 10),
    DocumentIndexPredicate.Compare("stock", DocumentIndexComparison.GreaterThan, 0));
var array = new DocumentIndexMetadata(Guid.NewGuid(), "prices",
    [DocumentIndexKey.DistinctArray("items", "price", when)], false, false, 1);
Console.WriteLine(array.Keys[0].ArraySource); // items
Console.WriteLine(array.Keys[0].Path);        // price
Console.WriteLine(array.Paths.Count);        // 0
Console.WriteLine(array.Path.Length);        // 0
```

## Related reference

[Assembly](index.md) · [DocumentCatalog](document-catalog.md) · [CREATE INDEX](../../../../database/document/language/statements/create-index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/ValueObjects/DocumentIndexMetadata.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Catalog/DocumentCatalogTests.Format.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Catalog/DocumentCatalogTests.Arrays.cs` and `DocumentIndexDocumentationTests.cs`.
