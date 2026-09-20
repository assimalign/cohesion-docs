# Assimalign.Cohesion.OpenApi.Integration

Connects endpoint metadata and document import and export to the OpenAPI family.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Endpoint sources feed description providers without introducing a Web or ApiManager dependency.
Import/export implementations compose serialization and version transforms. Retargeted exports
report losses instead of silently claiming equivalent documents.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Attributes`](../../open-api/assimalign-cohesion-openapi-attributes/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Generation`](../../open-api/assimalign-cohesion-openapi-generation/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Serialization`](../../open-api/assimalign-cohesion-openapi-serialization/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Versioning`](../../open-api/assimalign-cohesion-openapi-versioning/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IOpenApiDescriptionProvider` | `src/Abstractions/IOpenApiDescriptionProvider.cs` |
| `IOpenApiDocumentExporter` | `src/Abstractions/IOpenApiDocumentExporter.cs` |
| `IOpenApiDocumentImporter` | `src/Abstractions/IOpenApiDocumentImporter.cs` |
| `IOpenApiEndpointSource` | `src/Abstractions/IOpenApiEndpointSource.cs` |
| `OpenApiDescriptionInfo` | `src/OpenApiDescriptionInfo.cs` |
| `OpenApiExportResult` | `src/OpenApiExportResult.cs` |
| `OpenApiFormat` | `src/OpenApiFormat.cs` |
| `OpenApiIntegration` | `src/OpenApiIntegration.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Assimalign.Cohesion.OpenApi.Integration.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Abstractions/IOpenApiDescriptionProvider.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Abstractions/IOpenApiDocumentExporter.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Abstractions/IOpenApiDocumentImporter.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Abstractions/IOpenApiEndpointSource.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/OpenApiDescriptionInfo.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/OpenApiExportResult.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/OpenApiFormat.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/OpenApiIntegration.cs`.
