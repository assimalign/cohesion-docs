# Example: Memory Cache Options Tests

Exercise Memory Cache Options behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `MemoryCacheOptionsTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using Xunit;
using Assimalign.Cohesion.Caching.InMemory;

namespace Assimalign.Cohesion.Caching.InMemory.Tests;

public class MemoryCacheOptionsTests
{
    [Fact(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: defaults are sensible")]
    public void Defaults_AreSensible()
    {
        var options = new MemoryCacheOptions();

        Assert.Equal(TimeSpan.FromMinutes(1), options.ExpirationScanFrequency);
        Assert.Null(options.SizeLimit);
        Assert.Equal(0.05, options.CompactionPercentage);
        Assert.Null(options.TimeProvider);
    }

    [Theory(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: ExpirationScanFrequency must be > 0")]
    [InlineData(0)]
    [InlineData(-1)]
    public void ExpirationScanFrequency_NonPositive_Throws(int seconds)
    {
        var options = new MemoryCacheOptions();

        Assert.Throws<ArgumentOutOfRangeException>(() => options.ExpirationScanFrequency = TimeSpan.FromSeconds(seconds));
    }

    [Fact(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: ExpirationScanFrequency accepts any positive value")]
    public void ExpirationScanFrequency_Positive_Roundtrips()
    {
        var options = new MemoryCacheOptions
        {
            ExpirationScanFrequency = TimeSpan.FromSeconds(5),
        };

        Assert.Equal(TimeSpan.FromSeconds(5), options.ExpirationScanFrequency);
    }

    [Fact(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: SizeLimit accepts non-negative values")]
    public void SizeLimit_NonNegative_Roundtrips()
    {
        var options = new MemoryCacheOptions
        {
            SizeLimit = 0,
        };
        Assert.Equal(0, options.SizeLimit);

        options.SizeLimit = 1024;
        Assert.Equal(1024, options.SizeLimit);

        options.SizeLimit = null;
        Assert.Null(options.SizeLimit);
    }

    [Fact(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: negative SizeLimit throws")]
    public void SizeLimit_Negative_Throws()
    {
        var options = new MemoryCacheOptions();

        Assert.Throws<ArgumentOutOfRangeException>(() => options.SizeLimit = -1);
    }

    [Theory(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: CompactionPercentage must be in (0, 1]")]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(1.5)]
    public void CompactionPercentage_OutOfRange_Throws(double value)
    {
        var options = new MemoryCacheOptions();

        Assert.Throws<ArgumentOutOfRangeException>(() => options.CompactionPercentage = value);
    }

    [Theory(DisplayName = "Cohesion Test [Caching.InMemory] - MemoryCacheOptions: CompactionPercentage accepts edge values")]
    [InlineData(0.01)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void CompactionPercentage_InRange_Roundtrips(double value)
    {
        var options = new MemoryCacheOptions
        {
            CompactionPercentage = value,
        };

        Assert.Equal(value, options.CompactionPercentage);
    }
}
```

## Walkthrough

- **Covered behavior** — MemoryCacheOptions: defaults are sensible.
- **Covered behavior** — MemoryCacheOptions: ExpirationScanFrequency must be > 0.
- **Covered behavior** — MemoryCacheOptions: ExpirationScanFrequency accepts any positive value.
- **Covered behavior** — MemoryCacheOptions: SizeLimit accepts non-negative values.
- **Covered behavior** — MemoryCacheOptions: negative SizeLimit throws.
- **Covered behavior** — MemoryCacheOptions: CompactionPercentage must be in (0, 1].
- **Covered behavior** — MemoryCacheOptions: CompactionPercentage accepts edge values.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/tests/MemoryCacheOptionsTests.cs`.
- **Source** — `cohesion/libraries/Cache/Assimalign.Cohesion.Caching.InMemory/tests/Assimalign.Cohesion.Caching.InMemory.Tests.csproj`.
