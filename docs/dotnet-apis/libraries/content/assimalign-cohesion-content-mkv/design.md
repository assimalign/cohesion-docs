# Assimalign.Cohesion.Content.Mkv design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Mkv`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The only implementation is an empty internal segment type. The current reference to BMFF is
inherited scaffolding, not a Matroska format rule. A public parser, writer, and defined
malformed-input behavior are absent, and the fixture test is skipped.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content.Bmff`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src/Assimalign.Cohesion.Content.Mkv.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Mkv/src`.
