# Assimalign.Cohesion.OpenApi.Fluent

Builds OpenAPI documents through version-targeted fluent builders.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Concrete builders use nested callbacks to author the model and return a document from `Build()`.
Version-gated operations reject unsupported constructs at authoring time. The package needs the
model, not serialization or service integration.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiDocumentBuilder` | `src/OpenApiDocumentBuilder.cs` |
| `OpenApiCallbackBuilder` | `src/Builders/OpenApiCallbackBuilder.cs` |
| `OpenApiComponentsBuilder` | `src/Builders/OpenApiComponentsBuilder.cs` |
| `OpenApiExampleBuilder` | `src/Builders/OpenApiExampleBuilder.cs` |
| `OpenApiInfoBuilder` | `src/Builders/OpenApiInfoBuilder.cs` |
| `OpenApiLinkBuilder` | `src/Builders/OpenApiLinkBuilder.cs` |
| `OpenApiMediaTypeBuilder` | `src/Builders/OpenApiMediaTypeBuilder.cs` |
| `OpenApiOperationBuilder` | `src/Builders/OpenApiOperationBuilder.cs` |
| `OpenApiParameterBuilder` | `src/Builders/OpenApiParameterBuilder.cs` |
| `OpenApiPathItemBuilder` | `src/Builders/OpenApiPathItemBuilder.cs` |
| `OpenApiRequestBodyBuilder` | `src/Builders/OpenApiRequestBodyBuilder.cs` |
| `OpenApiResponseBuilder` | `src/Builders/OpenApiResponseBuilder.cs` |
| `OpenApiSchemaBuilder` | `src/Builders/OpenApiSchemaBuilder.cs` |
| `OpenApiSecuritySchemeBuilder` | `src/Builders/OpenApiSecuritySchemeBuilder.cs` |
| `OpenApiTagBuilder` | `src/Builders/OpenApiTagBuilder.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Assimalign.Cohesion.OpenApi.Fluent.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/OpenApiDocumentBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiCallbackBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiComponentsBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiExampleBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiInfoBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiLinkBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiMediaTypeBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiOperationBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiParameterBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiPathItemBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiRequestBodyBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiResponseBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiSchemaBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiSecuritySchemeBuilder.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Builders/OpenApiTagBuilder.cs`.
