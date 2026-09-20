# OpenApi

Version-aware OpenAPI models, authoring, serialization, validation, and integration contracts.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.OpenApi` | Defines a version-aware OpenAPI document model. | [Overview](assimalign-cohesion-openapi/index.md) |
| `Assimalign.Cohesion.OpenApi.Attributes` | Maps attribute declarations to intermediate OpenAPI metadata. | [Overview](assimalign-cohesion-openapi-attributes/index.md) |
| `Assimalign.Cohesion.OpenApi.Fluent` | Builds OpenAPI documents through version-targeted fluent builders. | [Overview](assimalign-cohesion-openapi-fluent/index.md) |
| `Assimalign.Cohesion.OpenApi.Generation` | Produces OpenAPI documents from collected intermediate metadata. | [Overview](assimalign-cohesion-openapi-generation/index.md) |
| `Assimalign.Cohesion.OpenApi.Integration` | Connects endpoint metadata and document import and export to the OpenAPI family. | [Overview](assimalign-cohesion-openapi-integration/index.md) |
| `Assimalign.Cohesion.OpenApi.Serialization` | Reads and writes OpenAPI documents through JSON and YAML representations. | [Overview](assimalign-cohesion-openapi-serialization/index.md) |
| `Assimalign.Cohesion.OpenApi.Validation` | Validates OpenAPI structure, semantics, and version placement. | [Overview](assimalign-cohesion-openapi-validation/index.md) |
| `Assimalign.Cohesion.OpenApi.Versioning` | Transforms OpenAPI documents between supported specification versions. | [Overview](assimalign-cohesion-openapi-versioning/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.OpenApi` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.OpenApi.Attributes` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Fluent` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Generation` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Attributes` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Integration` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Attributes` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Generation` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Serialization` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Versioning` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Serialization` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference), `Assimalign.Cohesion.Content.Yaml` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Validation` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Serialization` (CohesionProjectReference) |
| `Assimalign.Cohesion.OpenApi.Versioning` | `Assimalign.Cohesion.OpenApi` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Serialization` (CohesionProjectReference), `Assimalign.Cohesion.OpenApi.Validation` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Assimalign.Cohesion.OpenApi.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Assimalign.Cohesion.OpenApi.Attributes.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Assimalign.Cohesion.OpenApi.Fluent.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/Assimalign.Cohesion.OpenApi.Generation.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Assimalign.Cohesion.OpenApi.Integration.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/Assimalign.Cohesion.OpenApi.Serialization.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/Assimalign.Cohesion.OpenApi.Validation.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/Assimalign.Cohesion.OpenApi.Versioning.csproj`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src`.
