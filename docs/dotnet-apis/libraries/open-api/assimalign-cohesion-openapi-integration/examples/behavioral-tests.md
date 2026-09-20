# Example: Open Api Api Manager Integration Tests

Exercise Open Api Api Manager Integration behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `OpenApiApiManagerIntegrationTests.cs` listing from the package test project.
Keep it in that project when running it: the project supplies its package references, generated
sources, and any shared fixtures. The using block below makes the test-framework import explicit
where the original project supplies it globally.

## Code

```csharp
using Shouldly;
using Xunit;
using Assimalign.Cohesion.OpenApi.Validation;
using Assimalign.Cohesion.OpenApi.Versioning;

namespace Assimalign.Cohesion.OpenApi.Integration.Tests;

public class OpenApiApiManagerIntegrationTests
{
    private const string SampleJson = """
        {"openapi":"3.1.2","info":{"title":"Imported","version":"1.0.0"},"webhooks":{"onEvent":{"post":{"responses":{"200":{"description":"ok"}}}}},"paths":{"/ping":{"get":{"operationId":"ping","responses":{"200":{"description":"pong"}}}}}}
        """;

    [Fact(DisplayName = "Cohesion Test [OpenApi.Integration] - ApiManager: import parses JSON and YAML into the same model")]
    public void ApiManager_Import_JsonAndYaml()
    {
        var importer = OpenApiIntegration.CreateImporter();
        var exporter = OpenApiIntegration.CreateExporter();

        var fromJson = importer.Import(SampleJson, OpenApiFormat.Json);
        var yaml = exporter.Export(fromJson, OpenApiFormat.Yaml);
        var fromYaml = importer.Import(yaml, OpenApiFormat.Yaml);

        fromJson.Info.Title.ShouldBe("Imported");
        fromYaml.Paths!.Items.ShouldContainKey("/ping");
        fromYaml.Validate().IsValid.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [OpenApi.Integration] - ApiManager: export retargets and reports lossy diagnostics")]
    public void ApiManager_Export_RetargetsWithDiagnostics()
    {
        var importer = OpenApiIntegration.CreateImporter();
        var exporter = OpenApiIntegration.CreateExporter();

        var document = importer.Import(SampleJson, OpenApiFormat.Json);
        document.SpecVersion.ShouldBe(OpenApiSpecVersion.V3_1);

        var result = exporter.Export(document, OpenApiFormat.Json, OpenApiSpecVersion.V3_0);

        result.Content.ShouldContain("\"openapi\": \"3.0.4\"", Case.Sensitive);
        result.Content.ShouldNotContain("webhooks", Case.Sensitive);
        result.Diagnostics.ShouldContain(d =>
            d.Code == OpenApiTransformDiagnosticCodes.UnsupportedConstruct && d.Location.StartsWith("#/webhooks"));
    }

    [Fact(DisplayName = "Cohesion Test [OpenApi.Integration] - ApiManager: export at the same line raises no diagnostics")]
    public void ApiManager_Export_SameVersion_NoDiagnostics()
    {
        var importer = OpenApiIntegration.CreateImporter();
        var exporter = OpenApiIntegration.CreateExporter();

        var document = importer.Import(SampleJson, OpenApiFormat.Json);
        var result = exporter.Export(document, OpenApiFormat.Yaml, OpenApiSpecVersion.V3_1);

        result.Diagnostics.ShouldBeEmpty();
        result.Content.ShouldContain("openapi:", Case.Sensitive);
    }
}
```

## Walkthrough

- **Covered behavior** — ApiManager: import parses JSON and YAML into the same model.
- **Covered behavior** — ApiManager: export retargets and reports lossy diagnostics.
- **Covered behavior** — ApiManager: export at the same line raises no diagnostics.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/tests/OpenApiApiManagerIntegrationTests.cs`.
- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/tests/Assimalign.Cohesion.OpenApi.Integration.Tests.csproj`.
