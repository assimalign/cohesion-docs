# Assimalign.Cohesion.Content.Bmff

Contains an incomplete ISO base media file-format box model and reader and writer surface.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

Explicit box factories separate container structure from codecs. Many box and visitor operations
remain unimplemented, and the reader's unknown-box error is not a stable malformed-input contract.
Disposing the default reader closes its stream; the media fixture test requires a developer-local
file.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content.Media`](../../content/assimalign-cohesion-content-media/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AdditionalMetaBox` | `src/Boxes/BmffBox.AdditionalMeta.cs` |
| `BinaryXmlBox` | `src/Boxes/BmffBox.BinaryXml.cs` |
| `BmffBox` | `src/BmffBox.cs` |
| `BmffBoxComposite` | `src/BmffBoxComposite.cs` |
| `BmffBoxType` | `src/BmffBoxType.cs` |
| `BmffBoxVisitor<T>` | `src/BmffBoxVisitor.cs` |
| `BmffReader` | `src/BmffReader.cs` |
| `BmffStream` | `src/BmffStream.cs` |
| `BmffVersion` | `src/ValueObjects/BmffVersion.cs` |
| `BmffWriter` | `src/BmffWriter.cs` |
| `ChunkOffset64BitBox` | `src/Boxes/BmffBox.ChunkOffset64Bit.cs` |
| `ChunkOffsetBox` | `src/Boxes/BmffBox.ChunkOffset.cs` |
| `CompactSampleSizeBox` | `src/Boxes/BmffBox.SampleSizeCompact.cs` |
| `CopyrightBox` | `src/Boxes/BmffBox.Copyright.cs` |
| `DataInfoBox` | `src/Boxes/BmffBox.DataInfo.cs` |
| `DataReferenceBox` | `src/Boxes/BmffBox.DataReference.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Assimalign.Cohesion.Content.Bmff.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.AdditionalMeta.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.BinaryXml.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffBox.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffBoxComposite.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffBoxType.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffBoxVisitor.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffReader.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffStream.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/ValueObjects/BmffVersion.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/BmffWriter.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.ChunkOffset64Bit.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.ChunkOffset.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.SampleSizeCompact.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.Copyright.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.DataInfo.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Boxes/BmffBox.DataReference.cs`.
