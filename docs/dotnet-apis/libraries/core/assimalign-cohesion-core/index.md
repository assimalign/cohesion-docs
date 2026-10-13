# Assimalign.Cohesion.Core

Provides the foundational primitives, frozen `COHESION_*` runtime contract, typed `System.Uri`
endpoints, environment helpers, glob support, and low-level extensions shared across Cohesion.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`Glob`](glob.md)** — type reference.

[Core](../index.md)

## Scope

Core stays free of hosting, configuration, logging, and dependency-injection references. The frozen
runtime contract belongs to `AppEnvironment.Variables` so gateways and resources share one vocabulary.
Its project also delivers the component-integration analyzer to package consumers.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.SourceGeneration`](../assimalign-cohesion-sourcegeneration/index.md) | `CohesionAnalyzerReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AdaptiveMemoryPool` | `src/System/Buffers/AdaptiveMemoryPool.cs` |
| `AdaptiveMemoryPoolOptions` | `src/System/Buffers/AdaptiveMemoryPoolOptions.cs` |
| `AdaptiveMemoryPoolPressurePolicy` | `src/System/Buffers/AdaptiveMemoryPoolPressurePolicy.cs` |
| `AdaptiveMemoryPoolSnapshot` | `src/System/Buffers/AdaptiveMemoryPoolSnapshot.cs` |
| `AppEnvironment` | `src/AppEnvironment.cs` |
| `AppEnvironment.Keys` | `src/AppEnvironment.Keys.cs` |
| `AppEnvironment.Variables` | `src/AppEnvironment.Variables.cs` |
| `AsyncExtensions` | `src/System/Threading/Tasks/AsyncExtensions.cs` |
| `Glob` | `src/System/IO/Glob.Tokens.cs` |
| `Size` | `src/System/Size.cs` |
| `UriExtensions` | `src/System/UriExtensions.cs` |
| `ArgumentExceptionExtensions` | `src/System/ExceptionExtensions.cs` |
| `ArgumentNullExceptionExtensions` | `src/System/ExceptionExtensions.cs` |
| `ArgumentOutOfRangeExceptionExtensions` | `src/System/ExceptionExtensions.cs` |
| `ComponentIntegrationAttribute` | `src/ComponentModel/ComponentIntegrationAttribute.cs` |
| `DirectoryName` | `src/System/IO/DirectoryName.cs` |
| `FileSystemPath` | `src/System/IO/FileSystemPath.cs` |
| `Either<T1, T2, T3, T4, T5>` | `src/System/SumTypes/Either.T5.cs` |

## Sources

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/Assimalign.Cohesion.Core.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Core/README.md`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Buffers/AdaptiveMemoryPool.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Buffers/AdaptiveMemoryPoolOptions.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Buffers/AdaptiveMemoryPoolPressurePolicy.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Buffers/AdaptiveMemoryPoolSnapshot.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/AppEnvironment.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/AppEnvironment.Keys.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/AppEnvironment.Variables.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Threading/Tasks/AsyncExtensions.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/IO/Glob.Tokens.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/Size.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/UriExtensions.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/ExceptionExtensions.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/ComponentModel/ComponentIntegrationAttribute.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/IO/DirectoryName.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/IO/FileSystemPath.cs`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/System/SumTypes/Either.T5.cs`.
