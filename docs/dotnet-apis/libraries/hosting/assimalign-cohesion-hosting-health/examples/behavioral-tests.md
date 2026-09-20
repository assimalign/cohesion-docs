# Example: Health Status Tests

Exercise Health Status behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HealthStatusTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Hosting.Health.Tests;

public class HealthStatusTests
{
    private const string DisplayPrefix = "Cohesion Test [Hosting] - HealthStatus: ";

    [Fact(DisplayName = DisplayPrefix + "Values remain ordered from least to most healthy")]
    public void Values_WhenCompared_ShouldPreserveAggregationOrder()
    {
        // Arrange & Act & Assert
        ((int)HealthStatus.Unhealthy).ShouldBe(0);
        ((int)HealthStatus.Degraded).ShouldBe(1);
        ((int)HealthStatus.Healthy).ShouldBe(2);
    }
}
```

## Walkthrough

- **Test entry point** — `Values_WhenCompared_ShouldPreserveAggregationOrder` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/tests/HealthStatusTests.cs`.
- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/tests/Assimalign.Cohesion.Hosting.Health.Tests.csproj`.
