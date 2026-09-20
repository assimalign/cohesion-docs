# Example: Fallback Resilience Strategy Tests

Exercise Fallback Resilience Strategy behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `FallbackResilienceStrategyTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using System.Threading.Tasks;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Resilience;

namespace Assimalign.Cohesion.Resilience.Fallback.Tests;

public class FallbackResilienceStrategyTests
{
    [Fact(DisplayName = "Cohesion Test [Resilience.Fallback] - Strategy: Async fallback action runs for handled failure")]
    public async Task Strategy_ExecuteAsync_ShouldRunFallbackActionOnHandledFailure()
    {
        bool fallbackInvoked = false;

        IResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .UseFallback(options =>
            {
                options.FallbackAction = _ =>
                {
                    fallbackInvoked = true;
                    return ValueTask.CompletedTask;
                };
            })
            .Build();

        await pipeline.ExecuteAsync<object?>(static (_, _) => ValueTask.FromException(new InvalidOperationException("boom")));

        fallbackInvoked.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Resilience.Fallback] - Strategy: Sync execute returns fallback value")]
    public void Strategy_Execute_ShouldReturnFallbackValue()
    {
        IResiliencePipeline<int> pipeline = new ResiliencePipelineBuilder<int>()
            .UseFallback(options =>
            {
                options.FallbackAction = _ => ValueTask.FromResult(42);
            })
            .Build();

        int result = ResilienceExtensions.Execute<int, object?>(pipeline, static (_, _) => throw new InvalidOperationException("boom"));

        result.ShouldBe(42);
    }
}
```

## Walkthrough

- **Covered behavior** — Strategy: Async fallback action runs for handled failure.
- **Covered behavior** — Strategy: Sync execute returns fallback value.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests/FallbackResilienceStrategyTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Fallback/tests/Assimalign.Cohesion.Resilience.Fallback.Tests.csproj`.
