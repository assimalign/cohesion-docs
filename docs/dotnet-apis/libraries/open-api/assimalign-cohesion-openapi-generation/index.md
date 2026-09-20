# Assimalign.Cohesion.OpenApi.Generation

Produces OpenAPI documents from collected intermediate metadata.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Generation consumes explicit input and target-version options. Metadata can come from generated
registries or attribute mapping, so document assembly does not require runtime reflection discovery.
The resulting document remains a normal version-aware model.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Attributes`](../../open-api/assimalign-cohesion-openapi-attributes/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiDocumentGenerator` | `src/OpenApiDocumentGenerator.cs` |
| `OpenApiGenerationInput` | `src/OpenApiGenerationInput.cs` |
| `OpenApiGenerationOptions` | `src/OpenApiGenerationOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/Assimalign.Cohesion.OpenApi.Generation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/OpenApiDocumentGenerator.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/OpenApiGenerationInput.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/OpenApiGenerationOptions.cs`.
