# Assimalign.Cohesion.Content.Ebml

Contains the retained Extensible Binary Meta Language (EBML) document and element implementation.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

The project contains document, element, stream, and variable-integer types without a separate
overview or design record. The area README identifies the implementation as requiring repair. Its
current project file has no Cohesion references, so the intended root-contract integration is not an
implemented dependency.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `EbmlBody` | `src/EbmlBody.cs` |
| `EbmlDataFormatException` | `src/Exceptions/EbmlDataFormatException.cs` |
| `EbmlDocument` | `src/EbmlDocument.cs` |
| `EbmlElement` | `src/Elements/EbmlElement.cs` |
| `EbmlElementType` | `src/EbmlElementType.cs` |
| `EbmlHeader` | `src/EbmlHeader.cs` |
| `EbmlStream` | `src/EbmlStream.cs` |
| `VInt` | `src/VInt.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/Assimalign.Cohesion.Content.Ebml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/EbmlBody.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/Exceptions/EbmlDataFormatException.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/EbmlDocument.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/Elements/EbmlElement.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/EbmlElementType.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/EbmlHeader.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/EbmlStream.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Ebml/src/VInt.cs`.
