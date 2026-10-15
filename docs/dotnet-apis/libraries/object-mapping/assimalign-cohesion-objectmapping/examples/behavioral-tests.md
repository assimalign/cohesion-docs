# Example: Mapper Extensions Tests

Exercise Mapper Extensions behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `MapperExtensionsTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.ObjectMapping.Tests;

public class MapperExtensionsTests
{
    private static Mapper PersonMapper()
    {
        return new MapperBuilder()
            .AddProfile<PersonTarget, PersonSource>(descriptor => descriptor
                .MapMember(target => target.FirstName, source => source.FirstName)
                .MapMember(target => target.LastName, source => source.LastName)
                .MapMember(target => target.Age, source => source.Age))
            .Build();
    }

    [Fact]
    public void Map_CreateNewTargetFromSource_ReturnsPopulatedTarget()
    {
        // Arrange
        var mapper = PersonMapper();

        // Act
        var result = mapper.Map<PersonTarget, PersonSource>(new PersonSource { FirstName = "Jo", Age = 10 });

        // Assert
        result.FirstName.ShouldBe("Jo");
        result.Age.ShouldBe(10);
    }

    [Fact]
    public void Map_OntoExistingTarget_ReturnsSameInstance()
    {
        // Arrange
        var mapper = PersonMapper();
        var target = new PersonTarget();

        // Act
        var result = mapper.Map(target, new PersonSource { FirstName = "Jo" });

        // Assert
        result.ShouldBeSameAs(target);
        result.FirstName.ShouldBe("Jo");
    }

    [Fact]
    public void Map_WithRuntimeTypes_CreatesAndPopulatesTarget()
    {
        // Arrange
        var mapper = PersonMapper();

        // Act
        object result = mapper.Map(new PersonSource { FirstName = "Jo" }, typeof(PersonTarget), typeof(PersonSource));

        // Assert
        result.ShouldBeOfType<PersonTarget>();
        ((PersonTarget)result).FirstName.ShouldBe("Jo");
    }

    [Fact]
    public void Map_MultipleSources_ComposeIntoOneTarget()
    {
        // Arrange
        var mapper = new MapperBuilder()
            .AddProfile<PersonTarget, NameSource>(descriptor => descriptor
                .MapMember(target => target.FirstName, source => source.FirstName)
                .MapMember(target => target.LastName, source => source.LastName))
            .AddProfile<PersonTarget, AgeSource>(descriptor => descriptor
                .MapMember(target => target.Age, source => source.Age))
            .Build();

        // Act
        var result = mapper.Map<PersonTarget>(new object[]
        {
            new NameSource { FirstName = "Chase", LastName = "Crawford" },
            new AgeSource { Age = 25 }
        });

        // Assert
        result.FirstName.ShouldBe("Chase");
        result.LastName.ShouldBe("Crawford");
        result.Age.ShouldBe(25);
    }

    [Fact]
    public void Map_SequenceOfSources_ReturnsSequenceOfTargets()
    {
        // Arrange
        var mapper = PersonMapper();
        var sources = new[]
        {
            new PersonSource { FirstName = "A" },
            new PersonSource { FirstName = "B" }
        };

        // Act
        var results = mapper.Map<PersonTarget, PersonSource>(sources).ToList();

        // Assert
        results.Count.ShouldBe(2);
        results[0].FirstName.ShouldBe("A");
        results[1].FirstName.ShouldBe("B");
    }

    [Fact]
    public void IsMatch_WhenTypesMatch_ReturnsTrue()
    {
        // Arrange
        var profile = PersonMapper().Profiles[0];

        // Act / Assert
        profile.IsMatch(typeof(PersonTarget), typeof(PersonSource)).ShouldBeTrue();
    }

    [Fact]
    public void IsMatch_WhenTypesDiffer_ReturnsFalse()
    {
        // Arrange
        var profile = PersonMapper().Profiles[0];

        // Act / Assert
        profile.IsMatch(typeof(PersonTarget), typeof(AgeSource)).ShouldBeFalse();
    }
}
```

## Walkthrough

- **Test entry point** — `Map_CreateNewTargetFromSource_ReturnsPopulatedTarget` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Map_OntoExistingTarget_ReturnsSameInstance` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Map_WithRuntimeTypes_CreatesAndPopulatesTarget` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Map_MultipleSources_ComposeIntoOneTarget` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Map_SequenceOfSources_ReturnsSequenceOfTargets` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `IsMatch_WhenTypesMatch_ReturnsTrue` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `IsMatch_WhenTypesDiffer_ReturnsFalse` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/tests/MapperExtensionsTests.cs`.
- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/tests/Assimalign.Cohesion.ObjectMapping.Tests.csproj`.
