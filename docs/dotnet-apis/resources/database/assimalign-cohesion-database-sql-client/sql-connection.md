# SqlConnection

A typed SQL rental over one authenticated pooled session.

Namespace: `Assimalign.Cohesion.Database.Sql.Client`.

## Signature

```csharp
public sealed class SqlConnection : IAsyncDisposable
{
    public string Database { get; }
    public bool IsOpen { get; }
    public ValueTask<SqlResultSet> QueryAsync(SqlCommand command, CancellationToken cancellationToken = default);
    public ValueTask<SqlResultSet> QueryAsync(string commandText,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<long> ExecuteAsync(SqlCommand command, CancellationToken cancellationToken = default);
    public ValueTask<long> ExecuteAsync(string commandText,
        IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
    public ValueTask<T?> ExecuteScalarAsync<T>(SqlCommand command, CancellationToken cancellationToken = default);
    public ValueTask<long> CopyToAsync(string statement, Stream destination,
        CancellationToken cancellationToken = default);
    public ValueTask AbortAsync();
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

SqlClient.ConnectAsync rents the connection. QueryAsync materializes all rows; ExecuteAsync returns
the affected count, buffering and discarding RETURNING rows. ExecuteScalarAsync returns the first
cell. Only one command runs at a time. Null commands throw ArgumentNullException; blank text throws
ArgumentException, concurrent exchanges InvalidOperationException, and cancellation
OperationCanceledException. Server/transport failures use SqlClientException; fully decoded
statement rejections permit reuse while unfinished exchanges discard the session.

The shared pool reuses an internal connector behind a new lease on every rent. Disposal is final:
later commands with valid arguments throw ObjectDisposedException and repeated disposal/abort cannot
reach another renter. Database is cached and remains readable; IsOpen returns false after disposal.
The core DatabaseConnection's repeated disposal joins the original return; the typed wrapper's
repeated disposal is a no-op. AbortAsync on a live wrapper discards its session without cancellation.

SQL, Graph and Documents negotiate server-reported transaction status and retained session state. Pool return rolls back active/failed work, then runs the family reset only if Ready still reports retained state (SQL uses DISCARD ALL). Reuse requires Idle and no retained state. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

SQL uses ordinary BEGIN/COMMIT/ROLLBACK commands on the same connection. If cleanup fails, abort
the live rental. Abort cannot establish the outcome of an unacknowledged COMMIT or undo published writes.

Ready also reports retained state. After rollback, pool return sends DISCARD ALL only when that
flag remains set, clearing currval/lastval observations and restoring the timeout values configured
before the server session's first statement. Clean rentals add no reset exchange. Session state
does not pass to a later renter. See [sequence functions](../../../../database/sql/language/functions/sequences.md)
and [pool reset](../assimalign-cohesion-database-client/design.md#transaction-status-and-pool-reset).

An idle open wire transaction retains its table-intent, row and unique-key locks until COMMIT,
ROLLBACK, disconnect, pool return or server `IdleTimeout` (30 minutes by default), blocking
conflicting writers. New clients discard sessions against pre-#1540 servers without negotiated
transaction-status rather than
pooling them. The connector consumes Ready status automatically after every Execute; caller
cancellation after a completed result preserves an acknowledged committed result.

## CopyToAsync

Exports a table in [text or CSV](../../../../database/sql/language/statements/copy-to.md) and returns
the verified data-row count, excluding a header. The destination remains open on every outcome.
The peer must negotiate [SqlProtocol.OutboundStream](../assimalign-cohesion-database-sql-protocol/sql-protocol.md#outboundstream).
Missing capability refuses locally with SqlClientException/CS05096; ordinary Execute/Query also
refuses COPY with CS05096. A non-COPY statement supplied to this API uses CS05095. An explicit
transaction refuses COPY with CS04075 (25001).

Blank statement or a nonwritable destination throws ArgumentException; null destination throws
ArgumentNullException, disposed/aborted connection ObjectDisposedException, overlapping command
InvalidOperationException, and cancellation OperationCanceledException. Server/protocol failures
use SqlClientException. A pre-canceled call sends nothing. A coded statement Error before CopyStart
preserves the connection and transaction. Errors after CopyStart retain diagnostics but set
[ConnectionUsable](sql-client-exception.md) false and discard the rental. Cancellation during the
exchange, malformed/incomplete responses, disconnect and destination failures discard it too.
The destination's original exception is rethrown after cleanup and can leave partial output.

One snapshot spans binding, scanning and delivery, with no writer/row/object locks. Data frames
are at most 16 KiB, with one outstanding frame acknowledged only after its destination write.
Destinations must honor cancellation and must not reenter the connection from write callbacks.
The client drains the negotiated Ready postamble before returning success.

SqlCopyToWireTests checks this against the users fixture over InMemory and TCP:

```csharp
using var output = new MemoryStream();
long rows = await connection.CopyToAsync(
    "COPY users(name,id) TO STDOUT (FORMAT csv, HEADER, FORCE_QUOTE *)", output,
    cancellationToken);
