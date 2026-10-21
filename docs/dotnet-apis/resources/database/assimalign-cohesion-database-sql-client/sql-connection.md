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

> **Known gap (#1540):** End or roll back an explicit transaction before disposing its connection.
> A healthy pooled session retains unfinished transaction state, which a later renter can inherit.

SQL uses ordinary BEGIN/COMMIT/ROLLBACK commands on the same connection. If cleanup fails, abort
the live rental. Abort cannot establish the outcome of an unacknowledged COMMIT or undo published writes.

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

## Related reference

[SQL client](index.md) · [Design](design.md) · [Shared lease](../assimalign-cohesion-database-client/database-connection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlClientTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseConnectionLeaseTests.cs`.
