# DocumentConnection

An authenticated database-bound rental supporting one active operation at a time.

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentConnection : IAsyncDisposable
{
    public string Database { get; }
    public bool IsOpen { get; }
    public ValueTask<DocumentResultSet> QueryAsync(string statement,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<long> ExecuteAsync(string statement,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<Document?> GetAsync(string collection, DocumentId id, CancellationToken cancellationToken = default);
    public ValueTask<Document> PutAsync(string collection, DocumentId id, ReadOnlyMemory<byte> content,
        DocumentVersion? expectedVersion = null, CancellationToken cancellationToken = default);
    public ValueTask<bool> DeleteAsync(string collection, DocumentId id,
        DocumentVersion? expectedVersion = null, CancellationToken cancellationToken = default);
    public ValueTask<string> CreateCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<string> GetCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask DropCollectionAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<string>> GetCollectionsAsync(CancellationToken cancellationToken = default);
    public ValueTask AbortAsync();
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

QueryAsync materializes rows or whole documents; ExecuteAsync returns the affected count (-1 for
queries, zero for DDL and transaction controls, written count for INSERT/UPSERT, deleted count for DELETE, matched count for UPDATE). Parameters follow case-sensitive bare-name
binding and accept JSON scalars/JsonElement values. Requests snapshot parameter/content bytes before
their first await. Get returns null when absent; Put replaces/upserts and returns a new version;
Delete returns whether a document existed. Expected versions must match visible content. Collection
operations return names; lists use ordinal name order. Text controls manage the connection's transaction.

Blank statement/names or invalid parameters throw ArgumentException; invalid wire JSON can throw
ProtocolException. Disposed rentals throw ObjectDisposedException, concurrent exchanges
InvalidOperationException and cancellation OperationCanceledException. An unfinished exchange is
discarded; a canceled mutation may already have committed. A fully decoded model Failure raises
DocumentClientException and permits reuse, even if partial results preceded it; partial results are
discarded. Malformed traffic and shared Error discard the connection. Dispose returns a healthy
rental exactly once; Abort discards it. Repeated disposal cannot return a later rental.

The wrapper caches Database at rental time, so that name remains readable after disposal; IsOpen
returns false. Its shared DatabaseConnection lease permanently rejects execution after return.
SQL, Graph and Documents negotiate server-reported transaction status and retained session state. Pool return rolls back active/failed work, then runs the family reset only if Ready still reports retained state (SQL uses DISCARD ALL). Reuse requires Idle and no retained state. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

## Text transactions

ExecuteAsync accepts [BEGIN/START, COMMIT and ROLLBACK](../../../../database/document/language/statements/transactions.md)
and returns zero for each successful control. Use the same connection throughout. Snapshot is
the default; `BEGIN ISOLATION LEVEL READ COMMITTED` refreshes the committed snapshot for each
statement. QueryAsync, OQL writes and typed CRUD share the transaction and see its own writes.

A server statement failure faults the transaction. DocumentClientException retains the original
diagnostic; subsequent work reports CD04101. ROLLBACK discards pending writes and recovers the
session. COMMIT on a failed transaction reports CD04101 and ends it without committing. OQL
collection/index DDL inside it reports CD04105. A current client negotiates transaction-status;
CD07002 is the server refusal for controls from a peer without that agreement.

DisposeAsync joins the last operation and rolls back active or failed work before returning the
session to the pool. Reset must complete with both counts zero and Idle status. Invalid or
missing status, failed reset, unfinished cancellation, or an unnegotiated peer causes discard.

After COMMIT's Complete acknowledgement, status cleanup ignores caller cancellation and uses a
five-second internal bound. If it succeeds, ExecuteAsync returns success even if the caller's
token was canceled during cleanup. If COMMIT throws because its completion or Ready is lost,
the outcome is unknown to the caller and the connection is discarded. A missing Ready can raise
OperationCanceledException from that internal bound independently of the caller's token; closure
before Ready raises DocumentClientException with ProtocolViolation. Cleanup never retries COMMIT
or promises to undo committed work.

With `items` already created, the wire transaction tests exercise this sequence:

```csharp
await connection.ExecuteAsync("BEGIN");
await connection.PutAsync("items", "pending", "{}"u8.ToArray());
Console.WriteLine((await connection.GetAsync("items", "pending")).HasValue); // True
await connection.ExecuteAsync("ROLLBACK");
Console.WriteLine((await connection.GetAsync("items", "pending")).HasValue); // False
```

## Example

```csharp
await connection.CreateCollectionAsync("items");
var saved = await connection.PutAsync("items", "one", "{\"amount\":2}"u8.ToArray());
var read = await connection.GetAsync("items", "one");
Console.WriteLine(read!.Value.Version == saved.Version); // True
bool deleted = await connection.DeleteAsync("items", "one", saved.Version);
Console.WriteLine(deleted); // True
```

connection is rented from [DocumentClient](document-client.md); DocumentId has an implicit string conversion.

After putting `{"amount":2}` under key one, this partial update affects one document:

```csharp
await connection.PutAsync("items", "one", "{\"amount\":2}"u8.ToArray());
long matched = await connection.ExecuteAsync("UPDATE items SET amount = amount + 1");
Console.WriteLine(matched); // 1; stored amount is 3
```

Without RETURNING, UPDATE emits no result rows. Invalid path/overlap/stored-JSON results raise DocumentClientException
with CD02016/CD01017/CD02018, respectively, and a completed Failure permits reuse. See
[UPDATE](../../../../database/document/language/statements/update.md) for NULL/MISSING and version rules.

ExecuteAsync accepts [UPSERT](../../../../database/document/language/statements/upsert.md) and returns
the number of replaced/inserted pairs; it drains optional RETURNING rows. CD03012 rejects repeated input keys; decoded
Failure preserves the rental. Generated UTF-8 JSON bytes agree with in-process and InMemory results.

## Related reference

QueryAsync returns [RETURNING](../../../../database/document/language/clauses/returning.md) rows and
their affected count; ExecuteAsync drains rows and returns that count. There is no capability flag.
An older client rejects the response after autocommit has committed, so retrying reapplies the write;
an explicit session's closure rolls back uncommitted work. See [client compatibility](design.md#returning-and-client-compatibility).

[Results](document-result-set.md) · [Exceptions](document-client-exception.md) · [Design](design.md)

## Queued terminal errors

Before the next admitted operation writes a request, the shared connector checks buffered input or
a closed transport and consumes a queued terminal Error once. Its original code reaches
DocumentClientException and the connector is discarded. This also applies to streaming startup;
IsOpen continues to describe physical transport health. Unexpected/malformed terminal frames use
ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are not implemented.
A clean handshake EOF has its own message naming the startup handshake. See
[shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors)
and [terminal delivery/TLS limits](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Sources

- **Terminal client conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionTextWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionCancellationTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DocumentTransactionResetTests.cs`.
