# Example: Http Session Options Tests

Exercise Http Session Options behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `HttpSessionOptionsTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Http.Tests;

public class HttpSessionOptionsTests
{
    [Fact]
    public void Defaults_ShouldMatchConventionalValues()
    {
        HttpSessionOptions options = new();

        options.CookieName.ShouldBe(".Cohesion.Session");
        options.CookiePath.ShouldBe("/");
        options.CookieHttpOnly.ShouldBeTrue();
        options.IdleTimeout.ShouldBe(TimeSpan.FromMinutes(20));
    }

    [Fact]
    public void Properties_ShouldBeMutable()
    {
        HttpSessionOptions options = new()
        {
            CookieName = "custom",
            CookiePath = "/app",
            CookieHttpOnly = false,
            IdleTimeout = TimeSpan.FromMinutes(5),
        };

        options.CookieName.ShouldBe("custom");
        options.CookiePath.ShouldBe("/app");
        options.CookieHttpOnly.ShouldBeFalse();
        options.IdleTimeout.ShouldBe(TimeSpan.FromMinutes(5));
    }
}
```

## Walkthrough

- **Test entry point** — `Defaults_ShouldMatchConventionalValues` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Properties_ShouldBeMutable` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/tests/HttpSessionOptionsTests.cs`.
- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/tests/Assimalign.Cohesion.Http.Sessions.Tests.csproj`.
