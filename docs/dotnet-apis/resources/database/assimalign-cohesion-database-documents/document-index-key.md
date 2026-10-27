# DocumentIndexKey

A normalized scalar path or DISTINCT array-element projection for a typed catalog index.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

## Signature

```csharp
public sealed record DocumentIndexKey
{
    public string Path { get; }
    public string? ArraySource { get; }
    public DocumentIndexPredicate? When { get; }
    public static DocumentIndexKey Scalar(string path);
    public static DocumentIndexKey DistinctArray(string sourcePath, string elementPath = "",
        DocumentIndexPredicate? when = null);
}
```

## Behavior and exceptions

Scalar uses a document-relative path and leaves ArraySource/When null. DistinctArray represents
`DISTINCT ARRAY v.p FOR v IN arr [WHEN cond] END`: sourcePath names arr in the document, Path is
the element-relative projection p, and When is a normalized element-relative predicate. Variable
names are absent from the descriptor. An empty elementPath projects the element itself. Paths
use lossless dotted/bracket syntax, including `items[0]` and `['x.y']`.

Factories validate paths; invalid paths throw ArgumentException. A null elementPath throws
ArgumentNullException. [Metadata](document-index-metadata.md) permits at most one array key,
at any compound-key position, and refuses UNIQUE array indexes or IncludeMissing with a leading
array key (DocumentCatalogException/CD03025).

### Posting semantics

For each document version, the index stores one posting per distinct qualifying encoded scalar
projection, combined with the other scalar document keys. Numeric scales deduplicate (1 and
1.00 are equal). WHEN admits only TRUE and tests all its conjuncts on the **same element**.
NULL projections are posted; MISSING, array and object projections are omitted. Empty or fully
filtered arrays produce no postings. A missing source gives a MISSING component, omitted when
leading and sparse; a present non-array source gives NULL, following ARRAY source semantics.
IncludeMissing on a leading scalar key does not create elements for an empty/filtered array.

Postings reference document versions, not element identities. Search deduplicates document IDs;
UNNEST still needs the original array to preserve element multiplicity. Build, replacement and
deletion maintain postings transactionally. OQL array-index DDL and access planning arrive in
batch 6; this step exposes the typed catalog API only.

## Example

From DocumentCatalogTests.Arrays, with an active caller-owned `writer` and the database writer
lock held (the [catalog contract](document-catalog.md) describes this lower-level context):

```csharp
var when = DocumentIndexPredicate.And(
    DocumentIndexPredicate.Compare("price", DocumentIndexComparison.GreaterThan, 10),
    DocumentIndexPredicate.Compare("stock", DocumentIndexComparison.GreaterThan, 0));
var index = await catalog.CreateIndexAsync(collection.Id, "prices",
    [DocumentIndexKey.DistinctArray("items", "price", when)], false, false, writer);
var candidates = await catalog.SearchIndexAsync(collection.Id, index.Name,
    10m, false, null, false, writer.Snapshot);
```

Given `cross` with `items:[{price:20,stock:0},{price:1,stock:5}]` and `same` with
`items:[{price:20,stock:5},{price:20.0,stock:8},{stock:9}]`, the build writes one value posting,
20 for `same`. Candidates contains only `same`; `cross` has no element satisfying both tests.
The caller commits or rolls back writer. Reapply the full query predicate to candidates.

## Related reference

[Assembly](index.md) · [DocumentIndexMetadata](document-index-metadata.md) · [DocumentIndexPredicate](document-index-predicate.md) · [Catalog design](catalog-design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/ValueObjects/DocumentIndexKey.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Catalog/DocumentIndexKeys.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Catalog/DocumentCatalogTests.Arrays.cs`.
