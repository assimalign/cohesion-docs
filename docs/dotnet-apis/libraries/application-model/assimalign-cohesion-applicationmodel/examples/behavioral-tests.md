# Example: Cohesion Application Attribute Tests

Exercise Cohesion Application Attribute behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `CohesionApplicationAttributeTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using System;
using Shouldly;
using Xunit;
using Assimalign.Cohesion.ApplicationModel;

namespace Assimalign.Cohesion.ApplicationModel.Tests;

public class CohesionApplicationAttributeTests
{
    [Fact(DisplayName = "Cohesion Test [ApplicationModel] - Cohesion application metadata preserves its name")]
    public void Constructor_Name_PreservesName()
    {
        var attribute = new CohesionApplicationAttribute("appa");

        attribute.Name.ShouldBe("appa");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel] - Cohesion application metadata rejects a null name")]
    public void Constructor_NullName_Throws()
    {
        ArgumentNullException error = Should.Throw<ArgumentNullException>(
            () => new CohesionApplicationAttribute(null!));

        error.ParamName.ShouldBe("name");
    }

    [Theory(DisplayName = "Cohesion Test [ApplicationModel] - Cohesion application metadata rejects a blank name")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_BlankName_Throws(string name)
    {
        ArgumentException error = Should.Throw<ArgumentException>(
            () => new CohesionApplicationAttribute(name));

        error.ParamName.ShouldBe("name");
    }

    [Fact(DisplayName = "Cohesion Test [ApplicationModel] - Cohesion application metadata is assembly-only and singular")]
    public void AttributeUsage_IsAssemblyOnlyAndSingular()
    {
        AttributeUsageAttribute? usage = Attribute.GetCustomAttribute(
            typeof(CohesionApplicationAttribute),
            typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

        usage.ShouldNotBeNull();
        usage.ValidOn.ShouldBe(AttributeTargets.Assembly);
        usage.AllowMultiple.ShouldBeFalse();
        usage.Inherited.ShouldBeFalse();
    }
}
```

## Walkthrough

- **Covered behavior** — Cohesion application metadata preserves its name.
- **Covered behavior** — Cohesion application metadata rejects a null name.
- **Covered behavior** — Cohesion application metadata rejects a blank name.
- **Covered behavior** — Cohesion application metadata is assembly-only and singular.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/tests/CohesionApplicationAttributeTests.cs`.
- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/tests/Assimalign.Cohesion.ApplicationModel.Tests.csproj`.
