# Example: Open Api End To End Tests

Exercise Open Api End To End behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `OpenApiEndToEndTests.cs` listing from the package test project. Keep it in
that project when running it: the project supplies its package references, generated sources, and
any shared fixtures. The using block below makes the test-framework import explicit where the
original project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;
using Assimalign.Cohesion.OpenApi.Serialization;

namespace Assimalign.Cohesion.OpenApi.Validation.Tests;

public class OpenApiEndToEndTests
{
    [Fact(DisplayName = "Cohesion Test [OpenApi] - EndToEnd: author, emit 3.1.2 JSON, reparse, validate clean")]
    public void Author_Emit_Reparse_Validate_IsClean()
    {
        // Arrange: author a document in the object model.
        var document = SampleDocuments.CreateValid(OpenApiSpecVersion.V3_1);

        // Act: emit JSON for 3.1.2, then read it back.
        var json = document.ToJson(OpenApiSpecVersion.V3_1);
        var reparsed = OpenApiJson.Parse(json);
        var result = reparsed.Validate();

        // Assert: the emitted document targets 3.1.2, round-trips, and validates clean.
        json.ShouldContain("\"openapi\": \"3.1.2\"", Case.Sensitive);
        reparsed.SpecVersion.ShouldBe(OpenApiSpecVersion.V3_1);
        reparsed.Paths!.Items.ShouldContainKey("/pets/{id}");
        result.IsValid.ShouldBeTrue();
        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Cohesion Test [OpenApi] - EndToEnd: emitting 3.0 from a 3.1 model adapts version-gated fields")]
    public void Emit_ThreeZero_FromModel_AdaptsVersionGatedFields()
    {
        var document = SampleDocuments.CreateValid(OpenApiSpecVersion.V3_1);
        document.Components!.Schemas["Pet"] = new OpenApiSchema { Type = SchemaType.String, Nullable = true };

        var json = document.ToJson(OpenApiSpecVersion.V3_0, indented: false);

        json.ShouldContain("\"openapi\":\"3.0.4\"", Case.Sensitive);
        json.ShouldContain("\"nullable\":true", Case.Sensitive);
    }
}
```

## Walkthrough

- **Covered behavior** — EndToEnd: author, emit 3.1.2 JSON, reparse, validate clean.
- **Covered behavior** — EndToEnd: emitting 3.0 from a 3.1 model adapts version-gated fields.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/tests/OpenApiEndToEndTests.cs`.
- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/tests/Assimalign.Cohesion.OpenApi.Validation.Tests.csproj`.
