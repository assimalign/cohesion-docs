# Route Precedence Tests

This example exercises `Assimalign.Cohesion.Web.Routing` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/tests/RoutePrecedenceTests.cs`. It retains
the test class and assertions so the setup, operation, and expected outcome stay together. `Use` it in
the source project’s test context, with its test dependencies and supporting test objects.

## Behavior exercised

- **Case 1** — InboundPrecedence: Should compute documented values.
- **Case 2** — InboundPrecedence: Specificity orders literal < constrained < unconstrained < catch-all.

## Source example

```csharp
using System.Globalization;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.Web.Routing.Patterns;

namespace Assimalign.Cohesion.Web.Routing.Tests;

public class RoutePrecedenceTests
{
    [Theory(DisplayName = "Cohesion Test [Web.Routing] - InboundPrecedence: Should compute documented values")]
    [InlineData("/api/status", "1.1")]
    [InlineData("/api/{id:int}", "1.2")]
    [InlineData("/api/{id}", "1.3")]
    [InlineData("/api/template/{id:int}", "1.12")]
    [InlineData("/api/template/{id}", "1.13")]
    public void InboundPrecedence_ShouldMatchDocumentedValues(string pattern, string expected)
    {
        // Arrange
        decimal expectedPrecedence = decimal.Parse(expected, CultureInfo.InvariantCulture);

        // Act
        RoutePattern routePattern = RoutePatternParser.Parse(pattern);

        // Assert
        routePattern.InboundPrecedence.ShouldBe(expectedPrecedence);
    }

    [Fact(DisplayName = "Cohesion Test [Web.Routing] - InboundPrecedence: Specificity orders literal < constrained < unconstrained < catch-all")]
    public void InboundPrecedence_ShouldOrderBySpecificity()
    {
        // Arrange
        decimal literal = RoutePatternParser.Parse("/api/status").InboundPrecedence;
        decimal constrained = RoutePatternParser.Parse("/api/{id:int}").InboundPrecedence;
        decimal unconstrained = RoutePatternParser.Parse("/api/{id}").InboundPrecedence;
        decimal catchAll = RoutePatternParser.Parse("/api/{**path}").InboundPrecedence;

        // Assert — lower precedence is more specific and is matched first.
        literal.ShouldBeLessThan(constrained);
        constrained.ShouldBeLessThan(unconstrained);
        unconstrained.ShouldBeLessThan(catchAll);
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/tests/RoutePrecedenceTests.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/tests/Assimalign.Cohesion.Web.Routing.Tests.csproj`.
