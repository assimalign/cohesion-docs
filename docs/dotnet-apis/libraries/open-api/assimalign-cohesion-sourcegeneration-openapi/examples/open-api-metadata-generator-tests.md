# Example: Open Api Metadata Generator Tests

Exercise Open Api Metadata Generator behavior using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

This is the complete `OpenApiMetadataGeneratorTests.cs` listing from the generator test project. Keep it
in that project when running it: the project supplies its package references, generated sources, and
any shared fixtures.

## Code

```csharp
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

using Shouldly;
using Xunit;

namespace Assimalign.Cohesion.SourceGeneration.OpenApi.Tests;

public class OpenApiMetadataGeneratorTests
{
    private static (string GeneratedSource, ImmutableArray<Diagnostic> GeneratorDiagnostics, ImmutableArray<Diagnostic> CompilationErrors) Run(string source)
    {
        var run = OpenApiGeneratorHarness.Run("GeneratorTests", source);
        return (run.GeneratedSource, run.GeneratorDiagnostics, run.Errors);
    }

    private const string ValidApi = """
        using Assimalign.Cohesion.OpenApi;
        using Assimalign.Cohesion.OpenApi.Attributes;

        [OpenApiSchema(Description = "A pet.")]
        public sealed class Pet
        {
            [OpenApiSchemaProperty(Required = true, SchemaType = OpenApiSchemaKind.Integer, Format = "int64")]
            public long Id { get; set; }

            [OpenApiSchemaProperty(SchemaType = OpenApiSchemaKind.String)]
            public string Name { get; set; }
        }

        [OpenApiTag("pets", Description = "Pet operations")]
        public static class PetApi
        {
            [OpenApiOperation(OperationType.Get, "/pets/{id}", OperationId = "getPet", Tags = new[] { "pets" })]
            [OpenApiParameter("id", ParameterLocation.Path, Required = true, SchemaType = OpenApiSchemaKind.Integer)]
            [OpenApiResponse(200, Description = "The pet", ContentType = "application/json", ModelType = typeof(Pet))]
            [OpenApiResponse(404, Description = "Not found")]
            public static void GetPet() { }
        }
        """;

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: a valid API emits an advertised provider and an internal registry that compile")]
    public void Generator_ValidApi_EmitsAdvertisedProviderAndInternalRegistry()
    {
        // Act
        var (generated, generatorDiagnostics, compileErrors) = Run(ValidApi);

        // Assert
        generatorDiagnostics.ShouldBeEmpty();
        compileErrors.ShouldBeEmpty(compileErrors.Length == 0 ? "" : string.Join("; ", compileErrors.Select(d => d.GetMessage())));

        generated.ShouldContain("[assembly: global::Assimalign.Cohesion.OpenApi.Attributes.OpenApiMetadataProviderAttribute(typeof(global::Assimalign.Cohesion.OpenApi.Generated.OpenApiMetadataProvider_GeneratorTests))]", Case.Sensitive);
        generated.ShouldContain("public sealed class OpenApiMetadataProvider_GeneratorTests : global::Assimalign.Cohesion.OpenApi.Attributes.IOpenApiMetadataProvider", Case.Sensitive);
        generated.ShouldContain("internal static class OpenApiMetadataRegistry", Case.Sensitive);
        generated.ShouldNotContain("public static class OpenApiMetadataRegistry", Case.Sensitive);
        generated.ShouldContain("Path = \"/pets/{id}\"", Case.Sensitive);
        generated.ShouldContain("OperationId = \"getPet\"", Case.Sensitive);
        generated.ShouldContain("#/components/schemas/Pet", Case.Sensitive);
        generated.ShouldContain("OpenApiSchemaMetadata", Case.Sensitive);
        generated.ShouldContain("Name = \"pets\"", Case.Sensitive);
    }

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: an ambiguous body schema reports a diagnostic")]
    public void Generator_AmbiguousSchema_ReportsDiagnostic()
    {
        const string source = """
            using Assimalign.Cohesion.OpenApi;
            using Assimalign.Cohesion.OpenApi.Attributes;

            public sealed class Pet { }

            public static class Api
            {
                [OpenApiOperation(OperationType.Get, "/pets")]
                [OpenApiResponse(200, Description = "ok", ModelType = typeof(Pet), SchemaReference = "#/components/schemas/Other")]
                public static void Get() { }
            }
            """;

        var (_, generatorDiagnostics, _) = Run(source);

        generatorDiagnostics.ShouldContain(d => d.Id == "OPENAPIATTR0002" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: a non-required path parameter reports a warning")]
    public void Generator_PathParameterNotRequired_ReportsWarning()
    {
        const string source = """
            using Assimalign.Cohesion.OpenApi;
            using Assimalign.Cohesion.OpenApi.Attributes;

            public static class Api
            {
                [OpenApiOperation(OperationType.Get, "/pets/{id}")]
                [OpenApiParameter("id", ParameterLocation.Path)]
                public static void Get() { }
            }
            """;

        var (_, generatorDiagnostics, _) = Run(source);

        generatorDiagnostics.ShouldContain(d => d.Id == "OPENAPIATTR0003" && d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: an empty operation path reports an error")]
    public void Generator_EmptyPath_ReportsError()
    {
        const string source = """
            using Assimalign.Cohesion.OpenApi;
            using Assimalign.Cohesion.OpenApi.Attributes;

            public static class Api
            {
                [OpenApiOperation(OperationType.Get, "")]
                public static void Get() { }
            }
            """;

        var (_, generatorDiagnostics, _) = Run(source);

        generatorDiagnostics.ShouldContain(d => d.Id == "OPENAPIATTR0001" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: an incomplete API key scheme reports an error")]
    public void Generator_IncompleteApiKey_ReportsError()
    {
        const string source = """
            using Assimalign.Cohesion.OpenApi;
            using Assimalign.Cohesion.OpenApi.Attributes;

            [assembly: OpenApiSecurityScheme("key", SecuritySchemeType.ApiKey)]
            """;

        var (_, generatorDiagnostics, _) = Run(source);

        generatorDiagnostics.ShouldContain(d => d.Id == "OPENAPIATTR0006" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact(DisplayName = "Cohesion Test [SourceGeneration.OpenApi] - Generator: a project with no attributes emits nothing")]
    public void Generator_NoAttributes_EmitsNothing()
    {
        var (generated, generatorDiagnostics, compileErrors) = Run("public class Empty { }");

        generated.ShouldBeEmpty();
        generatorDiagnostics.ShouldBeEmpty();
        compileErrors.ShouldBeEmpty();
    }
}
```

## Walkthrough

- **Test entry point** — `Generator_ValidApi_EmitsAdvertisedProviderAndInternalRegistry` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Generator_AmbiguousSchema_ReportsDiagnostic` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Generator_PathParameterNotRequired_ReportsWarning` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Generator_EmptyPath_ReportsError` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Generator_IncompleteApiKey_ReportsError` contains the setup, invocation, and assertions for this case.
- **Test entry point** — `Generator_NoAttributes_EmitsNothing` contains the setup, invocation, and assertions for this case.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.OpenApi/tests/OpenApiMetadataGeneratorTests.cs`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.OpenApi/tests/Assimalign.Cohesion.SourceGeneration.OpenApi.Tests.csproj`.
