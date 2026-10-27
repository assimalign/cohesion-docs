# DocumentAggregateAccumulator

One document aggregate call's state for one group.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public abstract class DocumentAggregateAccumulator
{
    protected DocumentAggregateAccumulator();
    public void Add(scoped in DocumentArguments arguments);
    public DocumentValue Finish();
    protected abstract void AddCore(scoped in DocumentArguments arguments);
    protected abstract DocumentValue FinishCore();
}
```

## Behavior and exceptions

CreateAccumulator attaches state exactly once. Reusing it is CD05017. Add/Finish on
unattached or finished state throw InvalidOperationException; wrong Add arity throws
ArgumentException. Finish runs once and checks the declared result type. Default policy skips
NULL/MISSING before AddCore; incompatible valued input kinds also skip the core.
Callback/result failures use CD05017 and cancellation uses OperationCanceledException.
Its public lifecycle is Add followed by Finish.

## Example

```csharp
var total = DocumentAggregateFunction.Create<decimal, decimal, decimal>("total_value",
    () => 0m, (state, value) => state + value, state => state);
var context = default(DocumentFunctionContext);
var state = total.CreateAccumulator(in context);
Console.WriteLine(state.Finish().IsNull); // True: no admitted input
// A second Finish throws InvalidOperationException.
```

## Related reference

[Assembly](index.md) · [DocumentAggregateFunction](document-aggregate-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentAggregateAccumulator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
