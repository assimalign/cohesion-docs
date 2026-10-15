# Example: Stream Limits Feature Tests

Exercise cumulative stream-limit feature discovery using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `StreamLimitsFeatureTests.cs` tests from the package test project.
Keep them in that project when running them: the project supplies its package references,
generated sources, and shared fixtures. The test-only doubles in `TestObjects` supply controlled
features and connections; they are not additional shipped APIs.

## Code

```csharp
using Shouldly;
using Xunit;

using Assimalign.Cohesion.Connections.StreamLimits.Tests.TestObjects;

namespace Assimalign.Cohesion.Connections.StreamLimits.Tests;

public class StreamLimitsFeatureTests
{
    [Fact(DisplayName = "Cohesion Test [Connections.StreamLimits] - StreamLimits: Returns the registered feature")]
    public void StreamLimits_WhenRegistered_ShouldReturnFeature()
    {
        // Arrange
        IMultiplexedConnection connection = new TestConnection();
        TestFeature feature = new();
        connection.Features.Set(feature);

        // Act
        IMultiplexedConnectionStreamLimits? result = connection.StreamLimits;

        // Assert
        result.ShouldBeSameAs(feature);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.StreamLimits] - StreamLimits: Returns null without a feature")]
    public void StreamLimits_WhenAbsent_ShouldReturnNull()
    {
        // Arrange
        IMultiplexedConnection connection = new TestConnection();

        // Act
        IMultiplexedConnectionStreamLimits? result = connection.StreamLimits;

        // Assert
        result.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.StreamLimits] - FeatureName: Pins the registration name")]
    public void FeatureName_ForContract_ShouldMatchPinnedName()
    {
        // Arrange
        TestFeature feature = new();

        // Act
        string name = feature.Name;

        // Assert
        IMultiplexedConnectionStreamLimits.FeatureName.ShouldBe("StreamLimits");
        name.ShouldBe(IMultiplexedConnectionStreamLimits.FeatureName);
    }
}
```

## Walkthrough

- **Covered behavior** — StreamLimits: Returns the registered feature.
- **Covered behavior** — StreamLimits: Returns null without a feature.
- **Covered behavior** — FeatureName: Pins the registration name.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/tests/StreamLimitsFeatureTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/tests/Assimalign.Cohesion.Connections.StreamLimits.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/tests/TestObjects/TestConnection.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamLimits/tests/TestObjects/TestFeature.cs`.
