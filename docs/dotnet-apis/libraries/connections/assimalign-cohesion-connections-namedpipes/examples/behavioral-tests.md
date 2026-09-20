# Example: Named Pipe End Point Tests

Exercise Named Pipe End Point behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `NamedPipeEndPointTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Net.Sockets;
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Connections.NamedPipes.Tests;

public class NamedPipeEndPointTests
{
    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should default the server name to the local host")]
    public void Constructor_WithPipeNameOnly_ShouldDefaultToLocalServer()
    {
        // Arrange / Act
        NamedPipeEndPoint endPoint = new("orders");

        // Assert
        endPoint.PipeName.ShouldBe("orders");
        endPoint.ServerName.ShouldBe(".");
        endPoint.IsLocal.ShouldBeTrue();
        endPoint.AddressFamily.ShouldBe(AddressFamily.Unspecified);
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should format as a Windows pipe path")]
    public void ToString_ShouldFormatAsWindowsPipePath()
    {
        // Arrange
        NamedPipeEndPoint endPoint = new("orders", "build-server");

        // Act / Assert
        endPoint.ToString().ShouldBe(@"\\build-server\pipe\orders");
        endPoint.IsLocal.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should compare case-insensitively")]
    public void Equals_WithDifferentCasing_ShouldBeEqual()
    {
        // Arrange
        NamedPipeEndPoint a = new("Orders");
        NamedPipeEndPoint b = new("orders", ".");

        // Act / Assert
        a.Equals(b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Theory(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should reject null or empty names")]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidPipeName_ShouldThrowArgumentException(string? pipeName)
    {
        // Act / Assert
        Should.Throw<ArgumentException>(() => new NamedPipeEndPoint(pipeName!));
    }

    [Fact(DisplayName = "Cohesion Test [Connections.NamedPipes] - EndPoint: Should reject an empty server name")]
    public void Constructor_WithEmptyServerName_ShouldThrowArgumentException()
    {
        // Act / Assert
        Should.Throw<ArgumentException>(() => new NamedPipeEndPoint("orders", ""));
    }
}
```

## Walkthrough

- **Covered behavior** — EndPoint: Should default the server name to the local host.
- **Covered behavior** — EndPoint: Should format as a Windows pipe path.
- **Covered behavior** — EndPoint: Should compare case-insensitively.
- **Covered behavior** — EndPoint: Should reject null or empty names.
- **Covered behavior** — EndPoint: Should reject an empty server name.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/tests/NamedPipeEndPointTests.cs`.
- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/tests/Assimalign.Cohesion.Connections.NamedPipes.Tests.csproj`.
