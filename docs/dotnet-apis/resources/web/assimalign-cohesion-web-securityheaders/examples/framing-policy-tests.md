# Framing Policy Tests

This example exercises `Assimalign.Cohesion.Web.SecurityHeaders` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/FramingPolicyTests.cs`. It
retains the test class and assertions so the setup, operation, and expected outcome stay together.
Use it in the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — Framing: Deny and SameOrigin should pair both fields.
- **Case 2** — Framing: AllowFrom should list the ancestors and emit no X-Frame-Options.
- **Case 3** — Framing: AllowFrom with only 'self' should be SameOrigin.
- **Case 4** — Framing: A value outside the ancestor-source grammar should be rejected.
- **Case 5** — Framing: AllowFrom should require at least one ancestor.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Web.SecurityHeaders.Tests;

/// <summary>
/// The framing policy: the paired <c>frame-ancestors</c> and <c>X-Frame-Options</c> values, and the
/// ancestor-source checks of <see cref="FramingPolicy.AllowFrom"/>.
/// </summary>
public class FramingPolicyTests
{
    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Framing: Deny and SameOrigin should pair both fields")]
    public void Presets_DenyAndSameOrigin_ShouldPairFrameAncestorsWithFrameOptions()
    {
        // Arrange & Act & Assert
        FramingPolicy.Deny.FrameAncestors.ShouldBe("'none'");
        FramingPolicy.Deny.XFrameOptions.ShouldBe("DENY");
        FramingPolicy.SameOrigin.FrameAncestors.ShouldBe("'self'");
        FramingPolicy.SameOrigin.XFrameOptions.ShouldBe("SAMEORIGIN");
    }

    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Framing: AllowFrom should list the ancestors and emit no X-Frame-Options")]
    public void AllowFrom_Ancestors_ShouldEmitNoFrameOptions()
    {
        // Arrange & Act
        FramingPolicy policy = FramingPolicy.AllowFrom("'SELF'", "https://partner.example.com", "https://*.example.org:8443", "https:");

        // Assert
        policy.FrameAncestors.ShouldBe("'self' https://partner.example.com https://*.example.org:8443 https:");
        policy.XFrameOptions.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Framing: AllowFrom with only 'self' should be SameOrigin")]
    public void AllowFrom_OnlySelf_ShouldReturnSameOrigin()
    {
        // Arrange & Act
        FramingPolicy policy = FramingPolicy.AllowFrom("'self'", "'self'");

        // Assert
        policy.ShouldBeSameAs(FramingPolicy.SameOrigin);
    }

    [Theory(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Framing: A value outside the ancestor-source grammar should be rejected")]
    [InlineData("'none'")]
    [InlineData("'unsafe-inline'")]
    [InlineData("'nonce-abc'")]
    [InlineData("")]
    [InlineData("https://partner.example.com; script-src *")]
    [InlineData("self")]
    public void AllowFrom_InvalidAncestor_ShouldThrow(string source)
    {
        // Arrange & Act
        Action act = () => FramingPolicy.AllowFrom(source);

        // Assert
        act.ShouldThrow<ArgumentException>();
    }

    [Fact(DisplayName = "Cohesion Test [Web.SecurityHeaders] - Framing: AllowFrom should require at least one ancestor")]
    public void AllowFrom_NoAncestor_ShouldThrow()
    {
        // Arrange & Act
        Action empty = () => FramingPolicy.AllowFrom();
        Action nullList = () => FramingPolicy.AllowFrom(null!);

        // Assert
        empty.ShouldThrow<ArgumentException>();
        nullList.ShouldThrow<ArgumentNullException>();
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/FramingPolicyTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/tests/Assimalign.Cohesion.Web.SecurityHeaders.Tests.csproj`.
