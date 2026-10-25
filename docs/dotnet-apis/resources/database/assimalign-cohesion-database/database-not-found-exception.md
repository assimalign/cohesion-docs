# DatabaseNotFoundException

Namespace: `Assimalign.Cohesion.Database`.

```csharp
public sealed class DatabaseNotFoundException : DatabaseException
{
    public DatabaseNotFoundException(string message);
    public DatabaseNotFoundException(string message, Exception? innerException);
    public DatabaseNotFoundException(string message, IEnumerable<Diagnostic> diagnostics,
        Exception? innerException);
}
```

This remains the precise logical-database absence signal. The new three-argument overload copies and orders diagnostics through DatabaseException and rejects null diagnostics with ArgumentNullException. Its explicit third argument preserves existing two-argument null-inner calls. The message/inner overload preserves diagnostics when the cause is a DatabaseException; ordinary message construction leaves an empty, non-null carrier. Messages remain unprefixed when a structured identity is available.

[Exception root](database-exception.md) · [Database](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database/src/Exceptions/DatabaseNotFoundException.cs`.