Console.WriteLine(rows); // 2
Console.Write(Encoding.UTF8.GetString(output.ToArray()));
// name,id\n"ada","1"\n"grace","2"\n
```

Row order is unspecified; this is the test fixture's observed output.

## Example

With a connected client and the users fixture from SqlClientTests:

```csharp
await using var connection = await client.ConnectAsync();
SqlResultSet rows = await connection.QueryAsync("SELECT id, name FROM users ORDER BY id");
Console.WriteLine(rows[0].GetString("name")); // ada
Console.WriteLine(rows[1].GetString("name")); // grace
await connection.ExecuteAsync("BEGIN");
await connection.ExecuteAsync("ROLLBACK"); // finish before returning the lease
```

## Result failure example

Adapted from SqlResultDeliveryWireConformanceTests, using an open connection with diagnostics
negotiated. Each expression fits, but the combined second RETURNING row exceeds one frame:

```csharp
using System.Linq;

await connection.ExecuteAsync("CREATE TABLE result_sizes(id INT)");
await connection.ExecuteAsync("BEGIN");
await connection.ExecuteAsync("INSERT INTO result_sizes VALUES (9)");
string columns = string.Join(", ", Enumerable.Range(1, 34).Select(i =>
    $"CASE WHEN id = 3 THEN 'partial' ELSE RPAD('x', 524288, 'x') END AS c{i}"));
try
{
    await connection.QueryAsync(
        "INSERT INTO result_sizes VALUES (3), (4) RETURNING " + columns);
}
catch (SqlClientException failure)
{
    Console.WriteLine(failure.Diagnostics[0].Code); // CS05045
    Console.WriteLine(failure.ConnectionUsable);    // True
}
var remaining = await connection.QueryAsync("SELECT COUNT(*) FROM result_sizes");
Console.WriteLine(remaining[0].GetInt64(0)); // 1: earlier statement remains
await connection.ExecuteAsync("COMMIT");
```

The failed QueryAsync exposes no partial rows. The result refusal maps to SQLSTATE 54000.
Read-only result failures also throw and discard partial rows while allowing another command
on the same connection. See [wire results](../assimalign-cohesion-database-sql-protocol/design.md#statement-results).

## Joined mutations

QueryAsync executes [UPDATE FROM](../../../../database/sql/language/statements/update.md#from-sources)
and [DELETE USING](../../../../database/sql/language/statements/delete.md#using-sources), returning
rows and AffectedCount. ExecuteAsync returns that count after buffering and discarding rows.
SET/WHERE/RETURNING can read source columns, while SET destinations remain target-only.
UPDATE RETURNING sees new target values; DELETE sees old target values. Source values come
from pre-write collection. Bare star expands target columns then visible source columns;
duplicate names require ordinal access to distinguish them.

For a new database, adapted from SqlDmlSourceWireTests:

```csharp
await connection.ExecuteAsync("CREATE TABLE t (id INT PRIMARY KEY, v INT)");
await connection.ExecuteAsync("CREATE TABLE s (id INT, v INT)");
await connection.ExecuteAsync("INSERT INTO t VALUES (1,0), (2,0)");
await connection.ExecuteAsync("INSERT INTO s VALUES (1,10), (2,20), (2,20)");
try
{
    await connection.ExecuteAsync("UPDATE t SET v=s.v FROM s WHERE t.id=s.id");
}
catch (SqlClientException failure)
{
    Console.WriteLine(failure.Diagnostics[0].Code); // CS05068 (SQLSTATE mapping 21000)
    Console.WriteLine(failure.ConnectionUsable); // True; t still contains (1,0), (2,0)
}
Console.WriteLine(await connection.ExecuteAsync(
    "DELETE FROM t USING s WHERE t.id=s.id")); // 2; duplicate source matches delete each target once
```

Cardinality and constraint failures undo only the statement and preserve earlier transaction
work. Concurrent committed target changes use the existing first-updater-wins conflict.

## MERGE results and failures

[MERGE](../../../../database/sql/language/statements/merge.md) uses QueryAsync and ExecuteAsync
with the same signatures. QueryAsync returns direct action rows and the affected count, before
referential effects; a row subsequently cascaded away still returns and counts. A no-op returns
no rows and count 0. Constraint or late RETURNING failures preserve earlier explicit work and
set SqlClientException.ConnectionUsable to true. Wire BEGIN uses Snapshot; a concurrent committed
target change uses retryable CS04064 (40001), requiring ROLLBACK and a transaction retry.

Adapted from SqlMergeWireTests, with one existing row (id = 1):

```csharp
var noOp = await connection.QueryAsync(
    "MERGE INTO t USING (SELECT 99 AS id) AS s ON t.id=s.id " +
    "WHEN MATCHED THEN DO NOTHING WHEN NOT MATCHED THEN DO NOTHING " +
    "RETURNING merge_action()");
Console.WriteLine(noOp.Count); // 0
Console.WriteLine(noOp.AffectedCount); // 0
```

## Related reference

[SQL client](index.md) · [Design](design.md) · [Shared lease](../assimalign-cohesion-database-client/database-connection.md)

## Queued terminal errors

Before the next admitted operation writes a request, the shared connector checks buffered input or
a closed transport and consumes a queued terminal Error once. Its original code reaches
SqlClientException and the connector is discarded. This also applies to streaming startup;
IsOpen continues to describe physical transport health. Unexpected/malformed terminal frames use
ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are not implemented.
A clean handshake EOF has its own message naming the startup handshake. See
[shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors)
and [terminal delivery/TLS limits](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Sources

- **COPY** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlConnection.Copy.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlCopyToWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlDmlSourceWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlMergeWireTests.cs` and `SqlMergeConcurrencyWireTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlResultDeliveryWireConformanceTests.cs`.
- **Terminal client conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlClientTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseConnectionLeaseTests.cs`.
