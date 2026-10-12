# Example: TLS Connection Feature Tests

Exercise TLS feature discovery using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are the complete `TlsConnectionFeatureExtensionsTests.cs` tests from the package test project.
Keep them in that project when running them: the project supplies its package references,
generated sources, and shared fixtures. The test-only doubles in `TestObjects` supply controlled
features and connections; they are not additional shipped APIs.

## Code

```csharp
using System.Threading.Tasks;

using Shouldly;
using Xunit;

using Assimalign.Cohesion.Connections.InMemory;
using Assimalign.Cohesion.Connections.Tls.Tests.TestObjects;

namespace Assimalign.Cohesion.Connections.Tls.Tests;

public class TlsConnectionFeatureExtensionsTests
{
    [Fact(DisplayName = "Cohesion Test [Connections.Tls] - Tls: A connection returns its registered feature")]
    public async Task Tls_OnConnectionWithFeature_ShouldReturnRegisteredFeature()
    {
        // Arrange
        (Connection client, Connection server) = InMemoryConnectionPair.Create();
        await using Connection clientLifetime = client;
        await using Connection serverLifetime = server;
        TestTlsFeature feature = new();
        client.Features.Set(feature);

        // Act
        ITlsConnectionInfo? tls = ((IConnection)client).Tls;

        // Assert
        tls.ShouldBeSameAs(feature);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Tls] - Tls: A connection without the feature returns null")]
    public async Task Tls_OnConnectionWithoutFeature_ShouldReturnNull()
    {
        // Arrange
        (Connection client, Connection server) = InMemoryConnectionPair.Create();
        await using Connection clientLifetime = client;
        await using Connection serverLifetime = server;

        // Act
        ITlsConnectionInfo? tls = ((IConnection)client).Tls;

        // Assert
        tls.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Tls] - Tls: A multiplexed connection returns its registered feature")]
    public async Task Tls_OnMultiplexedConnectionWithFeature_ShouldReturnRegisteredFeature()
    {
        // Arrange
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using MultiplexedConnection clientLifetime = client;
        await using MultiplexedConnection serverLifetime = server;
        TestTlsFeature feature = new();
        client.Features.Set(feature);

        // Act
        ITlsConnectionInfo? tls = ((IMultiplexedConnection)client).Tls;

        // Assert
        tls.ShouldBeSameAs(feature);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Tls] - Tls: A multiplexed connection without the feature returns null")]
    public async Task Tls_OnMultiplexedConnectionWithoutFeature_ShouldReturnNull()
    {
        // Arrange
        (MultiplexedConnection client, MultiplexedConnection server) = InMemoryMultiplexedConnectionPair.Create();
        await using MultiplexedConnection clientLifetime = client;
        await using MultiplexedConnection serverLifetime = server;

        // Act
        ITlsConnectionInfo? tls = ((IMultiplexedConnection)client).Tls;

        // Assert
        tls.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.Tls] - FeatureName: The TLS slot name is pinned")]
    public void FeatureName_ForTls_ShouldBePinnedValue()
    {
        // Arrange
        TestTlsFeature feature = new();

        // Act
        string name = feature.Name;

        // Assert
        ITlsConnectionInfo.FeatureName.ShouldBe("Tls");
        name.ShouldBe(ITlsConnectionInfo.FeatureName);
    }
}
```

## Walkthrough

- **Covered behavior** — Tls: A connection returns its registered feature.
- **Covered behavior** — Tls: A connection without the feature returns null.
- **Covered behavior** — Tls: A multiplexed connection returns its registered feature.
- **Covered behavior** — Tls: A multiplexed connection without the feature returns null.
- **Covered behavior** — FeatureName: The TLS slot name is pinned.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/tests/TlsConnectionFeatureExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/tests/Assimalign.Cohesion.Connections.Tls.Tests.csproj`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/tests/TestObjects/TestTlsFeature.cs`.
