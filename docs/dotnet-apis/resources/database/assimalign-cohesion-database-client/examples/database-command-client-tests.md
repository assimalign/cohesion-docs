# Database Command Client Tests

This example exercises `Assimalign.Cohesion.Database.Client` through its co-located test source.

> **Status:** Implemented.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseCommandClientTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — `Create`: Disposing a client retains its caller-owned transport.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Client.Tests;

public sealed class DatabaseCommandClientTests
{
    [Fact(DisplayName = "Cohesion Test [Database.Client] - Create: Disposing a client retains its caller-owned transport")]
    public void Create_WithCallerOwnedTransport_ShouldNotDisposeIt()
    {
        // Arrange
        using var transport = new RecordingHttpMessageInvoker();
        using IDatabaseCommandClient client = DatabaseCommandClient.Create(
            new Uri("https://resource.test:8443/custom/control"), "bootstrap-token", transport);

        // Act
        client.Dispose();

        // Assert
        transport.IsDisposed.ShouldBeFalse();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseCommandClientTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/Assimalign.Cohesion.Database.Client.Tests.csproj`.
