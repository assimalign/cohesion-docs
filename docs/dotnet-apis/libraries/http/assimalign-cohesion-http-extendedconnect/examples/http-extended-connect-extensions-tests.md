# Example: Http Extended Connect Extensions Tests

Exercise Http Extended Connect Extensions behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpExtendedConnectExtensionsTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http.ExtendedConnect.Tests.TestObjects;

namespace Assimalign.Cohesion.Http.ExtendedConnect.Tests;

public class HttpExtendedConnectExtensionsTests
{
    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: An installed feature is exposed, the same instance on every read")]
    public void ExtendedConnect_OnInstalledFeature_ShouldExposeTheSameFeatureOnEveryRead()
    {
        // Arrange — the extended CONNECT interceptor installs its implementation on the feature collection.
        FakeHttpContext context = new();
        FakeExtendedConnectFeature feature = new("websocket", Stream.Null);
        context.Features.Set(feature);

        // Act
        IHttpExtendedConnectFeature? first = context.ExtendedConnect;
        IHttpExtendedConnectFeature? second = context.ExtendedConnect;

        // Assert
        context.IsExtendedConnect.ShouldBeTrue();
        first.ShouldBeSameAs(feature);
        second.ShouldBeSameAs(feature);
        first!.Protocol.ShouldBe("websocket");
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: Accepting through the accessor returns the feature's tunnel")]
    public async Task ExtendedConnect_OnAccept_ShouldReturnTheFeaturesTunnel()
    {
        // Arrange
        FakeHttpContext context = new();
        await using MemoryStream tunnel = new();
        FakeExtendedConnectFeature feature = new("websocket", tunnel);
        context.Features.Set(feature);

        // Act
        Stream accepted = await context.ExtendedConnect!.AcceptAsync();

        // Assert
        accepted.ShouldBeSameAs(tunnel);
        feature.AcceptCount.ShouldBe(1);
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: No installed feature exposes no feature")]
    public void ExtendedConnect_OnNoFeature_ShouldReturnNull()
    {
        // Arrange
        FakeHttpContext context = new();

        // Act / Assert
        context.IsExtendedConnect.ShouldBeFalse();
        context.ExtendedConnect.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: A :protocol item alone exposes no feature")]
    public void ExtendedConnect_OnProtocolItemWithoutFeature_ShouldReturnNull()
    {
        // Arrange — the former transport bridge published :protocol under Items. The accessors read only
        // the feature collection now: a value there models no capability the transport can honor.
        FakeHttpContext context = new();
        context.Items[":protocol"] = "websocket";

        // Act / Assert
        context.IsExtendedConnect.ShouldBeFalse();
        context.ExtendedConnect.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: A null context throws")]
    public void ExtendedConnect_OnNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange
        IHttpContext context = null!;

        // Act / Assert
        Should.Throw<ArgumentNullException>(() => { _ = context.ExtendedConnect; });
        Should.Throw<ArgumentNullException>(() => { _ = context.IsExtendedConnect; });
    }
}
```

## Walkthrough

- **Covered behavior** — ExtendedConnect: An installed feature is exposed, the same instance on every read.
- **Covered behavior** — ExtendedConnect: Accepting through the accessor returns the feature's tunnel.
- **Covered behavior** — ExtendedConnect: No installed feature exposes no feature.
- **Covered behavior** — ExtendedConnect: A :protocol item alone exposes no feature.
- **Covered behavior** — ExtendedConnect: A null context throws.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/tests/HttpExtendedConnectExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/tests/Assimalign.Cohesion.Http.ExtendedConnect.Tests.csproj`.
