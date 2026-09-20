# Assimalign.Cohesion.Content.Binary design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Binary`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`IBinaryFile` extends the root content contract, but the package still includes an empty `Class1`
scaffold. A complete bounded binary reader is not delivered by this project.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src/Assimalign.Cohesion.Content.Binary.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Binary/src`.
