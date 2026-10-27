# KeyValueFunction

A registered scalar CALL with an exact primitive signature.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public abstract partial class KeyValueFunction
{
    protected KeyValueFunction(KeyValueFunctionName name, ReadOnlySpan<DatabaseType> parameters,
        DatabaseType returnType,
        KeyValueNullBehavior nullBehavior = KeyValueNullBehavior.ReturnsNullOnNullInput);
    public KeyValueFunctionName Name { get; }
    public IReadOnlyList<DatabaseType> Parameters { get; }
    public DatabaseType ReturnType { get; }
    public KeyValueNullBehavior NullBehavior { get; }
    public object? Invoke(scoped in KeyValueArguments arguments);
    protected abstract object? InvokeCore(scoped in KeyValueArguments arguments);
    public static KeyValueFunction Create<TResult>(KeyValueFunctionName name, Func<TResult> body,
        KeyValueNullBehavior nullBehavior = KeyValueNullBehavior.ReturnsNullOnNullInput);
    public static KeyValueFunction Create<T1, TResult>(KeyValueFunctionName name, Func<T1, TResult> body,
        KeyValueNullBehavior nullBehavior = KeyValueNullBehavior.ReturnsNullOnNullInput);
    // Typed factories also support two, three and four inputs; derived functions any fixed arity.
}
```

## Behavior and exceptions

Register on KeyValueDatabaseEngine.CreateBuilder(name).Functions before Build/BuildAsync.
The frozen catalog serves all engine databases. Direct Create has an empty catalog. Signatures
use native primitive DatabaseType values through Guid; there is no numeric coercion or aggregate
framework. NULL results are allowed. Overloads resolve exact non-NULL types and arity; NULL
is compatible with every signature and can be ambiguous. Default strict policy returns NULL
without a callback. Otherwise one valid CALL invokes the body exactly once. No determinism
flag or optimizer folding exists; callbacks may have external effects.

One function instance serves every session of its engine; keep it stateless or thread-safe.
Callbacks are trusted application code: external effects cannot be rolled back, and private CPU,
temporary allocations and I/O cannot be sandboxed or preempted by the engine. Do not reenter an
engine from a callback. Same-session reentry is refused before execution (CK07003); another
session can wait indefinitely for a lock held by the invoking statement. Borrowed arguments
and context must not be retained or mutated. Typed delegates do not receive the statement token;
derive a function and observe its borrowed context when cancellation is needed.

Unexpected callback failures, including application I/O and timeout, use CV05003 and retain
the original InnerException. DatabaseException, OutOfMemoryException,
InsufficientExecutionStackException and actual statement cancellation retain their identity.
These function conditions have no assigned SQLSTATE. Completed wire failures use
ExecutionFailure and leave the connection reusable.

Ordinary function failures keep an explicit Key-Value transaction active. CV02016 means an
unsupported result or a result different from ReturnType. Direct Invoke count/type mismatch
throws ArgumentException. Null delegates throw ArgumentNullException; default names and
invalid called-on-NULL type combinations ArgumentException; unsupported CLR factory types
NotSupportedException; unsupported DatabaseType/NULL enums ArgumentOutOfRangeException.
Binary arguments/results are detached; typed byte[] inputs receive defensive copies.

## Example

```csharp
var builder = KeyValueDatabaseEngine.CreateBuilder("typed-functions");
builder.Functions.Add(KeyValueFunction.Create<long, long>("f1", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
var database = await engine.CreateDatabaseAsync("app", CancellationToken.None);
await using var session = await database.CreateSessionAsync(CancellationToken.None);
await using var result = (DatabaseResultSet)await session.ExecuteAsync(new KeyValueFunctionRequest("f1", new object?[] { 4L }),
    CancellationToken.None); // one nullable Int64 column named value, one row: 4
```

## Related reference

[Assembly](index.md) · [KeyValueFunctionRequest](key-value-function-request.md) · [CALL](../../../../database/key-value-pair/commands/call.md)

## Function identifier

[KeyValueFunctionName](../assimalign-cohesion-database-keyvaluepair-protocol/key-value-function-name.md) supplies the identifier grammar and case-insensitive equality.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Functions/KeyValueFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
