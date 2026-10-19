# DocumentIndexMetadata

An immutable index definition with ordered paths and a UNIQUE flag.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

## Signature

```csharp
public readonly record struct DocumentIndexMetadata
{
    public DocumentIndexMetadata(Guid collectionId, string name, string path, ulong objectId);
    public DocumentIndexMetadata(Guid collectionId, string name, IReadOnlyList<string> paths,
        bool isUnique, ulong objectId);
    public Guid CollectionId { get; }
    public string Name { get; }
    public string Path { get; }
    public IReadOnlyList<string> Paths { get; }
    public bool IsUnique { get; }
    public ulong ObjectId { get; }
    public bool Equals(DocumentIndexMetadata other);
    public override int GetHashCode();
}
```

## Behavior and exceptions

The single-path overload creates a nonunique definition. The composite overload copies Paths to a
read-only snapshot; null throws ArgumentNullException, and a count outside 1..32 throws
ArgumentOutOfRangeException. Equality and hashing include path values in order, along with all
other definition fields. Path is the leading member; a default struct returns an empty Path
and has a null Paths reference. Construction does not write storage or validate path syntax.

Every persisted index uses v2 definition/registration records. V1 requires recreation with this
engine; there is no upgrade path. See [Design](design.md).

## Example

```csharp
string[] paths = ["a", "b[0]", "['x.y']"];
var metadata = new DocumentIndexMetadata(Guid.NewGuid(), "uq", paths, true, 1);
paths[0] = "changed";
Console.WriteLine(metadata.Path);        // a
Console.WriteLine(metadata.Paths.Count); // 3
Console.WriteLine(metadata.IsUnique);    // True
```

## Related reference

[Assembly](index.md) · [DocumentCatalog](document-catalog.md) · [CREATE INDEX](../../../../database/document/language/statements/create-index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/src/ValueObjects/DocumentIndexMetadata.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/tests/DocumentCatalogTests.Format.cs`.
