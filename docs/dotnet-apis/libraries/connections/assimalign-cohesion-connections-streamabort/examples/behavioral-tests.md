# Example: Stream Abort Feature Tests

Exercise directional-abort feature discovery using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `StreamAbortFeatureTests.cs` tests from the package test project.
Keep them in that project when running them: the project supplies its package references,
generated sources, and shared fixtures. The test-only doubles in `TestObjects` supply controlled
features and connections; they are not additional shipped APIs.

## Code

```csharp
using Shouldly;
using Xunit;

using Assimalign.Cohesion.Connections.StreamAbort.Tests.TestObjects;

namespace Assimalign.Cohesion.Connections.StreamAbort.Tests;

public class StreamAbortFeatureTests
{
    [Fact(DisplayName = "Cohesion Test [Connections.StreamAbort] - DirectionalAbort: Returns the registered feature")]
    public void DirectionalAbort_WhenRegistered_ShouldReturnFeature()
    {
        // Arrange
        IMultiplexedConnectionStream connection = new TestConnection();
        TestFeature feature = new();
        connection.Features.Set(feature);

        // Act
        IMultiplexedStreamAbort? result = connection.DirectionalAbort;

        // Assert
        result.ShouldBeSameAs(feature);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.StreamAbort] - DirectionalAbort: Returns null without a feature")]
    public void DirectionalAbort_WhenAbsent_ShouldReturnNull()
    {
        // Arrange
        IMultiplexedConnectionStream connection = new TestConnection();

        // Act
        IMultiplexedStreamAbort? result = connection.DirectionalAbort;

        // Assert
        result.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.StreamAbort] - FeatureName: Pins the registration name")]
    public void FeatureName_ForContract_ShouldMatchPinnedName()
    {
        // Arrange
        TestFeature feature = new();

        // Act
        string name = feature.Name;

        // Assert
        IMultiplexedStreamAbort.FeatureName.ShouldBe("StreamAbort");
        name.ShouldBe(IMultiplexedStreamAbort.FeatureName);
    }
}
```

## Walkthrough

- **Covered behavior** — DirectionalAbort: Returns the registered feature.
- **Covered behavior** — DirectionalAbort: Returns null without a feature.
- **Covered behavior** — FeatureName: Pins the registration name.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamAbort/tests/StreamAbortFeatureTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamAbort/tests/Assimalign.Cohesion.Connections.StreamAbort.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamAbort/tests/TestObjects/TestConnection.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.StreamAbort/tests/TestObjects/TestFeature.cs`.
