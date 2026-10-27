# DocumentScalarFunction

A validated document scalar over a protected callback core.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public abstract class DocumentScalarFunction : DocumentFunction
{
    protected DocumentScalarFunction(DocumentFunctionName name, int minimumArguments,
        int maximumArguments, DatabaseType returnType,
        DocumentNullBehavior nullBehavior = DocumentNullBehavior.PropagateMissingOrNull,
        params DocumentFunctionArgumentKind[] argumentKinds);
    public DocumentValue Invoke(scoped in DocumentArguments arguments);
    protected abstract DocumentValue InvokeCore(scoped in DocumentArguments arguments);
    public static DocumentScalarFunction Create<T1, TResult>(DocumentFunctionName name,
        Func<T1, TResult> body,
        DocumentNullBehavior nullBehavior = DocumentNullBehavior.PropagateMissingOrNull);
    // Typed Create overloads also accept two, three or four inputs.
}
```

## Behavior and exceptions

Invoke checks cancellation and arity before the NULL/MISSING policy, accepted valued kinds
and core/result validation. Default policy propagates MISSING before NULL without a callback.
CalledOnNullOrMissingInput delivers unknown inputs to nullable/reference or DocumentValue
parameters. Incompatible valued stored inputs yield scalar NULL; known incompatible literals
fail binding. Typed JsonElement inputs accept arrays/objects only.

Typed values are bool, int, long, decimal, string, nullable bool/int/long/decimal, DocumentValue
and JsonElement. Int64 results keep Int64 metadata; Int32 and Decimal use Decimal. Fractional
integer input conversion uses CD02008; range overflow uses CD02007 (22003) before callback entry.
An overflow thrown by the callback itself uses CD05017.

Null delegates throw ArgumentNullException; unsupported CLR types NotSupportedException;
invalid names/kinds or NULL/type combinations ArgumentException; invalid arity/enum/type metadata
ArgumentOutOfRangeException. A direct Invoke with wrong count throws ArgumentException.
Callback/result failures use CD05017; cancellation uses OperationCanceledException.

## Example

```csharp
var function = DocumentScalarFunction.Create<string, string>("slug",
    value => value.ToLowerInvariant());
var arguments = new DocumentArguments(new[] { DocumentValue.FromString("Alpha") });
Console.WriteLine(function.Invoke(in arguments).AsString()); // alpha
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
