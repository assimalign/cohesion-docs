# Http Context Authentication Verb Extensions Tests

This example exercises `Assimalign.Cohesion.Web.Authentication` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/tests/HttpContextAuthenticationVerbExtensionsTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — context.`AuthenticateAsync`: throws when no service is installed.
- **Case 2** — context.`ChallengeAsync`: dispatches through the installed service.
- **Case 3** — context.`SignInAsync`: dispatches the principal through the service.

## Source example

```csharp
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Authentication.Tests.TestObjects;

namespace Assimalign.Cohesion.Web.Authentication.Tests;

public class HttpContextAuthenticationVerbExtensionsTests
{
    private static TestHttpContext CreateContextWithScheme(string scheme, RecordingAuthenticationHandler handler)
    {
        AuthenticationOptions options = new() { DefaultScheme = scheme };
        options.AddScheme(new AuthenticationScheme(scheme, null, () => handler));
        IAuthenticationService service = AuthenticationService.Create(options);

        TestHttpContext context = TestHttpContext.Create();
        context.Features.Set<IAuthenticationService>(service);
        return context;
    }

    [Fact(DisplayName = "Cohesion Test [Web.Authentication] - context.AuthenticateAsync: throws when no service is installed")]
    public async Task AuthenticateAsync_NoService_Throws()
    {
        // Arrange
        TestHttpContext context = TestHttpContext.Create();

        // Act + Assert
        await Should.ThrowAsync<InvalidOperationException>(() => context.AuthenticateAsync());
    }

    [Fact(DisplayName = "Cohesion Test [Web.Authentication] - context.ChallengeAsync: dispatches through the installed service")]
    public async Task ChallengeAsync_ServiceInstalled_Dispatches()
    {
        // Arrange
        RecordingAuthenticationHandler handler = new();
        TestHttpContext context = CreateContextWithScheme("Test", handler);

        // Act
        await context.ChallengeAsync();

        // Assert
        handler.ChallengeCount.ShouldBe(1);
    }

    [Fact(DisplayName = "Cohesion Test [Web.Authentication] - context.SignInAsync: dispatches the principal through the service")]
    public async Task SignInAsync_ServiceInstalled_Dispatches()
    {
        // Arrange
        RecordingAuthenticationHandler handler = new();
        TestHttpContext context = CreateContextWithScheme("Test", handler);
        ClaimsPrincipal principal = new(new ClaimsIdentity("Test"));

        // Act
        await context.SignInAsync(principal);

        // Assert
        handler.SignedInUser.ShouldBeSameAs(principal);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/tests/HttpContextAuthenticationVerbExtensionsTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/tests/Assimalign.Cohesion.Web.Authentication.Tests.csproj`.
