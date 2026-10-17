# DocumentDuplicateKeyException

An insert-only write found a document identity already visible in its transaction.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentDuplicateKeyException : DatabaseException
{
    public DocumentDuplicateKeyException(string collectionName, DocumentId documentId);
    public string CollectionName { get; }
    public DocumentId DocumentId { get; }
}
```

## Behavior and exceptions

Carries one Error diagnostic, COHDOC012, with the collection and duplicate identity. Message has no
code prefix. INSERT uses this type for existing keys, repeated keys within VALUES and a self-copy.
The failed statement's writes, versions and indexes roll back together. PutAsync retains
replacement/upsert behavior. The constructor adds no validation exception beyond its value
arguments. Over the wire, the failure becomes DocumentClientException with COHDOC012.

## Example

After creating items in a Documents session:

```csharp
await session.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('a', 1)",
    cancellationToken: CancellationToken.None);
try {
    await session.ExecuteAsync("INSERT INTO items (KEY, VALUE) VALUES ('a', 2)",
        cancellationToken: CancellationToken.None);
} catch (DocumentDuplicateKeyException failure) {
    Console.WriteLine(failure.CollectionName);      // items
    Console.WriteLine(failure.Diagnostics[0].Code); // COHDOC012
}
```

## Related reference

[Assembly](index.md) · [INSERT](../../../../database/document/language/statements/insert.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Exceptions/DocumentDuplicateKeyException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
