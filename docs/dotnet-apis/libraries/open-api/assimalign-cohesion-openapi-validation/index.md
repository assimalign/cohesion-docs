# Assimalign.Cohesion.OpenApi.Validation

Validates OpenAPI structure, semantics, and version placement.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Diagnostics have stable codes and severity rather than requiring callers to interpret exception
text. Rule contracts allow additional validators to compose with the default rule set. Version
placement follows the root capability matrix.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.OpenApi.Serialization`](../../open-api/assimalign-cohesion-openapi-serialization/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IOpenApiValidationRule` | `src/IOpenApiValidationRule.cs` |
| `IOpenApiValidator` | `src/IOpenApiValidator.cs` |
| `OpenApiDiagnostic` | `src/OpenApiDiagnostic.cs` |
| `OpenApiDiagnosticSeverity` | `src/OpenApiDiagnosticSeverity.cs` |
| `OpenApiValidation` | `src/OpenApiValidation.cs` |
| `OpenApiValidationContext` | `src/OpenApiValidationContext.cs` |
| `OpenApiValidationResult` | `src/OpenApiValidationResult.cs` |
| `OpenApiValidationRuleCodes` | `src/OpenApiValidationRuleCodes.cs` |
| `OpenApiDocumentValidationExtensions` | `src/Extensions/OpenApiDocumentValidationExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/Assimalign.Cohesion.OpenApi.Validation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/IOpenApiValidationRule.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/IOpenApiValidator.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiDiagnostic.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiDiagnosticSeverity.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiValidation.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiValidationContext.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiValidationResult.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/OpenApiValidationRuleCodes.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/Extensions/OpenApiDocumentValidationExtensions.cs`.
