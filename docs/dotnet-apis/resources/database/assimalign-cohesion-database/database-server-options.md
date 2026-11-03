# DatabaseServerOptions

Namespace: `Assimalign.Cohesion.Database`.

```csharp
public abstract class DatabaseServerOptions
{
    protected DatabaseServerOptions();
    public IDuplexConnectionListener? Listener { get; set; }
    public DatabaseAuthenticator? Authenticator { get; set; }
    public int MaxSessions { get; set; } = 100;
    public TimeSpan AuthenticationTimeout { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan ShutdownDrainTimeout { get; set; } = TimeSpan.FromSeconds(30);
    protected void ValidateCommon(string model, DatabaseName engineName);
    protected static void ValidateTimeout(TimeSpan value, string option, string model,
        DatabaseName engineName, bool allowInfinite = false, bool allowZero = false);
}
```

All five sealed model options derive from this base. TerminalDrainLinger remains on each leaf, defaulting to two seconds. Creation snapshots settings, preserving listener/authenticator references; later option changes do not reconfigure a server. The server owns the listener through stop. Null authentication selects DatabaseAuthenticator.AllowAll.

When Listener is null and COHESION_GATEWAY is nonempty, server creation reads the current
AppEnvironment scope (or process environment) for the model's named platform endpoint:

| Model | Assignment |
| --- | --- |
| SQL | `sql` |
| Documents | `documents` |
| Graph | `graph` |
| Key-Value Pair | `kv` |
| Blob | `blob` |

An explicit Listener, including Listen(Uri), wins. Fallback changes only the options snapshot;
the caller's options remain reusable. Missing/malformed assignments retain the missing-listener
ArgumentException with CK08001. An unmanaged server still requires an explicit listener even when
endpoint values exist. The default is plain TCP; the URI scheme does not enable TLS or select a
different transport. DNS bind hosts retain CK07101/InvalidOperationException. Use an explicit
listener for a secured or different transport.

ValidateTimeout checks the selected model timer policy before ValidateCommon checks positive
MaxSessions and resolves a listener. Invalid options therefore allocate no fallback listener.
Validation messages retain CK08001/CK08002/CK08003 once and identify model, engine and option
ParamName. Documents allows zero/infinite authentication and idle values; Blob requires positive
values or infinite. SQL/Graph/Key-Value Pair retain their existing timer policies. All models
require positive finite shutdown/linger values no greater than 4,294,967,294 milliseconds.

One assignment supplies one bind address per model. Two servers of the same model, on one engine
or different engines, need distinct listeners. Reusing the assignment causes AddressAlreadyInUse
at Start and host startup rolls back, releasing the first listener. Build does not reject the
collision. The in-process platform currently allocates only `http`; wire addresses remain explicit
there.

[Shared TCP helper](../assimalign-cohesion-database-tcp/database-server-options-extensions.md) · [Database](index.md)

## Sources

- **Managed defaults and option reuse** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/tests/DatabaseEndpointResourceTests.cs`, `DatabaseUnmanagedEndpointTests.cs` and `DatabaseTcpTests.cs` in the same directory.

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseServerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/tests/DatabaseTcpTests.cs`.
