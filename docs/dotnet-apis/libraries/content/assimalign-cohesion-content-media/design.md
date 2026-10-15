# Assimalign.Cohesion.Content.Media design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Media`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`IMediaFile` combines binary content and composition, with component and media-kind types alongside
it. These abstractions do not supply playback, codec decoding, or a complete media-processing
service.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content.Binary`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Assimalign.Cohesion.Content.Media.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src`.
