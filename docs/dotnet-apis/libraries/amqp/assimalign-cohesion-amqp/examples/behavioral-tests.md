# Example: behavioral tests

Complete test listings that show the `Assimalign.Cohesion.Amqp` API in use.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

Each listing is a complete file from the package test project. Keep it in that project when running it: the project supplies its package references and the test doubles the listings use, which are not shipped APIs.

## AMQP array tests

This is the complete `AmqpArrayTests.cs` listing from the package test project. It covers `AmqpArray` and `AmqpPrimitiveType`: the element type and descriptor an array keeps, the checks the model applies when one is built, and how the primitive types are numbered.

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Amqp.Tests;

public class AmqpArrayTests
{
    public static TheoryData<object, object> Descriptors => new()
    {
        { new AmqpSymbol("x-opt-a"), new AmqpSymbol("x-opt-a") },
        { 0x77UL, 0x77UL },
        { "x-opt-b", new AmqpSymbol("x-opt-b") }
    };

    public static TheoryData<string, object> InvalidDescriptors => new()
    {
        { "an int", 7 },
        { "a long", 7L },
        { "a uuid", Guid.Empty },
        { "a default symbol, which holds no name", default(AmqpSymbol) }
    };

    [Theory(DisplayName = "Cohesion Test [Amqp] - Constructor: Should keep an array's element type and a symbol or ulong descriptor, converting a string to a symbol")]
    [MemberData(nameof(Descriptors))]
    public void Constructor_WithValidDescriptor_ShouldKeepDescriptor(object descriptor, object expected)
    {
        // Act
        AmqpArray array = new(AmqpPrimitiveType.String, ["a", "b"], descriptor);

        // Assert
        array.ElementType.ShouldBe(AmqpPrimitiveType.String);
        array.Descriptor.ShouldBe(expected);
        array.Elements.ShouldBe(["a", "b"]);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Constructor: Should leave the descriptor null when the elements are not described")]
    public void Constructor_WithoutDescriptor_ShouldHaveNoDescriptor()
    {
        // Act
        AmqpArray array = new(AmqpPrimitiveType.UByte, [(byte)1]);

        // Assert
        array.Descriptor.ShouldBeNull();
        array.Elements.ShouldHaveSingleItem().ShouldBe((byte)1);
    }

    [Theory(DisplayName = "Cohesion Test [Amqp] - Constructor: Should refuse a descriptor that is neither a symbol nor an unsigned long")]
    [MemberData(nameof(InvalidDescriptors))]
    public void Constructor_WithInvalidDescriptor_ShouldThrowArgumentException(string scenario, object descriptor)
    {
        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new AmqpArray(AmqpPrimitiveType.Int, [1], descriptor), scenario);

        // Assert
        exception.ParamName.ShouldBe("descriptor");
        exception.Message.ShouldStartWith(ErrorMessages.DescriptorTypeInvalid);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Constructor: Should refuse a string descriptor that is not seven-bit ASCII")]
    public void Constructor_WithNonAsciiStringDescriptor_ShouldThrowArgumentException()
    {
        // Act
        ArgumentException exception = Should.Throw<ArgumentException>(() => new AmqpArray(AmqpPrimitiveType.Int, [1], "café"));

        // Assert
        exception.ParamName.ShouldBe("descriptor");
        exception.Message.ShouldStartWith(ErrorMessages.SymbolInvalid);
    }

    [Theory(DisplayName = "Cohesion Test [Amqp] - Constructor: Should refuse an element type that names no AMQP type")]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(-1)]
    public void Constructor_WithUndefinedElementType_ShouldThrowArgumentOutOfRangeException(int elementType)
    {
        // Act
        ArgumentOutOfRangeException exception = Should.Throw<ArgumentOutOfRangeException>(() => new AmqpArray((AmqpPrimitiveType)elementType, []));

        // Assert
        exception.ParamName.ShouldBe("elementType");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Constructor: Should refuse null elements")]
    public void Constructor_WithNullElements_ShouldThrowArgumentNullException()
    {
        // Act
        ArgumentNullException exception = Should.Throw<ArgumentNullException>(() => new AmqpArray(AmqpPrimitiveType.Int, null!));

        // Assert
        exception.ParamName.ShouldBe("elements");
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Constructor: Should copy the elements, so a later change to the source leaves the array as it was")]
    public void Constructor_WithElements_ShouldCopyThem()
    {
        // Arrange
        List<object?> source = [1, 2];

        // Act
        AmqpArray array = new(AmqpPrimitiveType.Int, source);
        source[0] = 3;
        source.Add(4);

        // Assert
        array.Elements.ShouldBe([1, 2]);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Elements: Should be read-only")]
    public void Elements_OnArray_ShouldBeReadOnly()
    {
        // Arrange
        AmqpArray array = new(AmqpPrimitiveType.Int, [1, 2]);

        // Act
        IList<object?> elements = array.Elements.ShouldBeAssignableTo<IList<object?>>().ShouldNotBeNull();

        // Assert
        elements.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(() => elements[0] = 3);
        array.Elements.ShouldBe([1, 2]);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - AmqpPrimitiveType: Should number each type as its section in Part 1, section 1.6, with no type at zero")]
    public void AmqpPrimitiveType_OnEveryValue_ShouldNumberItsSection()
    {
        // Act
        int[] values = [.. Enum.GetValues<AmqpPrimitiveType>().Select(value => (int)value)];

        // Assert
        values.ShouldBe(Enumerable.Range(1, 24));
        ((int)AmqpPrimitiveType.Null).ShouldBe(1);
        ((int)AmqpPrimitiveType.Array).ShouldBe(24);
        Enum.IsDefined(default(AmqpPrimitiveType)).ShouldBeFalse();
    }
}
```

### Walkthrough

- **Covered behavior** — Constructor: Should keep an array's element type and a symbol or ulong descriptor, converting a string to a symbol.
- **Covered behavior** — Constructor: Should leave the descriptor null when the elements are not described.
- **Covered behavior** — Constructor: Should refuse a descriptor that is neither a symbol nor an unsigned long.
- **Covered behavior** — Constructor: Should refuse a string descriptor that is not seven-bit ASCII.
- **Covered behavior** — Constructor: Should refuse an element type that names no AMQP type.
- **Covered behavior** — Constructor: Should refuse null elements.
- **Covered behavior** — Constructor: Should copy the elements, so a later change to the source leaves the array as it was.
- **Covered behavior** — Elements: Should be read-only.
- **Covered behavior** — AmqpPrimitiveType: Should number each type as its section in Part 1, section 1.6, with no type at zero.

The assertions define the expected result or failure boundary.

## Link resumption tests

This is the complete `AmqpUnsettledReconciliationTests.cs` listing from the package test project. It covers `AmqpUnsettledReconciliation`, which compares the unsettled maps of a resumed link and says, delivery by delivery, what each end may do.

### Code

```csharp
using System.Collections.Generic;
using System.Linq;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Amqp.Tests;

/// <summary>
/// Pins the comparison of a resumed link's two unsettled maps (AMQP 1.0 Part 2, section 2.6.13), including the
/// worked example of Part 3, section 3.4.6.
/// </summary>
public class AmqpUnsettledReconciliationTests
{
    public static TheoryData<bool, bool> IncompleteFlags => new()
    {
        { false, false },
        { true, false },
        { false, true },
        { true, true }
    };

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should return one empty, complete comparison when neither map lists a delivery")]
    public void Create_OnNoUnsettledDeliveries_ShouldReturnEmptyCompleteComparison()
    {
        // Act
        AmqpUnsettledReconciliation none = AmqpUnsettledReconciliation.Create(null, false, null, false);
        AmqpUnsettledReconciliation empty = AmqpUnsettledReconciliation.Create(Map(), false, Map(), false);

        // Assert
        empty.ShouldBeSameAs(none);
        none.IsComplete.ShouldBeTrue();
        none.SourceIncomplete.ShouldBeFalse();
        none.TargetIncomplete.ShouldBeFalse();
        none.Resumable.ShouldBeEmpty();
        none.Settled.ShouldBeEmpty();
        none.UnknownToReceiver.ShouldBeEmpty();
        none.InDoubt.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should partition the unsettled maps of the Part 3 resumption example")]
    public void Create_OnPart3Example_ShouldPartitionTags()
    {
        // Arrange: the maps of AMQP 1.0 Part 3, section 3.4.6.
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map(
            (1, null), (2, null), (3, null), (4, null),
            (5, new AmqpReceived(0, 200)), (6, new AmqpReceived(1, 150)), (7, new AmqpReceived(0, 500)),
            (8, new AmqpReceived(3, 5)), (9, new AmqpReceived(2, 0)),
            (10, AmqpAccepted.Instance), (11, AmqpAccepted.Instance), (12, AmqpAccepted.Instance),
            (13, AmqpAccepted.Instance), (14, AmqpAccepted.Instance));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map(
            (2, new AmqpReceived(3, 0)), (3, AmqpAccepted.Instance), (4, null), (6, new AmqpReceived(2, 0)),
            (7, new AmqpReceived(0, 100)), (8, AmqpAccepted.Instance), (9, null), (11, new AmqpReceived(1, 2000)),
            (12, AmqpAccepted.Instance), (13, AmqpReleased.Instance), (14, null));

        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, false, target, false);

        // Assert
        reconciliation.IsComplete.ShouldBeTrue();
        Numbers(reconciliation.Resumable).ShouldBe([2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 14]);
        Numbers(reconciliation.UnknownToReceiver).ShouldBe([1, 5, 10]);
        reconciliation.Settled.ShouldBeEmpty();
        reconciliation.InDoubt.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should consider settled a delivery only the target lists while the source's map is complete")]
    public void Create_OnTargetOnlyTagWithCompleteSource_ShouldConsiderItSettled()
    {
        // Arrange
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map((1, null));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map((1, null), (2, AmqpAccepted.Instance));

        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, false, target, false);

        // Assert
        Numbers(reconciliation.Resumable).ShouldBe([1]);
        Numbers(reconciliation.Settled).ShouldBe([2]);
        reconciliation.UnknownToReceiver.ShouldBeEmpty();
        reconciliation.InDoubt.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should keep resumable every delivery the target lists while the source's map is incomplete")]
    public void Create_OnIncompleteSourceMap_ShouldKeepTargetTagsResumable()
    {
        // Arrange
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map((1, null));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map((2, null), (3, new AmqpReceived(0, 10)));

        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, true, target, false);

        // Assert
        reconciliation.SourceIncomplete.ShouldBeTrue();
        reconciliation.TargetIncomplete.ShouldBeFalse();
        reconciliation.IsComplete.ShouldBeFalse();
        Numbers(reconciliation.Resumable).ShouldBe([2, 3]);
        Numbers(reconciliation.UnknownToReceiver).ShouldBe([1]);
        reconciliation.Settled.ShouldBeEmpty();
        reconciliation.InDoubt.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should hold in doubt a delivery only the source lists while the target's map is incomplete")]
    public void Create_OnIncompleteTargetMap_ShouldHoldSourceOnlyTagsInDoubt()
    {
        // Arrange
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map((1, null), (2, null));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map((2, null), (3, null));

        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, false, target, true);

        // Assert
        reconciliation.SourceIncomplete.ShouldBeFalse();
        reconciliation.TargetIncomplete.ShouldBeTrue();
        reconciliation.IsComplete.ShouldBeFalse();
        Numbers(reconciliation.Resumable).ShouldBe([2]);
        Numbers(reconciliation.Settled).ShouldBe([3]);
        Numbers(reconciliation.InDoubt).ShouldBe([1]);
        reconciliation.UnknownToReceiver.ShouldBeEmpty();
    }

    [Theory(DisplayName = "Cohesion Test [Amqp] - Create: Should report incomplete maps even when neither lists a delivery")]
    [MemberData(nameof(IncompleteFlags))]
    public void Create_OnIncompleteMapsWithoutTags_ShouldReportThem(bool sourceIncomplete, bool targetIncomplete)
    {
        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(null, sourceIncomplete, null, targetIncomplete);

        // Assert
        reconciliation.SourceIncomplete.ShouldBe(sourceIncomplete);
        reconciliation.TargetIncomplete.ShouldBe(targetIncomplete);
        reconciliation.IsComplete.ShouldBe(!sourceIncomplete && !targetIncomplete);
        reconciliation.Resumable.ShouldBeEmpty();
        reconciliation.Settled.ShouldBeEmpty();
        reconciliation.UnknownToReceiver.ShouldBeEmpty();
        reconciliation.InDoubt.ShouldBeEmpty();
    }

    [Theory(DisplayName = "Cohesion Test [Amqp] - Create: Should place every tag either map lists in exactly one set")]
    [MemberData(nameof(IncompleteFlags))]
    public void Create_OnOverlappingMaps_ShouldPlaceEveryTagInOneSet(bool sourceIncomplete, bool targetIncomplete)
    {
        // Arrange
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map((1, null), (2, null), (3, null), (4, AmqpAccepted.Instance));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map((3, null), (4, AmqpAccepted.Instance), (5, null), (6, null));

        // Act
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, sourceIncomplete, target, targetIncomplete);
        AmqpDeliveryTag[] placed =
        [
            .. reconciliation.Resumable,
            .. reconciliation.Settled,
            .. reconciliation.UnknownToReceiver,
            .. reconciliation.InDoubt
        ];

        // Assert
        placed.Length.ShouldBe(6);
        placed.Distinct().Count().ShouldBe(6);
        placed.ShouldBe(source.Keys.Union(target.Keys), ignoreOrder: true);
        Numbers(reconciliation.Resumable).ShouldBe(sourceIncomplete ? [3, 4, 5, 6] : [3, 4]);
    }

    [Fact(DisplayName = "Cohesion Test [Amqp] - Create: Should not change when a map changes after the comparison")]
    public void Create_OnMapChangedAfterComparison_ShouldKeepItsSets()
    {
        // Arrange
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> source = Map((1, null));
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> target = Map((1, null));
        AmqpUnsettledReconciliation reconciliation = AmqpUnsettledReconciliation.Create(source, false, target, false);

        // Act
        source.Remove(AmqpDeliveryTag.Create(1UL));
        target[AmqpDeliveryTag.Create(2UL)] = null;

        // Assert
        Numbers(reconciliation.Resumable).ShouldBe([1]);
        reconciliation.Settled.ShouldBeEmpty();
    }

    private static Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> Map(params (ulong Tag, AmqpDeliveryState? State)[] entries)
    {
        Dictionary<AmqpDeliveryTag, AmqpDeliveryState?> map = [];

        foreach ((ulong tag, AmqpDeliveryState? state) in entries)
        {
            map.Add(AmqpDeliveryTag.Create(tag), state);
        }

        return map;
    }

    private static ulong[] Numbers(IEnumerable<AmqpDeliveryTag> tags)
    {
        return [.. tags.Select(tag => (ulong)tag.Value.Span[0]).Order()];
    }
}
```

### Walkthrough

- **Covered behavior** — Create: Should return one empty, complete comparison when neither map lists a delivery.
- **Covered behavior** — Create: Should partition the unsettled maps of the Part 3 resumption example.
- **Covered behavior** — Create: Should consider settled a delivery only the target lists while the source's map is complete.
- **Covered behavior** — Create: Should keep resumable every delivery the target lists while the source's map is incomplete.
- **Covered behavior** — Create: Should hold in doubt a delivery only the source lists while the target's map is incomplete.
- **Covered behavior** — Create: Should report incomplete maps even when neither lists a delivery.
- **Covered behavior** — Create: Should place every tag either map lists in exactly one set.
- **Covered behavior** — Create: Should not change when a map changes after the comparison.

The assertions define the expected result or failure boundary.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/tests/AmqpArrayTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/tests/AmqpUnsettledReconciliationTests.cs`.
- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/tests/Assimalign.Cohesion.Amqp.Tests.csproj`.
