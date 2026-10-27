# GraphScalarFunction

Validated scalar invocation over a protected implementation core.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public abstract class GraphScalarFunction : GraphFunction
{
    protected GraphScalarFunction(GraphFunctionName name,
        IEnumerable<GraphFunctionType> parameters, GraphFunctionType returnType,
        GraphNullBehavior nullBehavior = GraphNullBehavior.ReturnsNullOnNullInput);
    public object? Invoke(scoped in GraphArguments arguments);
    protected abstract object? InvokeCore(scoped in GraphArguments arguments);
    public static GraphScalarFunction Create<TResult>(GraphFunctionName name, Func<TResult> body);
    public static GraphScalarFunction Create<T1, TResult>(GraphFunctionName name,
        Func<T1, TResult> body,
        GraphNullBehavior nullBehavior = GraphNullBehavior.ReturnsNullOnNullInput);
    // Create overloads also accept two, three or four typed inputs.
}
```

## Behavior and exceptions

Invoke checks cancellation, arity, NULL policy and argument shapes before InvokeCore, then
validates the result and checks cancellation again. Strict calls eagerly evaluate arguments but
return NULL before the callback if any input is NULL. CalledOnNullInput passes NULL through.

Typed numeric adapters accept long, double and decimal, including nullable forms; smaller,
unsigned and float CLR types are refused. See [GraphFunctionType](graph-function-type.md).
CalledOnNullInput requires nullable/reference parameters. Invalid metadata/types/names use
ArgumentException, null metadata/delegates use ArgumentNullException and an undefined NULL
enum uses ArgumentOutOfRangeException. Direct invalid calls use DatabaseException; cancellation
uses OperationCanceledException. Callback/result failures use CG05021; input shape/cast failures
use CG02013. [GraphFunction](graph-function.md) defines trusted-code and reentry rules.

## Example

```csharp
var function = GraphScalarFunction.Create<long, long>("developer_echo", value => value);
var arguments = new GraphArguments(new object?[] { 42L });
Console.WriteLine(function.Invoke(in arguments)); // 42
```

## Related reference

[Assembly](index.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
