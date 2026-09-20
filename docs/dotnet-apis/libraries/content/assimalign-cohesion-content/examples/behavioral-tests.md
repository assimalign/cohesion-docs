# Example: Content Format Tests

Exercise Content Format behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `ContentFormatTests.cs` listing from the package test project. Keep it in that
project when running it: the project supplies its package references, generated sources, and any
shared fixtures. The using block below makes the test-framework import explicit where the original
project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.Content.Tests;

public class ContentFormatTests
{
    [Fact(DisplayName = "Cohesion Test [Content] - Format: descriptor carries name, kind, and associations")]
    public void ContentFormat_Descriptor_CarriesMetadata()
    {
        var format = new ContentFormat
        {
            Name = "YAML",
            Kind = ContentKind.Document,
            MediaTypes = ["application/yaml", "text/yaml"],
            FileExtensions = [".yaml", ".yml"],
            Specification = "https://yaml.org/spec/1.2.2/"
        };

        format.Name.ShouldBe("YAML");
        format.Kind.ShouldBe(ContentKind.Document);
        format.MediaTypes.ShouldBe(["application/yaml", "text/yaml"]);
        format.FileExtensions.ShouldBe([".yaml", ".yml"]);
        format.Specification.ShouldBe("https://yaml.org/spec/1.2.2/");
        format.ToString().ShouldBe("YAML");
    }

    [Fact(DisplayName = "Cohesion Test [Content] - Format: the unknown descriptor is a shared default")]
    public void ContentFormat_Unknown_IsSharedDefault()
    {
        ContentFormat.Unknown.ShouldBeSameAs(ContentFormat.Unknown);
        ContentFormat.Unknown.Kind.ShouldBe(ContentKind.Unknown);

        using var content = ContentFactory.FromBytes(new byte[] { 1 });
        content.Format.ShouldBeSameAs(ContentFormat.Unknown);
    }

    [Fact(DisplayName = "Cohesion Test [Content] - Format: format exception carries the failure position")]
    public void ContentFormatException_Position_IsPreserved()
    {
        var exception = new ContentFormatException("Unexpected token.", position: 42);

        exception.Position.ShouldBe(42);
        exception.ShouldBeAssignableTo<ContentException>();
    }
}
```

## Walkthrough

- **Covered behavior** — Format: descriptor carries name, kind, and associations.
- **Covered behavior** — Format: the unknown descriptor is a shared default.
- **Covered behavior** — Format: format exception carries the failure position.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/tests/ContentFormatTests.cs`.
- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/tests/Assimalign.Cohesion.Content.Tests.csproj`.
