# Assimalign.Cohesion.Content.Media

Reserves shared media contracts above binary content.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

`IMediaFile` combines binary content and composition, with component and media-kind types alongside
it. These abstractions do not supply playback, codec decoding, or a complete media-processing
service.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content.Binary`](../../content/assimalign-cohesion-content-binary/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `FPoint` | `src/ValueTypes/FPoint.cs` |
| `IMediaFile` | `src/Abstractions/IMediaFile.cs` |
| `IMediaFileComponent` | `src/Abstractions/IMediaFileComponent.cs` |
| `MediaFileKind` | `src/MediaFileKind.cs` |
| `StreamExtensions` | `src/Extensions/StreamExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Assimalign.Cohesion.Content.Media.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/ValueTypes/FPoint.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Abstractions/IMediaFile.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Abstractions/IMediaFileComponent.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/MediaFileKind.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Media/src/Extensions/StreamExtensions.cs`.
