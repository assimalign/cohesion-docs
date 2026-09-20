# Assimalign.Cohesion.Content.Mkv

Reserves the Matroska content package.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

The only implementation is an empty internal segment type. The current reference to BMFF is
inherited scaffolding, not a Matroska format rule. A public parser, writer, and defined
malformed-input behavior are absent, and the fixture test is skipped.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content.Bmff`](../../content/assimalign-cohesion-content-bmff/index.md) | `CohesionProjectReference` |

## Principal public types

No implemented public type declarations were found in the project's retained source files.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src/Assimalign.Cohesion.Content.Mkv.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src`.
