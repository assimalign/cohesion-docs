# DocumentDatabaseServerOptions

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public sealed class DocumentDatabaseServerOptions
{
    public IDuplexConnectionListener? Listener { get; set; }
    public DatabaseAuthenticator? Authenticator { get; set; }
    public int MaxSessions { get; set; } = 100;
    public TimeSpan AuthenticationTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan ShutdownDrainTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
```

## Behavior and exceptions

Listener is required and transfers its lifecycle to the server. A null Authenticator uses
DatabaseAuthenticator.AllowAll. MaxSessions bounds accepted sessions; authentication and idle
timeouts bound a peer's slot. IdleTimeout accepts Timeout.InfiniteTimeSpan. ShutdownDrainTimeout
also bounds terminal error write/flush/closure. Creation copies options, preserving listener and
authenticator references; later property changes do not alter a running server. The setters store
values. DocumentDatabaseServer.Create rejects missing listeners, nonpositive session limits and
invalid timer bounds with ArgumentException.

## Example

```csharp
var options = new DocumentDatabaseServerOptions {
    Listener = listener, MaxSessions = 32, IdleTimeout = TimeSpan.FromMinutes(5)
};
await using var server = DocumentDatabaseServer.Create(engine, options);
```

## Related reference

[Server](document-database-server.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Server/DocumentDatabaseServerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentServerDrainTests.cs`.
