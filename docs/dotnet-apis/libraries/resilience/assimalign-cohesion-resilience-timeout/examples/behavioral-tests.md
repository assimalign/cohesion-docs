# Example: Timeout Resilience Strategy Tests

Exercise Timeout Resilience Strategy behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `TimeoutResilienceStrategyTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Resilience;

namespace Assimalign.Cohesion.Resilience.Timeout.Tests;

public class TimeoutResilienceStrategyTests
{
    [Fact(DisplayName = "Cohesion Test [Resilience.Timeout] - Strategy: Timeout rejects slow execution")]
    public async Task Strategy_ExecuteAsync_ShouldRejectWhenTimeoutExpires()
    {
        IResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .UseTimeout(options => options.Timeout = TimeSpan.FromMilliseconds(20))
            .Build();

        ResilienceException exception = await Should.ThrowAsync<ResilienceException>(async () =>
        {
            await pipeline.ExecuteAsync<object?>(static async (_, _) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }).ConfigureAwait(false);
        });

        TimeoutRejectedException timeoutException = ExtractException<TimeoutRejectedException>(exception);

        timeoutException.Timeout.ShouldBe(TimeSpan.FromMilliseconds(20));
    }

    [Fact(DisplayName = "Cohesion Test [Resilience.Timeout] - Builder: Retry and timeout remain chainable through interfaces")]
    public async Task Builder_UseRetryThenUseTimeout_ShouldRemainChainable()
    {
        IResiliencePipelineBuilder<int> builder = new ResiliencePipelineBuilder<int>();

        IResiliencePipeline<int> pipeline = builder
            .UseRetry(_ => { })
            .UseTimeout(options => options.Timeout = TimeSpan.FromSeconds(1))
            .Build();

        int result = await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, static (_, _) => ValueTask.FromResult(5));

        result.ShouldBe(5);
    }

    private static TException ExtractException<TException>(Exception exception)
        where TException : Exception
    {
        Queue<Exception> pending = new();
        pending.Enqueue(exception);

        while (pending.Count > 0)
        {
            Exception current = pending.Dequeue();

            if (current is TException match)
            {
                return match;
            }

            if (current is AggregateException aggregate)
            {
                foreach (Exception inner in aggregate.InnerExceptions)
                {
                    pending.Enqueue(inner);
                }
            }

            if (current.InnerException is not null)
            {
                pending.Enqueue(current.InnerException);
            }
        }

        throw new InvalidOperationException($"Unable to locate an exception of type '{typeof(TException).FullName}'.");
    }
}
```

## Walkthrough

- **Covered behavior** — Strategy: Timeout rejects slow execution.
- **Covered behavior** — Builder: Retry and timeout remain chainable through interfaces.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/tests/TimeoutResilienceStrategyTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.Timeout/tests/Assimalign.Cohesion.Resilience.Timeout.Tests.csproj`.
