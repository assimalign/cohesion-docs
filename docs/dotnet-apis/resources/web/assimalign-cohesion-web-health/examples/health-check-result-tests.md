# Health Check Result Tests

This example exercises `Assimalign.Cohesion.Web.Health` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Health/tests/HealthCheckResultTests.cs`. It retains
the test class and assertions so the setup, operation, and expected outcome stay together. `Use` it in
the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — `HealthCheckResult`: `Healthy` factory sets status and empty data.
- **Case 2** — `HealthCheckResult`: `Unhealthy` factory captures exception.
- **Case 3** — `HealthCheckResult`: `Degraded` factory carries data.

## Source example

```csharp
using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Web.Health.Tests;

public class HealthCheckResultTests
{
    [Fact(DisplayName = "Cohesion Test [Health] - HealthCheckResult: Healthy factory sets status and empty data")]
    public void Healthy_WhenCreated_ShouldReportHealthyWithEmptyData()
    {
        HealthCheckResult result = HealthCheckResult.Healthy("all good");

        result.Status.ShouldBe(HealthStatus.Healthy);
        result.Description.ShouldBe("all good");
        result.Exception.ShouldBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Cohesion Test [Health] - HealthCheckResult: Unhealthy factory captures exception")]
    public void Unhealthy_WhenGivenException_ShouldCaptureIt()
    {
        var exception = new InvalidOperationException("boom");

        HealthCheckResult result = HealthCheckResult.Unhealthy("down", exception);

        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Exception.ShouldBeSameAs(exception);
    }

    [Fact(DisplayName = "Cohesion Test [Health] - HealthCheckResult: Degraded factory carries data")]
    public void Degraded_WhenGivenData_ShouldExposeIt()
    {
        var data = new Dictionary<string, object> { ["latencyMs"] = 250 };

        HealthCheckResult result = HealthCheckResult.Degraded("slow", data: data);

        result.Status.ShouldBe(HealthStatus.Degraded);
        result.Data["latencyMs"].ShouldBe(250);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/tests/HealthCheckResultTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/tests/Assimalign.Cohesion.Web.Health.Tests.csproj`.
