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
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http.ExtendedConnect.Tests.TestObjects;

namespace Assimalign.Cohesion.Http.ExtendedConnect.Tests;

public class HttpExtendedConnectExtensionsTests
{
    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: A present :protocol item exposes the feature")]
    public void ExtendedConnect_OnProtocolItemPresent_ShouldExposeFeature()
    {
        // Arrange
        FakeHttpContext context = new();
        context.Items[":protocol"] = "websocket";

        // Act / Assert
        context.IsExtendedConnect.ShouldBeTrue();
        context.ExtendedConnect.ShouldNotBeNull();
        context.ExtendedConnect!.Protocol.ShouldBe("websocket");
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: No :protocol item exposes no feature")]
    public void ExtendedConnect_OnNoProtocolItem_ShouldReturnNull()
    {
        // Arrange
        FakeHttpContext context = new();

        // Act / Assert
        context.IsExtendedConnect.ShouldBeFalse();
        context.ExtendedConnect.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Http.ExtendedConnect] - ExtendedConnect: An empty :protocol item exposes no feature")]
    public void ExtendedConnect_OnEmptyProtocolItem_ShouldReturnNull()
    {
        // Arrange
        FakeHttpContext context = new();
        context.Items[":protocol"] = "";

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

- **Covered behavior** — ExtendedConnect: A present :protocol item exposes the feature.
- **Covered behavior** — ExtendedConnect: No :protocol item exposes no feature.
- **Covered behavior** — ExtendedConnect: An empty :protocol item exposes no feature.
- **Covered behavior** — ExtendedConnect: A null context throws.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/tests/HttpExtendedConnectExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/tests/Assimalign.Cohesion.Http.ExtendedConnect.Tests.csproj`.
