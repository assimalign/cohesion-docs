# GraphAggregateAccumulator

Single-group state with one-time attachment and retained-memory accounting.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public abstract class GraphAggregateAccumulator : IDisposable
{
    protected GraphAggregateAccumulator();
    public void Add(scoped in GraphArguments arguments);
    public object? Finish();
    public void Dispose();
    protected abstract void AddCore(scoped in GraphArguments arguments);
    protected abstract object? FinishCore();
    protected abstract long RetainedBytesCore { get; }
    protected virtual void DisposeCore();
}
```

## Behavior and exceptions

The factory attaches state once. Add/Finish on unattached or already finished state throw
InvalidOperationException; calls after disposal throw ObjectDisposedException. Finish is
one-shot. Negative retained bytes, reused state, invalid results and callback/disposal failures
use CG05021. Cancellation uses OperationCanceledException. State charges obey CG05020 and
include retained referenced data, excluding the framework's fixed group charge.

The engine disposes every created group on success/failure and releases charges; cleanup does
not replace an earlier statement failure. Dispose releases private resources once.

## Example

```csharp
var function = GraphAggregateFunction.Create<long, long, long>("developer_sum",
    () => 0L, (state, value) => state + value, state => state, _ => 8);
var context = new GraphFunctionContext(default, DateTimeOffset.UnixEpoch);
using var state = function.CreateAccumulator(in context);
Console.WriteLine(state.Finish()); // 0
// A second Finish throws InvalidOperationException.
```

## Related reference

[Assembly](index.md) · [GraphAggregateFunction](graph-aggregate-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphAggregateAccumulator.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
