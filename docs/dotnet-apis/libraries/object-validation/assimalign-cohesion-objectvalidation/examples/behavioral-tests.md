# Example: Validator Factory Builder Tests

Exercise Validator Factory Builder behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ValidatorFactoryBuilderTests.cs` listing from the package test project. Keep
it in that project when running it: the project supplies its package references, generated sources,
and any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Assimalign.Cohesion.ObjectValidation.Tests;

public class ValidatorFactoryBuilderTests
{
    [Fact]
    public void TestFactoryBuilder()
    {
        var factory = ValidatorFactoryBuilder.Create(builder =>
        {
            builder.AddValidator("default", builder =>
            {
                builder.AddProfile<ProfileBuilderTest>();
            });
        });


        var validator = factory.CreateValidator("default");

        var result = validator.Validate(new Test());

        Assert.False(result.IsValid);
    }

    private partial class Test
    {
        public string FirstName { get; set; }
    }
    public class ProfileBuilderTest : ValidationProfileBuilder
    {
        protected override void OnBuild(IValidationProfileBuilder builder)
        {
            builder.CreateProfile<Test>(descriptor =>
            {
                descriptor.RuleFor(p => p.FirstName)
                    .NotNull()
                    .NotEmpty();
            });
        }
    }
}
```

## Walkthrough

- **Test entry point** — `TestFactoryBuilder` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/BuilderTests/ValidatorFactoryBuilderTests.cs`.
- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/tests/Assimalign.Cohesion.ObjectValidation.Tests.csproj`.
