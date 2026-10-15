# Assimalign.Cohesion.Content.Binary

Reserves shared binary-content contracts beneath concrete binary formats.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

`IBinaryFile` extends the root content contract, but the package still includes an empty `Class1`
scaffold. A complete bounded binary reader is not delivered by this project.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content`](../../content/assimalign-cohesion-content/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Class1` | `src/Class1.cs` |
| `IBinaryFile` | `src/Abstractions/IBinaryFile.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src/Assimalign.Cohesion.Content.Binary.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src/Class1.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src/Abstractions/IBinaryFile.cs`.
