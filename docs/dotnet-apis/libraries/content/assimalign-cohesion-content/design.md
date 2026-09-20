# Assimalign.Cohesion.Content design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Content contracts carry metadata and ownership without parsing formats or coupling to storage
services. `ContentFactory` creates byte-backed, stream-backed, writable, and composite content.
Format packages implement the reader/writer seams while keeping borrowed-stream lifetime explicit.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Assimalign.Cohesion.Content.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src`.
