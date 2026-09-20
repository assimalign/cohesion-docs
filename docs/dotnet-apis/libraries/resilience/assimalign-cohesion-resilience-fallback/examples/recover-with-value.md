# Example: Recover with a fallback value

Return a configured fallback value when a pipeline callback fails.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using System;
using System.Threading.Tasks;

using Assimalign.Cohesion.Resilience;

IResiliencePipeline<int> pipeline = new ResiliencePipelineBuilder<int>()
    .UseFallback(options =>
    {
        options.FallbackAction = _ => ValueTask.FromResult(42);
    })
    .Build();

int result = ResilienceExtensions.Execute<int, object?>(
    pipeline,
    static (_, _) => throw new InvalidOperationException("boom"));
```

## Walkthrough

The callback throws an ordinary operation failure. The default fallback predicate handles that
failure and invokes `FallbackAction`, producing `42`. The originating test asserts that value;
cancellation remains excluded by the default predicate.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests/FallbackResilienceStrategyTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/src/FallbackStrategyOptions.TResult.cs`.
