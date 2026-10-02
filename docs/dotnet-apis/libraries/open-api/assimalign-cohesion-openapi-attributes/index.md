# Assimalign.Cohesion.OpenApi.Attributes

Maps attribute declarations to intermediate OpenAPI metadata.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[OpenApi](../index.md)

## Scope

Attributes describe operations and schemas, while flat metadata records form the seam to generation.
The mapper reports invalid combinations. Compile-time discovery lives in the separate analyzer
project, keeping discovery out of the runtime model.

The package carries that analyzer, `Assimalign.Cohesion.OpenApi.SourceGeneration`, under
`analyzers/dotnet/cs/`. It runs at build time and is not a package dependency: a project that
references this package, directly or through `OpenApi.Generation` or `OpenApi.Integration`, gets an
internal `OpenApiMetadataRegistry` that combines its own annotated code with every annotated
assembly it references. A project inside the Cohesion repository references the package by project,
which carries no analyzer, so it adds
`<CohesionAnalyzerReference Include="Assimalign.Cohesion.OpenApi.SourceGeneration" />` itself.

Two public types carry metadata between assemblies (#1169): `IOpenApiMetadataProvider`, the
operations, schemas, tags and security schemes one assembly contributes, and
`[assembly: OpenApiMetadataProvider(typeof(T))]`, which advertises a provider type to every
compilation that references the assembly. The metadata records `OpenApiParameterMetadata`,
`OpenApiRequestBodyMetadata`, `OpenApiResponseMetadata` and `OpenApiSchemaMetadata` also carry an
optional `Schema`, a complete model schema that a runtime producer such as the Web OpenAPI adapter
passes through instead of the flat fields.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.OpenApi`](../../open-api/assimalign-cohesion-openapi/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.OpenApi.SourceGeneration` | `CohesionAnalyzerReference` |

## Principal public types

| Type | Source file |
|---|---|
| `OpenApiAttributeMapper` | `src/OpenApiAttributeMapper.cs` |
| `OpenApiSchemaKind` | `src/Attributes/OpenApiSchemaKind.cs` |
| `OpenApiExampleAttribute` | `src/Attributes/OpenApiExampleAttribute.cs` |
| `OpenApiExampleMetadata` | `src/Metadata/OpenApiExampleMetadata.cs` |
| `OpenApiMetadataDiagnostic` | `src/OpenApiMetadataDiagnostic.cs` |
| `OpenApiMetadataDiagnosticCodes` | `src/OpenApiMetadataDiagnosticCodes.cs` |
| `IOpenApiMetadataProvider` | `src/Abstractions/IOpenApiMetadataProvider.cs` |
| `OpenApiMetadataProviderAttribute` | `src/Attributes/OpenApiMetadataProviderAttribute.cs` |
| `OpenApiMetadataSeverity` | `src/OpenApiMetadataDiagnostic.cs` |
| `OpenApiOperationAttribute` | `src/Attributes/OpenApiOperationAttribute.cs` |
| `OpenApiOperationMetadata` | `src/Metadata/OpenApiOperationMetadata.cs` |
| `OpenApiParameterAttribute` | `src/Attributes/OpenApiParameterAttribute.cs` |
| `OpenApiParameterMetadata` | `src/Metadata/OpenApiParameterMetadata.cs` |
| `OpenApiRequestBodyAttribute` | `src/Attributes/OpenApiRequestBodyAttribute.cs` |
| `OpenApiRequestBodyMetadata` | `src/Metadata/OpenApiRequestBodyMetadata.cs` |
| `OpenApiResponseAttribute` | `src/Attributes/OpenApiResponseAttribute.cs` |
| `OpenApiResponseMetadata` | `src/Metadata/OpenApiResponseMetadata.cs` |
| `OpenApiSchemaAttribute` | `src/Attributes/OpenApiSchemaAttribute.cs` |

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Assimalign.Cohesion.OpenApi.Attributes.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/OpenApiAttributeMapper.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiSchemaKind.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiExampleAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Metadata/OpenApiExampleMetadata.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/OpenApiMetadataDiagnostic.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/OpenApiMetadataDiagnosticCodes.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Abstractions/IOpenApiMetadataProvider.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiMetadataProviderAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiOperationAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Metadata/OpenApiOperationMetadata.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiParameterAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Metadata/OpenApiParameterMetadata.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiRequestBodyAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Metadata/OpenApiRequestBodyMetadata.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiResponseAttribute.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Metadata/OpenApiResponseMetadata.cs`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Attributes/OpenApiSchemaAttribute.cs`.
