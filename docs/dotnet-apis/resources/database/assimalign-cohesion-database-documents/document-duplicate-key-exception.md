# DocumentDuplicateKeyException

INSERT found an existing identity, or INSERT/UPSERT repeated a key within one statement.

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
code prefix. The public constructor describes an existing INSERT key. The engine also creates this
type for repeated keys in INSERT/UPSERT VALUES or SELECT, with the distinct message
`Document key 'a' appears more than once in one statement.` No new public constructor is added.
The failed statement's writes, versions and indexes roll back together. PutAsync retains
replacement/upsert behavior. The constructor adds no validation exception beyond its value
arguments. Over the wire, the failure becomes DocumentClientException with COHDOC012.

UNIQUE secondary-index failures use the catalog-owned
[DocumentConstraintViolationException](document-constraint-violation-exception.md):
COHDOC020 (23505) for duplicate valued scalar tuples or COHDOC021 for array/object components.

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

```csharp
try {
    await session.ExecuteAsync(
        "UPSERT INTO items (KEY, VALUE) VALUES ('a', 1), ('a', 2)",
        cancellationToken: CancellationToken.None);
} catch (DocumentDuplicateKeyException failure) {
    Console.WriteLine(failure.DocumentId.Value); // a
    Console.WriteLine(failure.Message);
    // Document key 'a' appears more than once in one statement.
}
```

[UPSERT](../../../../database/document/language/statements/upsert.md) permits a key already
in the destination, but each input key must be unique within that statement.

## Related reference

[Assembly](index.md) · [INSERT](../../../../database/document/language/statements/insert.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Exceptions/DocumentDuplicateKeyException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentInsertTests.cs`.
