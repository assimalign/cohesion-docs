# Assimalign.Cohesion.OpenApi.Serialization

Reads and writes OpenAPI documents through JSON and YAML representations.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Serialization maps between the canonical model and a format-neutral node tree. Target-version
capabilities govern emitted fields. JSON uses the base class library and YAML uses `Content.Yaml`,
preserving the service-free model boundary.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Content.Yaml`](../../content/assimalign-cohesion-content-yaml/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IOpenApiReader` | `src/IOpenApiReader.cs` |
| `IOpenApiWriter` | `src/IOpenApiWriter.cs` |
| `OpenApiJson` | `src/OpenApiJson.cs` |
| `OpenApiWriterOptions` | `src/OpenApiWriterOptions.cs` |
| `OpenApiDocumentSerializationExtensions` | `src/Extensions/OpenApiDocumentSerializationExtensions.cs` |
| `OpenApiYaml` | `src/OpenApiYaml.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/Assimalign.Cohesion.OpenApi.Serialization.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/IOpenApiReader.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/IOpenApiWriter.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/OpenApiJson.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/OpenApiWriterOptions.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/Extensions/OpenApiDocumentSerializationExtensions.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/OpenApiYaml.cs`.
