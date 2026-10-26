# DocumentFunction

Immutable metadata for native and application document functions.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public abstract class DocumentFunction
{
    public DocumentFunctionName Name { get; }
    public DocumentFunctionKind Kind { get; }
    public int MinimumArguments { get; }
    public int MaximumArguments { get; }
    public DatabaseType ReturnType { get; }
    public DocumentNullBehavior NullBehavior { get; }
    public IReadOnlyList<DocumentFunctionArgumentKind> ArgumentKinds { get; }
}
```

## Behavior and exceptions

Derive DocumentScalarFunction or DocumentAggregateFunction; the base constructor is not an
external extension point. Register through DocumentDatabaseEngine.CreateBuilder(name).Functions.
Build freezes the catalog before storage/workers. Direct Create uses native functions only.
Metadata copies its masks; an empty list means Any and the last mask repeats for trailing inputs.
ReturnType Null means a dynamic result. This model describes parameters with
DocumentFunctionArgumentKind and result metadata with DatabaseType.

Application overloads have disjoint arity ranges and one scalar/aggregate kind per name.
No determinism/volatility flag or purity promise is exposed. Each aggregate appearance in
projection, HAVING and ORDER BY has independent arguments and state. EXPLAIN invokes no callbacks.
Engine requests resolve custom names automatically; standalone parsers keep unknown-name
diagnostics unless constructed with allowUnregisteredFunctions: true. Unknown names use CD00008; arity uses CD00006.

One function instance serves every session of its engine; keep it stateless or thread-safe.
Callbacks are trusted application code: external effects cannot be rolled back, and private CPU,
temporary allocations and I/O cannot be sandboxed or preempted by the engine. Do not reenter an
engine from a callback. Same-session reentry is refused before execution (CK07003); another
session can wait indefinitely for a lock held by the invoking statement. Borrowed arguments
and context must not be retained or mutated. Typed delegates do not receive the statement token;
derive a function and observe its borrowed context when cancellation is needed.

Unexpected callback failures, including application I/O and timeout, use CD05017 and retain
the original InnerException. DatabaseException, OutOfMemoryException,
InsufficientExecutionStackException and actual statement cancellation retain their identity.
These function conditions have no assigned SQLSTATE. Completed wire failures use
ExecutionFailure and leave the connection reusable.

Documents statement failures abort an explicit transaction. ROLLBACK recovers it.

## Example

```csharp
var builder = DocumentDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(DocumentScalarFunction.Create<string, string>("slug",
    value => value.ToLowerInvariant()));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("SLUG").Count); // 1
```

## Related reference

[Assembly](index.md) · [DocumentFunctionCatalog](document-function-catalog.md) · [DocumentScalarFunction](document-scalar-function.md)

## Function identifier

[DocumentFunctionName](document-function-name.md) supplies the identifier grammar and case-insensitive equality.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
