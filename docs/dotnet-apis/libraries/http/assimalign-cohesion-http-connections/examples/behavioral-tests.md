# Example: Http Min Data Rate Tests

Exercise Http Min Data Rate behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpMinDataRateTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Connections.Tests;

public class HttpMinDataRateTests
{
    [Fact(DisplayName = "Cohesion Test [Http.Connections] - MinDataRate: Should store the rate and grace period")]
    public void Constructor_OnValidValues_ShouldStoreDimensions()
    {
        HttpMinDataRate rate = new(bytesPerSecond: 240, gracePeriod: TimeSpan.FromSeconds(5));

        rate.BytesPerSecond.ShouldBe(240);
        rate.GracePeriod.ShouldBe(TimeSpan.FromSeconds(5));
    }

    [Theory(DisplayName = "Cohesion Test [Http.Connections] - MinDataRate: Should reject a non-positive rate")]
    [InlineData(0d)]
    [InlineData(-1d)]
    public void Constructor_OnNonPositiveRate_ShouldThrow(double bytesPerSecond)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new HttpMinDataRate(bytesPerSecond, TimeSpan.FromSeconds(5)));
    }

    [Fact(DisplayName = "Cohesion Test [Http.Connections] - MinDataRate: Should reject a non-positive grace period")]
    public void Constructor_OnNonPositiveGracePeriod_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new HttpMinDataRate(240, TimeSpan.Zero));
        Should.Throw<ArgumentOutOfRangeException>(() => new HttpMinDataRate(240, TimeSpan.FromSeconds(-1)));
    }
}
```

## Walkthrough

- **Covered behavior** — MinDataRate: Should store the rate and grace period.
- **Covered behavior** — MinDataRate: Should reject a non-positive rate.
- **Covered behavior** — MinDataRate: Should reject a non-positive grace period.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/tests/HttpMinDataRateTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/tests/Assimalign.Cohesion.Http.Connections.Tests.csproj`.
