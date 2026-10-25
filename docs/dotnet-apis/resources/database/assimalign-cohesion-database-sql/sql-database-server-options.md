# SqlDatabaseServerOptions

Configures session admission and graceful/terminal delivery for the Sql server.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public sealed class SqlDatabaseServerOptions : DatabaseServerOptions
{
    // Common members are inherited from DatabaseServerOptions.
    public TimeSpan TerminalDrainLinger { get; set; } = TimeSpan.FromSeconds(2);
}
```

## Behavior and exceptions

Listener is required; the server owns its bind-through-disposal lifecycle. A null Authenticator uses
DatabaseAuthenticator.AllowAll. MaxSessions includes handshaking peers. Creation snapshots all
settings, preserving listener/authenticator references; later setter changes do not reconfigure it.

ShutdownDrainTimeout bounds graceful shutdown and each terminal error/capacity refusal's write,
flush, output completion and peer-input drain with one deadline. TerminalDrainLinger bounds only
the peer-close wait after output completion; the outer budget still applies. Each must be positive,
finite and at most 4,294,967,294 milliseconds. SqlDatabaseServer.Create throws
ArgumentOutOfRangeException for invalid drain/linger values, ArgumentException for a missing
listener or nonpositive session limit, and ArgumentNullException for null engine/options.

A deadline aborts blocked I/O; final cleanup joins it before releasing resources and can be delayed
by a transport/execution that ignores abort. See [server delivery](design.md#terminal-errors-and-drain)
and its TLS limitation (#1086).

## Example

With an existing engine and listener, following the server drain and linger tests:

```csharp
var options = new SqlDatabaseServerOptions {
    Listener = listener,
    ShutdownDrainTimeout = TimeSpan.FromSeconds(2),
    TerminalDrainLinger = TimeSpan.FromMilliseconds(100)
};
await using var server = SqlDatabaseServer.Create(engine, options);
await server.StartAsync(CancellationToken.None);
await server.StopAsync(CancellationToken.None);
```

The server finishes pending work or aborts it after the drain budget, then joins cleanup.

## Related reference

[Assembly](index.md) · [Server design](design.md#terminal-errors-and-drain) · [Shared protocol](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)

## Shared TCP composition

Common members come from [DatabaseServerOptions](../assimalign-cohesion-database/database-server-options.md). [Database.Tcp](../assimalign-cohesion-database-tcp/index.md) supplies Listen(Uri) for this leaf; add `using Assimalign.Cohesion.Database;`. The extension returns the same concrete options type and the server owns the transferred listener.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Server/SqlDatabaseServerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlServerDrainTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlServerRejectionSetupTests.cs`.
