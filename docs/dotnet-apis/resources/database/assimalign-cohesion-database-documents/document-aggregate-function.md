# DocumentAggregateFunction

Creates one document accumulator per group and call appearance.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public abstract class DocumentAggregateFunction : DocumentFunction
{
    protected DocumentAggregateFunction(DocumentFunctionName name, int argumentCount,
        DatabaseType returnType,
        DocumentNullBehavior nullBehavior = DocumentNullBehavior.PropagateMissingOrNull,
        params DocumentFunctionArgumentKind[] argumentKinds);
    public DocumentAggregateAccumulator CreateAccumulator(scoped in DocumentFunctionContext context);
    protected abstract DocumentAggregateAccumulator CreateAccumulatorCore(
        scoped in DocumentFunctionContext context);
    public static DocumentAggregateFunction Create<TState, T1, TResult>(DocumentFunctionName name,
        Func<TState> seed, Func<TState, T1, TState> step, Func<TState, TResult> finish,
        DocumentNullBehavior nullBehavior = DocumentNullBehavior.PropagateMissingOrNull);
}
```

## Behavior and exceptions

An aggregate has fixed arity; zero uses star call syntax. Each call appearance has independent
argument evaluation and fresh state. Default policy skips NULL and MISSING. Incompatible valued
inputs are skipped by typed aggregates. A typed factory returns NULL without calling finish if
no input was admitted. A derived accumulator controls its own empty-group result.

Returning null/reused state, external callback failures and invalid result contracts use CD05017.
Cancellation uses OperationCanceledException. Constructor/factory guards match the scalar family:
null callbacks ArgumentNullException, unsupported CLR types NotSupportedException, invalid
name/kinds/type contracts ArgumentException and invalid metadata ArgumentOutOfRangeException.

## Example

```csharp
var total = DocumentAggregateFunction.Create<decimal, decimal, decimal>("total_value",
    () => 0m, (state, value) => state + value, state => state);
var context = default(DocumentFunctionContext);
var state = total.CreateAccumulator(in context);
var arguments = new DocumentArguments(new[] { DocumentValue.FromDecimal(2m) });
state.Add(in arguments);
Console.WriteLine(state.Finish().AsDecimal()); // 2
```

## Related reference

[Assembly](index.md) · [DocumentAggregateAccumulator](document-aggregate-accumulator.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentAggregateFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
