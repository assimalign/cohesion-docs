# Assimalign.Cohesion.OpenApi

Defines a version-aware OpenAPI document model.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

`OpenApiVersionCapabilities` centralizes field availability for the supported 3.0.4, 3.1.2, and
3.2.0 lines. The format-neutral node tree carries extension and example values. Serialization and
service-runtime dependencies stay outside the root model.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiComponents` | `src/Components/OpenApiComponents.cs` |
| `OpenApiDocument` | `src/Documents/OpenApiDocument.cs` |
| `OpenApiInfo` | `src/Documents/OpenApiInfo.cs` |
| `OpenApiNode` | `src/Nodes/OpenApiNode.cs` |
| `OpenApiOperation` | `src/Paths/OpenApiOperation.cs` |
| `OpenApiParameter` | `src/Parameters/OpenApiParameter.cs` |
| `OpenApiPaths` | `src/Paths/OpenApiPaths.cs` |
| `OpenApiRequestBody` | `src/Parameters/OpenApiRequestBody.cs` |
| `OpenApiResponse` | `src/Responses/OpenApiResponse.cs` |
| `OpenApiSchema` | `src/Schemas/OpenApiSchema.cs` |
| `OpenApiSecurityScheme` | `src/Security/OpenApiSecurityScheme.cs` |
| `OpenApiSpecVersion` | `src/Versioning/OpenApiSpecVersion.cs` |
| `OpenApiVersionCapabilities` | `src/Versioning/OpenApiVersionCapabilities.cs` |
| `SchemaType` | `src/Schemas/SchemaType.cs` |
| `IOpenApiElement` | `src/Abstractions/IOpenApiElement.cs` |
| `IOpenApiExtensible` | `src/Abstractions/IOpenApiExtensible.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Assimalign.Cohesion.OpenApi.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Components/OpenApiComponents.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Documents/OpenApiDocument.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Documents/OpenApiInfo.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Nodes/OpenApiNode.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Paths/OpenApiOperation.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Parameters/OpenApiParameter.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Paths/OpenApiPaths.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Parameters/OpenApiRequestBody.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Responses/OpenApiResponse.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Schemas/OpenApiSchema.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Security/OpenApiSecurityScheme.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Versioning/OpenApiSpecVersion.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Versioning/OpenApiVersionCapabilities.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Schemas/SchemaType.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Abstractions/IOpenApiElement.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Abstractions/IOpenApiExtensible.cs`.
