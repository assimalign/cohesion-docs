# Transaction Snapshot Tests

This example exercises `Assimalign.Cohesion.Database.Transactions` through its co-located test source.

> **Status:** Partial.

The example reproduces
`cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionSnapshotTests.cs`
. It retains the test class and assertions so the setup, operation, and expected outcome stay
together. `Use` it in the source project’s test context, with its test dependencies and supporting
test objects.

## Behavior exercised

- **Case 1** — Snapshot: Own writes are always visible.
- **Case 2** — Snapshot: Writes at or after the maximum are invisible.
- **Case 3** — Snapshot: Writers below the minimum are visible.
- **Case 4** — Snapshot: In-flight writers between the bounds are invisible.
- **Case 5** — Snapshot: Minimum above maximum is rejected.

## Source example

```csharp
using System;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Database.Transactions.Tests;

public class TransactionSnapshotTests
{
    private static TransactionSnapshot CreateSnapshot(ulong owner, ulong minimum, ulong maximum, params ulong[] active)
    {
        var activeSequences = Array.ConvertAll(active, value => new TransactionSequence(value));
        return new TransactionSnapshot(
            new TransactionSequence(owner),
            new TransactionSequence(minimum),
            new TransactionSequence(maximum),
            activeSequences);
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Snapshot: Own writes are always visible")]
    public void IsVisible_OwnWrites_ShouldBeVisible()
    {
        // Arrange
        var snapshot = CreateSnapshot(owner: 10, minimum: 5, maximum: 11, active: 10);

        // Act
        var visible = snapshot.IsVisible(new TransactionSequence(10));

        // Assert
        visible.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Snapshot: Writes at or after the maximum are invisible")]
    public void IsVisible_WriterAtOrAboveMaximum_ShouldBeInvisible()
    {
        // Arrange
        var snapshot = CreateSnapshot(owner: 10, minimum: 5, maximum: 11);

        // Act & Assert
        snapshot.IsVisible(new TransactionSequence(11)).ShouldBeFalse();
        snapshot.IsVisible(new TransactionSequence(42)).ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Snapshot: Writers below the minimum are visible")]
    public void IsVisible_WriterBelowMinimum_ShouldBeVisible()
    {
        // Arrange
        var snapshot = CreateSnapshot(owner: 10, minimum: 5, maximum: 11, active: 7);

        // Act
        var visible = snapshot.IsVisible(new TransactionSequence(4));

        // Assert
        visible.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Snapshot: In-flight writers between the bounds are invisible")]
    public void IsVisible_ActiveWriterWithinBounds_ShouldBeInvisible()
    {
        // Arrange
        var snapshot = CreateSnapshot(owner: 10, minimum: 5, maximum: 11, active: 7);

        // Act & Assert
        snapshot.IsVisible(new TransactionSequence(7)).ShouldBeFalse();
        snapshot.IsVisible(new TransactionSequence(6)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [Database] - Snapshot: Minimum above maximum is rejected")]
    public void Constructor_MinimumAboveMaximum_ShouldThrow()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CreateSnapshot(owner: 1, minimum: 9, maximum: 3));
    }
}
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionSnapshotTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/Assimalign.Cohesion.Database.Transactions.Tests.csproj`.
