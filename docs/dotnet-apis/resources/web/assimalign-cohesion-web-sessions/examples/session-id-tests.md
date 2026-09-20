# Session Id Tests

This example exercises `Assimalign.Cohesion.Web.Sessions` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Sessions/tests/SessionIdTests.cs`. It retains the
test class and assertions so the setup, operation, and expected outcome stay together. `Use` it in the
source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — SessionId: Should be a 22-character URL-safe 128-bit token.
- **Case 2** — SessionId: Successive ids should differ.

## Source example

```csharp
using System;
using System.Linq;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Web.Sessions.Internal;

namespace Assimalign.Cohesion.Web.Sessions.Tests;

public class SessionIdTests
{
    [Fact(DisplayName = "Cohesion Test [Web.Sessions] - SessionId: Should be a 22-character URL-safe 128-bit token")]
    public void Create_ShouldReturnUrlSafe128BitToken()
    {
        // Act
        string id = SessionId.Create();

        // Assert — 16 bytes of base64url (no padding) is 22 characters, all cookie-safe.
        id.Length.ShouldBe(22);
        id.All(static c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_').ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Web.Sessions] - SessionId: Successive ids should differ")]
    public void Create_Twice_ShouldProduceDistinctIds()
    {
        SessionId.Create().ShouldNotBe(SessionId.Create());
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Sessions/tests/SessionIdTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Sessions/tests/Assimalign.Cohesion.Web.Sessions.Tests.csproj`.
