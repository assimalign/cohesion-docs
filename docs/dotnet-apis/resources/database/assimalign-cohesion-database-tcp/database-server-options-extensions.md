# DatabaseServerOptionsExtensions

Namespace: `Assimalign.Cohesion.Database`. Assembly/package: `Assimalign.Cohesion.Database.Tcp`.

```csharp
public static partial class DatabaseServerOptionsExtensions
{
    extension<TOptions>(TOptions options) where TOptions : DatabaseServerOptions
    {
        public TOptions Listen(Uri endpoint);
    }
}
```

Listen configures a TcpConnectionListener and returns the same concrete options instance for all five model leaves. The server snapshots options and takes ownership of its listener through stop/disposal. Reference Database.Tcp and import Assimalign.Cohesion.Database.

Endpoint URIs must be absolute with a host and explicit positive port, and have no user information, query or fragment. Uri.IdnHost supplies the socket-facing host. IP literals, localhost (IPv4 loopback) and wildcard/any addresses are supported; DNS names are synchronously refused with CK07101 in the legacy message. Port zero is invalid for this resolved-URI helper; transport-level ephemeral listeners can still be assigned through Listener. Null options/endpoint throw ArgumentNullException, invalid URIs ArgumentException and unsupported hosts InvalidOperationException.

[Assembly](index.md) · [Example](examples/configure-listener.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/src/Extensions/DatabaseServerOptionsExtensions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/tests/DatabaseTcpTests.cs`.
