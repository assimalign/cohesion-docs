# Assimalign.Cohesion.ObjectMapping

Maps objects through explicit reusable profiles.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ObjectMapping](../index.md)

## Scope

Profiles record ordered actions and the mapper replays matching source/target mappings. The
low-level operation is target-first to support multiple sources merging into an existing target.
Scalar access uses compiled getters and setters; that implementation detail is not a blanket
reflection-free construction guarantee.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.SourceGeneration` | `CohesionAnalyzerReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IMapper` | `src/Abstractions/IMapper.cs` |
| `IMapperAction` | `src/Abstractions/IMapperAction.cs` |
| `IMapperBuilder` | `src/Abstractions/IMapperBuilder.cs` |
| `IMapperContext` | `src/Abstractions/IMapperContext.cs` |
| `IMapperFactory` | `src/Abstractions/IMapperFactory.cs` |
| `IMapperFactoryBuilder` | `src/Abstractions/IMapperFactoryBuilder.cs` |
| `IMapperProfile` | `src/Abstractions/IMapperProfile.cs` |
| `Mapper` | `src/Mapper.cs` |
| `MapperBuilder` | `src/MapperBuilder.cs` |
| `MapperCollectionHandling` | `src/ValueTypes/MapperCollectionHandling.cs` |
| `MapperContext` | `src/MapperContext.cs` |
| `MapperException` | `src/Exceptions/MapperException.cs` |
| `MapperExtensions` | `src/Extensions/MapperExtensions.cs` |
| `MapperFactoryBuilder` | `src/MapperFactoryBuilder.cs` |
| `MapperIgnoreHandling` | `src/ValueTypes/MapperIgnoreHandling.cs` |
| `MapperOptions` | `src/MapperOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Assimalign.Cohesion.ObjectMapping.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapper.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperAction.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperBuilder.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperContext.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperFactory.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Abstractions/IMapperProfile.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Mapper.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/MapperBuilder.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/ValueTypes/MapperCollectionHandling.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/MapperContext.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Exceptions/MapperException.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/Extensions/MapperExtensions.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/MapperFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/ValueTypes/MapperIgnoreHandling.cs`.

- **Source** — `cohesion/libraries/ObjectMapping/Assimalign.Cohesion.ObjectMapping/src/MapperOptions.cs`.
