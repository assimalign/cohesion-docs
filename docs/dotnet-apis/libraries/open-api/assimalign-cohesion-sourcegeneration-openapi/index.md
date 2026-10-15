# Assimalign.Cohesion.SourceGeneration.OpenApi

A Roslyn incremental source generator that discovers the OpenApi authoring attributes at compile time
and emits the flat intermediate metadata as static data, so document generation needs no runtime
reflection. Invalid attribute combinations are reported as compiler diagnostics whose ids match the
runtime mapper's `OpenApiMetadataDiagnosticCodes`.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Metadata composes across assemblies. Each annotated assembly gets a public provider class, advertised to
referencing compilations with `[assembly: OpenApiMetadataProvider]`. Each compilation that declares
metadata or references an annotated assembly gets an internal `OpenApiMetadataRegistry` that combines
its own metadata with every provider its references advertise. An application with endpoints in several
libraries reads all of them from its own registry, with no runtime discovery.

It has no NuGet package of its own. It ships inside `Assimalign.Cohesion.OpenApi.Attributes`, under
`analyzers/dotnet/cs/`, so a project that references that package, directly or through
`OpenApi.Generation` or `OpenApi.Integration`, runs the generator with no extra wiring.

A project inside this repository references the OpenApi packages by project, and a project reference
does not carry an analyzer, so it activates the generator itself:

```xml
<ItemGroup>
  <CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.OpenApi" />
</ItemGroup>
```

Then read the combined metadata from the registry (namespace `Assimalign.Cohesion.OpenApi.Generated`)
and feed it to `Assimalign.Cohesion.OpenApi.Generation.OpenApiDocumentGenerator`:

```csharp
using Assimalign.Cohesion.OpenApi.Generated;
using Assimalign.Cohesion.OpenApi.Generation;

var input = new OpenApiGenerationInput
{
    Operations = OpenApiMetadataRegistry.Operations,
    Schemas = OpenApiMetadataRegistry.Schemas,
    Tags = OpenApiMetadataRegistry.Tags,
    SecuritySchemes = OpenApiMetadataRegistry.SecuritySchemes
};
```

The registry is internal, so each assembly reads its own. A project that is granted
`InternalsVisibleTo` by an annotated assembly also sees that assembly's registry; naming the registry
there reports CS0436 while binding the project's own, complete registry. See
[Design](design.md) ("Known limit").

- [Design](design.md) — the generator's pipeline, cross-assembly composition, emitted
  shape, caching design, and delivery

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiMetadataGenerator` | `src/OpenApiMetadataGenerator.cs` |

## Sources

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.OpenApi/README.md`.

- **Source** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.OpenApi/src/Assimalign.Cohesion.SourceGeneration.OpenApi.csproj`.
