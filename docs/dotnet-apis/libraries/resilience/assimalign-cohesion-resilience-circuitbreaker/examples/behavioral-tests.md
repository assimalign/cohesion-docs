# Example: Circuit Breaker Resilience Strategy Tests

Exercise Circuit Breaker Resilience Strategy behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `CircuitBreakerResilienceStrategyTests.cs` listing from the package test
project. Keep it in that project when running it: the project supplies its package references,
generated sources, and any shared fixtures. The using block below makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Resilience;

namespace Assimalign.Cohesion.Resilience.CircuitBreaker.Tests;

public class CircuitBreakerResilienceStrategyTests
{
    [Fact(DisplayName = "Cohesion Test [Resilience.CircuitBreaker] - Strategy: Breaker opens after threshold and later recovers")]
    public async Task Strategy_ExecuteAsync_ShouldOpenAndRecover()
    {
        TestTimeProvider timeProvider = new(DateTimeOffset.Parse("2026-03-21T00:00:00Z"));
        int callbackCalls = 0;
        bool shouldFail = true;

        IResiliencePipeline<int> pipeline = new ResiliencePipelineBuilder<int>()
            .UseCircuitBreaker(options =>
            {
                options.TimeProvider = timeProvider;
                options.FailureThreshold = 2;
                options.BreakDuration = TimeSpan.FromSeconds(5);
            })
            .Build();

        async ValueTask<int> ExecuteAsync(IResilienceContext _, object? __)
        {
            callbackCalls++;

            if (shouldFail)
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            }

            return 9;
        }

        await Should.ThrowAsync<ResilienceException>(async () =>
        {
            await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, ExecuteAsync).ConfigureAwait(false);
        });

        await Should.ThrowAsync<ResilienceException>(async () =>
        {
            await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, ExecuteAsync).ConfigureAwait(false);
        });

        ResilienceException wrapped = await Should.ThrowAsync<ResilienceException>(async () =>
        {
            await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, ExecuteAsync).ConfigureAwait(false);
        });

        BrokenCircuitException broken = ExtractException<BrokenCircuitException>(wrapped);

        broken.RetryAfter.ShouldBeGreaterThan(TimeSpan.Zero);
        callbackCalls.ShouldBe(2);

        timeProvider.Advance(TimeSpan.FromSeconds(5));
        shouldFail = false;

        int result = await ResilienceExtensions.ExecuteAsync<int, object?>(pipeline, ExecuteAsync);

        result.ShouldBe(9);
        callbackCalls.ShouldBe(3);
    }

    private sealed class TestTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public TestTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }

        public void Advance(TimeSpan amount)
        {
            _utcNow = _utcNow.Add(amount);
        }
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

- **Covered behavior** — Strategy: Breaker opens after threshold and later recovers.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/tests/CircuitBreakerResilienceStrategyTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.CircuitBreaker/tests/Assimalign.Cohesion.Resilience.CircuitBreaker.Tests.csproj`.
