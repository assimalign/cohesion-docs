# Example: Resilience Pipeline Builder Tests

Exercise Resilience Pipeline Builder behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ResiliencePipelineBuilderTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Resilience.Tests;

public class ResiliencePipelineBuilderTests
{
    [Fact(DisplayName = "Cohesion Test [Resilience] - Builder: Execute invokes the callback through the strategy")]
    public void Builder_Execute_ShouldInvokeCallback()
    {
        IResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .UseStrategy(static async (callback, context, state) =>
            {
                await callback.Invoke(context, state).ConfigureAwait(context.ContinueOnCapturedContext);
                return Outcome.Success;
            })
            .Build();

        bool executed = false;

        pipeline.Execute<object?>((_, _) =>
        {
            executed = true;
        });

        executed.ShouldBeTrue();
    }
}
```

## Walkthrough

- **Covered behavior** — Builder: Execute invokes the callback through the strategy.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/tests/ResiliencePipelineBuilderTests.cs`.
- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/tests/Assimalign.Cohesion.Resilience.Tests.csproj`.
