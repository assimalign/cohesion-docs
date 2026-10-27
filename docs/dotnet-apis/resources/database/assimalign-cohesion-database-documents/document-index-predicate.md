# DocumentIndexPredicate

An immutable bounded WHEN predicate over paths relative to a single array element.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

## Signature

```csharp
public sealed record DocumentIndexPredicate
{
    public DocumentIndexPredicateKind Operation { get; }
    public string? Path { get; }
    public DocumentIndexComparison Comparison { get; }
    public object? Value { get; }
    public DocumentIndexPredicate? Left { get; }
    public DocumentIndexPredicate? Right { get; }
    public static DocumentIndexPredicate Compare(string path, DocumentIndexComparison comparison, object? value);
    public static DocumentIndexPredicate IsNull(string path);
    public static DocumentIndexPredicate IsMissing(string path);
    public static DocumentIndexPredicate And(DocumentIndexPredicate left, DocumentIndexPredicate right);
    public static DocumentIndexPredicate Or(DocumentIndexPredicate left, DocumentIndexPredicate right);
    public static DocumentIndexPredicate Not(DocumentIndexPredicate operand);
}

public enum DocumentIndexComparison : byte
{
    Equal, NotEqual, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual
}
public enum DocumentIndexPredicateKind : byte
{
    Comparison = 1, IsNull, IsMissing, And, Or, Not
}
```

## Behavior and exceptions

Path is element-relative; the empty string means the element itself. Compare supports a path
against null, Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Decimal or String.
Integers normalize to Decimal. **Single and Double throw ArgumentException**, even for finite,
exactly representable values. Other unsupported constant types also throw ArgumentException.
There are no function/callback, parameter, subquery, nested-comprehension, document-relative
operand or arbitrary engine-AST forms in this typed predicate API.

Invalid paths throw ArgumentException; null paths/operands throw ArgumentNullException.
An unknown comparison enum throws ArgumentOutOfRangeException. And, Or and Not throw
ArgumentException if the result exceeds **32 levels or 128 nodes**, counting a repeated subtree
at every occurrence. These typed argument guards have no diagnostic code or SQLSTATE.

Only TRUE admits an element. Comparisons with NULL/MISSING are unknown; explicit IsNull and
IsMissing distinguish them (IsNull on MISSING is unknown). Valued mismatched kinds are unequal
and unordered. Strings compare ordinally; numeric values compare as Decimal. And/Or/Not preserve
unknown logic. Logical nodes have null Path; Left is the sole operand of Not. Value is a
normalized immutable comparison constant. These records persist as data, without engine callbacks.

## Example

From DocumentCatalogTests.Arrays' logical predicate round trip:

```csharp
var when = DocumentIndexPredicate.Or(DocumentIndexPredicate.IsMissing("p"),
    DocumentIndexPredicate.And(DocumentIndexPredicate.Not(DocumentIndexPredicate.IsNull("p")),
        DocumentIndexPredicate.Compare("p", DocumentIndexComparison.GreaterThan, "w")));
var key = DocumentIndexKey.DistinctArray("items", "q", when);
```

For `items:[{p:null,q:1},{q:2},{p:false,q:3},{p:'x',q:4},{p:'y',q:5}]`, this descriptor posts
q values **2, 4 and 5**. The explicit NULL and the Boolean value do not qualify. Creating
`DocumentIndexPredicate.Compare("p", DocumentIndexComparison.Equal, 1.0d)` throws ArgumentException.

## Related reference

[Assembly](index.md) · [DocumentIndexKey](document-index-key.md) · [DocumentIndexMetadata](document-index-metadata.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Catalog/ValueObjects/DocumentIndexPredicate.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Catalog/DocumentCatalogTests.Arrays.cs` and `DocumentIndexDocumentationTests.cs`.
