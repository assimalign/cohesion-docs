# Web Socket Options Tests

This example exercises `Assimalign.Cohesion.Web.WebSockets` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/tests/WebSocketOptionsTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Options: The defaults refuse cross-site handshakes, keep alive every 30 seconds and leave compression off.
- **Case 2** — Options: A negative keep-alive interval or timeout is rejected.
- **Case 3** — UseWebSockets: An allowed origin that is not a serialized origin fails when the middleware is added.
- **Case 4** — UseWebSockets: A null builder throws.

## Source example

```csharp
using System;
using System.Net.WebSockets;
using System.Threading;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Web.WebSockets.Tests.TestObjects;

namespace Assimalign.Cohesion.Web.WebSockets.Tests;

public class WebSocketOptionsTests
{
    [Fact(DisplayName = "Cohesion Test [Web.WebSockets] - Options: The defaults refuse cross-site handshakes, keep alive every 30 seconds and leave compression off")]
    public void Options_Defaults_AreSafe()
    {
        // Act
        WebSocketOptions options = new();

        // Assert
        options.AllowedOrigins.ShouldBeEmpty();
        options.AllowAnyOrigin.ShouldBeFalse();
        options.KeepAliveInterval.ShouldBe(WebSocket.DefaultKeepAliveInterval);
        options.KeepAliveTimeout.ShouldBe(Timeout.InfiniteTimeSpan);
        options.DangerousEnableCompression.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Web.WebSockets] - Options: A negative keep-alive interval or timeout is rejected")]
    public void Options_NegativeKeepAlive_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        WebSocketOptions options = new();

        // Act / Assert
        Should.Throw<ArgumentOutOfRangeException>(() => options.KeepAliveInterval = TimeSpan.FromMilliseconds(-5));
        Should.Throw<ArgumentOutOfRangeException>(() => options.KeepAliveTimeout = TimeSpan.FromMilliseconds(-5));
        options.KeepAliveInterval = TimeSpan.Zero;
        options.KeepAliveTimeout = TimeSpan.FromSeconds(10);
        options.KeepAliveInterval.ShouldBe(TimeSpan.Zero);
        options.KeepAliveTimeout.ShouldBe(TimeSpan.FromSeconds(10));
    }

    [Theory(DisplayName = "Cohesion Test [Web.WebSockets] - UseWebSockets: An allowed origin that is not a serialized origin fails when the middleware is added")]
    [InlineData("*")]
    [InlineData("null")]
    [InlineData("https://app.example/")]
    [InlineData("app.example")]
    public void UseWebSockets_InvalidAllowedOrigin_ThrowsArgumentException(string origin)
    {
        // Arrange
        TestPipelineBuilder builder = new();

        // Act / Assert
        Should.Throw<ArgumentException>(() => builder.UseWebSockets(options => options.AllowedOrigins.Add(origin)));
    }

    [Fact(DisplayName = "Cohesion Test [Web.WebSockets] - UseWebSockets: A null builder throws")]
    public void UseWebSockets_NullBuilder_ThrowsArgumentNullException()
    {
        IWebApplicationPipelineBuilder builder = null!;

        Should.Throw<ArgumentNullException>(() => builder.UseWebSockets());
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/tests/WebSocketOptionsTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/tests/Assimalign.Cohesion.Web.WebSockets.Tests.csproj`.
