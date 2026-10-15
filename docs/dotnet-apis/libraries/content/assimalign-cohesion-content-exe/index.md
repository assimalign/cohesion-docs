# Assimalign.Cohesion.Content.Exe

Reserves executable-content contracts over the binary-content layer.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

`IExecutableFile` declares invocation but the result type and implementation remain scaffolding. The
design establishes no executable parser, process-launch lifecycle, or meaningful invocation error
model. The empty test does not demonstrate execution support.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content.Binary`](../../content/assimalign-cohesion-content-binary/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Class1` | `src/Class1.cs` |
| `ExecutableResult` | `src/ExecutableResult.cs` |
| `IExecutableFile` | `src/Abstractions/IExecutableFile.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/Assimalign.Cohesion.Content.Exe.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/Class1.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/ExecutableResult.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Exe/src/Abstractions/IExecutableFile.cs`.
