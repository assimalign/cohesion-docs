# Example: Http Connection Info Tests

Exercise Http Connection Info behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpConnectionInfoTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Net;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Tests;

public class HttpConnectionInfoTests
{
    [Fact]
    public void Constructor_IPEndPoints_ShouldExposeDerivedAddressAndPortInformation()
    {
        // Arrange
        IPEndPoint local = new(IPAddress.Loopback, 8080);
        IPEndPoint remote = new(IPAddress.Parse("192.168.1.20"), 52341);

        // Act
        HttpConnectionInfo info = new(local, remote);

        // Assert
        info.LocalEndPoint.ShouldBe(local);
        info.RemoteEndPoint.ShouldBe(remote);
        info.LocalIp.ShouldBe(local.Address);
        info.RemoteIp.ShouldBe(remote.Address);
        info.LocalPort.ShouldBe(8080);
        info.RemotePort.ShouldBe(52341);
    }
}
```

## Walkthrough

- **Test entry point** — `Constructor_IPEndPoints_ShouldExposeDerivedAddressAndPortInformation` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/tests/Connection/HttpConnectionInfoTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/tests/Assimalign.Cohesion.Http.Tests.csproj`.
