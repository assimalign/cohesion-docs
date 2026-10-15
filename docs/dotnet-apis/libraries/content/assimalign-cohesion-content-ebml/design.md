# Assimalign.Cohesion.Content.Ebml design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Ebml`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The project contains document, element, stream, and variable-integer types without a separate
overview or design record. The area README identifies the implementation as requiring repair. Its
current project file has no Cohesion references, so the intended root-contract integration is not an
implemented dependency.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/Assimalign.Cohesion.Content.Ebml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src`.
