# Documents storage

`Assimalign.Cohesion.Database.Documents.Storage` stores immutable UTF-8 JSON content as chunk chains over `Database.Storage`. `Database.Transactions` supplies the shared logical coordinator, version stamps, undo ledger, and recovery classification.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.Documents.Storage`. Assembly: `Assimalign.Cohesion.Database.Documents.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Documents`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Reference pages

- **[Design](storage-design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/storage.md)** — Source-backed usage and expected behavior.

`Assimalign.Cohesion.Database.Documents.Storage` stores immutable UTF-8 JSON content as chunk chains
over `Database.Storage`. `Database.Transactions` supplies the shared logical coordinator, version
stamps, undo ledger, and recovery classification. The component has no dependency on the document
engine, Hosting, ApplicationModel, or a serializer that discovers runtime types.

`Create` a file set with `DocumentStorage.Create`; construct a `TransactionCoordinator` using
`WriteAheadJournal` and `Records`. `WriteContentAsync` validates a document, writes its chunks in
the supplied transaction, and returns a `DocumentContentReference`. Publish that reference through
Documents.Catalog before committing. `ReadContent` validates integrity and returns the original
bytes. JSON objects, arrays, and scalar roots are supported, including mixed document shapes.

## WriteContentAsync

```csharp
public ValueTask<DocumentContentReference> WriteContentAsync(
    TransactionCoordinator coordinator, TransactionContext context,
    ReadOnlyMemory<byte> content, CancellationToken cancellationToken = default);
```

The method borrows the caller's active context and never commits or rolls it back, including
after a chunk failure or cancellation. Earlier chunks remain tracked until the caller aborts.
Invalid JSON or a number outside the stored decimal domain throws `JsonException`; cancellation
throws `OperationCanceledException`. Chunk storage failures propagate to the caller. Engine
callers route them through the engine's ordered failure/end handler; standalone callers must
abort their context to undo chunks already written.

With an open storage and its coordinator, this adapts DocumentContentTests:

```csharp
var context = await coordinator.BeginAsync(IsolationLevel.Snapshot);
try
{
    await storage.WriteContentAsync(coordinator, context, "{"u8.ToArray());
}
catch (System.Text.Json.JsonException)
{
    Console.WriteLine(context.State); // Active; validation wrote no chunks
    await coordinator.RollbackAsync(context, CancellationToken.None);
    Console.WriteLine(context.State); // RolledBack
}
```

## Streaming and limits

The streaming chunk helpers follow the Blob storage precedent. They transport bytes and require
their caller to validate document syntax before publication; the engine uses the validated memory
helper. Documents are currently materialized for query evaluation and limited to `Int32.MaxValue`
content bytes. This is a format/implementation boundary rather than an eager allocation promise.

The original `InsertDocument`, `ReadDocument`, `UpdateDocument`, and `DeleteDocument` methods are
retained low-level, unstamped record helpers for existing consumers. They are not the engine's
document write path and must not be mixed into engine-owned file sets. See [DESIGN.md](storage-design.md)
for the exact format, compatibility policy, and recovery order.


[Parent: Assimalign.Cohesion.Database.Documents](index.md)

## Sources

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/Storage/DocumentContentTests.cs`.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Assimalign.Cohesion.Database.Documents.csproj`.
