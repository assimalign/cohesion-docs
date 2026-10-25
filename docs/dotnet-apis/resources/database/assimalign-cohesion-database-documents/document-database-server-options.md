# DocumentDatabaseServerOptions

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentDatabaseServerOptions : DatabaseServerOptions
{
    // Common members are inherited from DatabaseServerOptions.
    public TimeSpan TerminalDrainLinger { get; set; } = TimeSpan.FromSeconds(2);
}
```

## Behavior and exceptions

Listener is required and transfers its lifecycle to the server. A null Authenticator uses
DatabaseAuthenticator.AllowAll. MaxSessions bounds accepted sessions; authentication and idle
timeouts bound a peer's slot. IdleTimeout accepts Timeout.InfiniteTimeSpan. ShutdownDrainTimeout
also bounds terminal error/capacity refusal write, flush, output completion and peer-input drain.
TerminalDrainLinger bounds only peer close after output completion; the outer budget still applies.
Both drain and linger must be positive, finite and at most 4,294,967,294 milliseconds. Creation
copies options, preserving listener and authenticator references; later property changes do not alter a running server. The setters store
values. DocumentDatabaseServer.Create rejects missing listeners, nonpositive session limits and
invalid authentication/idle timer bounds with ArgumentException. Invalid drain/linger bounds throw
ArgumentOutOfRangeException. Null engine/options throw ArgumentNullException. Deadline aborts join
pending I/O before cleanup; a transport/execution ignoring abort can delay final Stop completion.
See [terminal delivery](design.md#terminal-errors-and-drain), including the TLS limitation (#1086).

## Example

```csharp
var options = new DocumentDatabaseServerOptions {
    Listener = listener, MaxSessions = 32, IdleTimeout = TimeSpan.FromMinutes(5),
    ShutdownDrainTimeout = TimeSpan.FromSeconds(2), TerminalDrainLinger = TimeSpan.FromMilliseconds(100)
};
await using var server = DocumentDatabaseServer.Create(engine, options);
```

## Related reference

[Server](document-database-server.md) · [Assembly](index.md)

## Shared TCP composition

Common members come from [DatabaseServerOptions](../assimalign-cohesion-database/database-server-options.md). [Database.Tcp](../assimalign-cohesion-database-tcp/index.md) supplies Listen(Uri) for this leaf; add `using Assimalign.Cohesion.Database;`. The extension returns the same concrete options type and the server owns the transferred listener.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Server/DocumentDatabaseServerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentServerDrainTests.cs`.
