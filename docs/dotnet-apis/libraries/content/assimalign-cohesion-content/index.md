# Assimalign.Cohesion.Content

Defines format-neutral content identity, stream ownership, and reader and writer contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

Content contracts carry metadata and ownership without parsing formats or coupling to storage
services. `ContentFactory` creates byte-backed, stream-backed, writable, and composite content.
Format packages implement the reader/writer seams while keeping borrowed-stream lifetime explicit.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `ContentException` | `src/Exceptions/ContentException.cs` |
| `ContentFactory` | `src/ContentFactory.cs` |
| `ContentFormat` | `src/ContentFormat.cs` |
| `ContentFormatException` | `src/Exceptions/ContentFormatException.cs` |
| `ContentKind` | `src/ContentKind.cs` |
| `IComposableContent` | `src/Abstractions/IComposableContent.cs` |
| `IContent` | `src/Abstractions/IContent.cs` |
| `IContentReader<TDocument>` | `src/Abstractions/IContentReader.cs` |
| `IContentWriter<TDocument>` | `src/Abstractions/IContentWriter.cs` |
| `IWritableContent` | `src/Abstractions/IWritableContent.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Assimalign.Cohesion.Content.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Exceptions/ContentException.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/ContentFactory.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/ContentFormat.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Exceptions/ContentFormatException.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/ContentKind.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Abstractions/IComposableContent.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Abstractions/IContent.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Abstractions/IContentReader.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Abstractions/IContentWriter.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content/src/Abstractions/IWritableContent.cs`.
