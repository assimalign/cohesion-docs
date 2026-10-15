# Sql Schema Expression Tests

This example exercises `Assimalign.Cohesion.Database.Sql` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSchemaExpressionTests.cs`.
It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Schema: Sum retains analyzable selector and predicate expressions.
- **Case 2** — Schema: Sum rejects null expressions.

## Source example

```csharp
using System;
using System.Linq.Expressions;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Sql.Tests;

public class SqlSchemaExpressionTests
{
    [Fact(DisplayName = "Cohesion Test [Database.Sql] - Schema: Sum retains analyzable selector and predicate expressions")]
    public void Sum_WithSelectorAndPredicate_ShouldRetainExpressionShape()
    {
        Expression<Func<OrderLine, object?>> selector = line => line.Quantity * line.UnitPrice;
        Expression<Func<OrderLine, bool>> predicate = line => line.OrderId == 42;

        ISqlAggregateExpression expression = Sql.Sum(selector, predicate);

        expression.SourceType.ShouldBe(typeof(OrderLine));
        expression.Selector.ShouldBeSameAs(selector);
        expression.Predicate.ShouldBeSameAs(predicate);
        expression.Selector.Parameters.ShouldHaveSingleItem().Type.ShouldBe(typeof(OrderLine));
        expression.Predicate.Parameters.ShouldHaveSingleItem().Type.ShouldBe(typeof(OrderLine));
    }

    [Fact(DisplayName = "Cohesion Test [Database.Sql] - Schema: Sum rejects null expressions")]
    public void Sum_WithNullExpression_ShouldRejectDeclaration()
    {
        Expression<Func<OrderLine, object?>> selector = line => line.Quantity;
        Expression<Func<OrderLine, bool>> predicate = line => line.OrderId == 42;

        Should.Throw<ArgumentNullException>(() => Sql.Sum<OrderLine>(null!, predicate));
        Should.Throw<ArgumentNullException>(() => Sql.Sum<OrderLine>(selector, null!));
    }

    private sealed record OrderLine(long Id, long OrderId, int Quantity, decimal UnitPrice);
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSchemaExpressionTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/Assimalign.Cohesion.Database.Sql.Tests.csproj`.
