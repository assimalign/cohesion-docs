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

ValidateCommon requires Listener and positive MaxSessions, otherwise ArgumentException. ValidateTimeout checks the selected model timer policy, otherwise ArgumentOutOfRangeException. Validation messages retain CK08001/CK08002/CK08003 once and identify model, engine and option ParamName. Documents allows zero/infinite authentication and idle values; Blob requires positive values or infinite. SQL/Graph/Key-Value Pair retain their existing timer policies. All models require positive finite shutdown/linger values no greater than 4,294,967,294 milliseconds.

[Shared TCP helper](../assimalign-cohesion-database-tcp/database-server-options-extensions.md) · [Database](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/DatabaseServerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/tests/DatabaseTcpTests.cs`.
