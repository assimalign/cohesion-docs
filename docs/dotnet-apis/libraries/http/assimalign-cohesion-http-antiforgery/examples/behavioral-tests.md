# Example: Http Context Antiforgery Extensions Tests

Exercise Http Context Antiforgery Extensions behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpContextAntiforgeryExtensionsTests.cs` listing from the package test
project. Keep it in that project when running it: the project supplies its package references,
generated sources, and any shared fixtures. The using block below makes the test-framework import
explicit where the original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Antiforgery.Tests;

public class HttpContextAntiforgeryExtensionsTests
{
    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - Antiforgery: Should return null before a feature is installed")]
    public void Antiforgery_BeforeSet_ShouldReturnNull()
    {
        IHttpContext context = new TestHttpContext();

        context.Antiforgery.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - Antiforgery: Should round-trip through the setter")]
    public void Antiforgery_Set_ShouldRoundTripViaGetter()
    {
        IHttpContext context = new TestHttpContext();
        IHttpAntiforgery service = HttpAntiforgery.Create();

        context.Antiforgery = service;

        context.Antiforgery.ShouldBeSameAs(service);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - Antiforgery: Should install an antiforgery feature on set")]
    public void Antiforgery_Set_ShouldInstallFeature()
    {
        IHttpContext context = new TestHttpContext();

        context.Antiforgery = HttpAntiforgery.Create();

        context.Features.Get<IHttpAntiforgeryFeature>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - Antiforgery: Should reject a null assignment")]
    public void Antiforgery_SetNull_ShouldThrow()
    {
        IHttpContext context = new TestHttpContext();

        Should.Throw<ArgumentNullException>(() => context.Antiforgery = null);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - RequireAntiforgery: Should throw before set")]
    public void RequireAntiforgery_BeforeSet_ShouldThrow()
    {
        IHttpContext context = new TestHttpContext();

        Should.Throw<InvalidOperationException>(() => _ = context.RequireAntiforgery);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - RequireAntiforgery: Should return the service after set")]
    public void RequireAntiforgery_AfterSet_ShouldReturnService()
    {
        IHttpContext context = new TestHttpContext();
        IHttpAntiforgery service = HttpAntiforgery.Create();
        context.Antiforgery = service;

        context.RequireAntiforgery.ShouldBeSameAs(service);
    }

    [Fact(DisplayName = "Cohesion Test [Http.Antiforgery] - Antiforgery: Should throw on a null context")]
    public void Antiforgery_OnNullContext_ShouldThrow()
    {
        IHttpContext context = null!;

        Should.Throw<ArgumentNullException>(() => _ = context.Antiforgery);
    }
}
```

## Walkthrough

- **Covered behavior** — Antiforgery: Should return null before a feature is installed.
- **Covered behavior** — Antiforgery: Should round-trip through the setter.
- **Covered behavior** — Antiforgery: Should install an antiforgery feature on set.
- **Covered behavior** — Antiforgery: Should reject a null assignment.
- **Covered behavior** — RequireAntiforgery: Should throw before set.
- **Covered behavior** — RequireAntiforgery: Should return the service after set.
- **Covered behavior** — Antiforgery: Should throw on a null context.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/tests/HttpContextAntiforgeryExtensionsTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/tests/Assimalign.Cohesion.Http.Antiforgery.Tests.csproj`.
