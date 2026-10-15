# Example: Hedging Resilience Strategy Tests

Exercise Hedging Resilience Strategy behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HedgingResilienceStrategyTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Threading.Tasks;
using System.Threading;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Resilience;

namespace Assimalign.Cohesion.Resilience.Hedging.Tests;

public class HedgingResilienceStrategyTests
{
    [Fact(DisplayName = "Cohesion Test [Resilience.Hedging] - Strategy: Async execute returns the first successful hedged result")]
    public async Task Strategy_ExecuteAsync_ShouldReturnFirstSuccessfulResult()
    {
        int attempts = 0;

        IResiliencePipeline<int> pipeline = new ResiliencePipelineBuilder<int>()
            .UseHedging(options =>
            {
                options.Delay = TimeSpan.FromMilliseconds(10);
                options.MaxHedgedAttempts = 2;
            })
            .Build();

        async ValueTask<int> ExecuteAsync(IResilienceContext context, object? _)
        {
            int attempt = Interlocked.Increment(ref attempts);

            if (attempt == 1)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(80), context.CancellationToken);
                throw new InvalidOperationException("slow failure");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(5), context.CancellationToken);
            return 42;
        }

        int result = await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, ExecuteAsync);

        result.ShouldBe(42);
        attempts.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact(DisplayName = "Cohesion Test [Resilience.Hedging] - Strategy: Sync execute can recover with a hedged retry")]
    public void Strategy_Execute_ShouldRecoverWithSecondAttempt()
    {
        int attempts = 0;

        IResiliencePipeline<int> pipeline = new ResiliencePipelineBuilder<int>()
            .UseHedging(options =>
            {
                options.Delay = TimeSpan.Zero;
                options.MaxHedgedAttempts = 2;
            })
            .Build();

        int result = ResilienceExtensions.Execute<int, object?>(pipeline, (_, _) =>
        {
            int attempt = Interlocked.Increment(ref attempts);

            return attempt == 1
                ? throw new InvalidOperationException("boom")
                : 7;
        });

        result.ShouldBe(7);
        attempts.ShouldBeGreaterThanOrEqualTo(2);
    }
}
```

## Walkthrough

- **Covered behavior** — Strategy: Async execute returns the first successful hedged result.
- **Covered behavior** — Strategy: Sync execute can recover with a hedged retry.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/tests/HedgingResilienceStrategyTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Hedging/tests/Assimalign.Cohesion.Resilience.Hedging.Tests.csproj`.
