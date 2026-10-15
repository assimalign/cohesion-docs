# Example: Configuration Binding Attribute Tests

Exercise Configuration Binding Attribute behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ConfigurationBindingAttributeTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System;
using Xunit;

namespace Assimalign.Cohesion.Configuration.Tests;

public class ConfigurationBindingAttributeTests
{
    [Fact(DisplayName = "Cohesion Test [Configuration] - BindingAttribute: Type property is set")]
    public void BindingAttribute_Type_ShouldBeSet()
    {
        var attribute = new ConfigurationBindingAttribute(typeof(string));

        Assert.Equal(typeof(string), attribute.Type);
    }

    [Fact(DisplayName = "Cohesion Test [Configuration] - BindingAttribute: Generic version sets type")]
    public void BindingAttribute_Generic_ShouldSetType()
    {
        var attribute = new ConfigurationBindingAttribute<List<string>>();

        Assert.Equal(typeof(List<string>), attribute.Type);
    }

    [Fact(DisplayName = "Cohesion Test [Configuration] - BindingAttribute: Can be applied to property")]
    public void BindingAttribute_ShouldBeApplicableToProperty()
    {
        var attr = typeof(TestClass)
            .GetProperty(nameof(TestClass.Items))!
            .GetCustomAttributes(typeof(ConfigurationBindingAttribute), false);

        Assert.Single(attr);
        Assert.Equal(typeof(List<string>), ((ConfigurationBindingAttribute)attr[0]).Type);
    }

    private class TestClass
    {
        [ConfigurationBinding<List<string>>]
        public IEnumerable<string>? Items { get; set; }
    }
}
```

## Walkthrough

- **Covered behavior** — BindingAttribute: Type property is set.
- **Covered behavior** — BindingAttribute: Generic version sets type.
- **Covered behavior** — BindingAttribute: Can be applied to property.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/tests/ConfigurationBindingAttributeTests.cs`.
- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/tests/Assimalign.Cohesion.Configuration.Tests.csproj`.
