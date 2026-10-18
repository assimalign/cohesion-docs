# DocumentConstraintViolationException

A UNIQUE secondary index refused a document write or index build.

Namespace: `Assimalign.Cohesion.Database.Documents.Catalog`.

## Signature

```csharp
public sealed class DocumentConstraintViolationException : DatabaseException
{
    public string IndexName { get; }
    public string? CollectionName { get; }
    public string? Path { get; }
    public string? SqlState { get; }
}
```

## Behavior and exceptions

There is no public constructor. Duplicate valued scalar keys carry COHDOC020, CollectionName
and SqlState `23505`, with Path null. An array/object UNIQUE component carries COHDOC021 and
Path, with CollectionName and SqlState null. Each has one Error diagnostic and a code-free
message. Duplicate messages name the index/collection without exposing key data.

Keys containing NULL or MISSING never conflict. Constraints are immediate per row. Failure rolls
back statement writes and aborts an explicit transaction until RollbackAsync. Wire callers receive
DocumentClientException with ExecutionFailure (6) and the diagnostic, preserving the connection.
[DocumentDuplicateKeyException](../assimalign-cohesion-database-documents/document-duplicate-key-exception.md)
instead describes document identities and uses COHDOC012.

## Example

With an items collection and `CREATE UNIQUE INDEX uq ON items (a, b)`:

```csharp
await session.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('one', {a:1,b:2.0})");
try {
    await session.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('two', {a:1.00,b:2})");
} catch (DocumentConstraintViolationException failure) {
    Console.WriteLine(failure.IndexName);            // uq
    Console.WriteLine(failure.CollectionName);       // items
    Console.WriteLine(failure.Diagnostics[0].Code);   // COHDOC020
    Console.WriteLine(failure.SqlState);             // 23505
}
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [CREATE INDEX](../../../../database/document/language/statements/create-index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Catalog/src/Exceptions/DocumentConstraintViolationException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.CompositeIndexes.cs`.
