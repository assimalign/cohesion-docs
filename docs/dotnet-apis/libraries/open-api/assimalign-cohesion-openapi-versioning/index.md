# Assimalign.Cohesion.OpenApi.Versioning

Transforms OpenAPI documents between supported specification versions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Transforms create a separate document and report construct changes and untranslatable content.
Serialization supplies deep-copy behavior and validation analyzes target-version fit. Callers must
inspect diagnostics before treating a retargeted document as equivalent.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Serialization`](../../open-api/assimalign-cohesion-openapi-serialization/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Validation`](../../open-api/assimalign-cohesion-openapi-validation/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiTransformDiagnostic` | `src/OpenApiTransformDiagnostic.cs` |
| `OpenApiTransformDiagnosticCodes` | `src/OpenApiTransformDiagnosticCodes.cs` |
| `OpenApiTransformResult` | `src/OpenApiTransformResult.cs` |
| `OpenApiVersionTransformer` | `src/OpenApiVersionTransformer.cs` |
| `OpenApiDocumentTransformExtensions` | `src/Extensions/OpenApiDocumentTransformExtensions.cs` |
| `OpenApiTransformSeverity` | `src/OpenApiTransformDiagnostic.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/Assimalign.Cohesion.OpenApi.Versioning.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/OpenApiTransformDiagnostic.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/OpenApiTransformDiagnosticCodes.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/OpenApiTransformResult.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/OpenApiVersionTransformer.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/Extensions/OpenApiDocumentTransformExtensions.cs`.
