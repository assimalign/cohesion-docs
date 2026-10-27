# DocumentCatalog

The sealed, transaction-bound metadata catalog; index operations build and maintain shared B+Trees.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

Assembly: `Assimalign.Cohesion.Database.Documents.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents`.

## Index signatures

```csharp
public sealed partial class DocumentCatalog
{
    public static DocumentCatalog Open(DocumentStorage storage, TransactionCoordinator coordinator);
    public ValueTask<DocumentIndexMetadata> CreateIndexAsync(Guid collectionId, string name,
        string path, TransactionContext context, CancellationToken cancellationToken = default);
    public ValueTask<DocumentIndexMetadata> CreateIndexAsync(Guid collectionId, string name,
        IReadOnlyList<string> paths, bool isUnique, TransactionContext context,
        CancellationToken cancellationToken = default);
    public ValueTask<DocumentIndexMetadata> CreateIndexAsync(Guid collectionId, string name,
        IReadOnlyList<DocumentIndexKey> keys, bool isUnique, bool includeMissing,
        TransactionContext context, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<DocumentCatalogEntry>> SearchIndexAsync(Guid collectionId,
        string indexName, object? lower, bool includeLower, object? upper, bool includeUpper,
        TransactionSnapshot snapshot, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<DocumentCatalogEntry>> SearchIndexAsync(Guid collectionId,
        string indexName, IReadOnlyList<object> equalityPrefix, object? lower, bool includeLower,
        object? upper, bool includeUpper, TransactionSnapshot snapshot,
        CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

Open loads metadata after recovery; callers must check index formats before recovery writes as
described in [Design](catalog-design.md). Mutations join the supplied transaction without committing it;
the caller holds model locks, resolves conflicts and enforces ownership. Applications ordinarily
use OQL index DDL through a session.

The typed overload accepts [scalar/array keys](document-index-key.md) and IncludeMissing.
It snapshots the key list and builds deduplicated per-document postings immediately. Array-aware
callers use metadata.Keys; metadata.Paths is empty for an array index. Search returns each visible
document ID once even if several postings match. OQL array-index syntax/access planning are not
delivered in this step. The caller owns the active transaction and database writer lock.

CreateIndexAsync builds immediately. The single-path overload is nonunique. Composite creation
copies one to 32 paths and checks UNIQUE constraints before publishing a definition. NULL/MISSING
keys never conflict. Search accepts a leading scalar equality prefix and optional range on the
next path; null bounds are unbounded. It returns visible candidates in ordinal document-ID order;
the caller reapplies the full predicate.

Null required arguments throw ArgumentNullException; invalid names/paths/bounds throw
ArgumentException. An inactive context throws InvalidOperationException. Invalid build path count
uses DatabaseException/CD03022 (no SQLSTATE). Duplicate UNIQUE keys use
[DocumentConstraintViolationException](document-constraint-violation-exception.md)/CD03020 (23505);
array/object UNIQUE keys use CD02021 (no SQLSTATE). Missing definitions/collections/trees,
unsupported or malformed catalog records and oversized keys use DocumentCatalogException.
Open may throw IndexFormatException for an incompatible tree. Storage I/O/corruption failures
propagate from seeks. Cancellation throws OperationCanceledException; an ended build transaction
throws TransactionAbortedException.

Multiple array keys, UNIQUE array indexes or IncludeMissing with a leading array key use
DocumentCatalogException/CD03025. An encoded definition above 8,092 bytes uses CD03026 before
posting extraction or physical tree creation. Neither has an assigned SQLSTATE. Unsupported
typed WHEN constants and oversized predicate trees retain [ArgumentException](document-index-predicate.md).
Index definition/registration v1/v2 are refused before recovery writes with CD06105; v3 is required.

## Example

Inside the catalog test fixture, with collection `Collection`, a caller-owned writer transaction
and its database writer lock:

```csharp
var index = await catalog.CreateIndexAsync(collection.Id, "mixed", ["a", "b"], false, writer);
var candidates = await catalog.SearchIndexAsync(collection.Id, index.Name,
    [1m], 2m, true, 3m, true, writer.Snapshot);
```

Candidates have leading a = 1 and next component between 2 and 3, in document-ID order.
Apply the residual predicate to the returned documents. The caller commits or rolls back writer.

## Related reference

[Assembly](index.md) · [DocumentIndexMetadata](document-index-metadata.md) · [Examples](examples/catalog.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/DocumentCatalog.Indexes.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Catalog/DocumentCatalogTests.Format.cs`.
