# Assimalign.Cohesion.Content.Text

Provides text encoding detection, line reading, and configurable tokenization.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

Encoding detection inspects a small byte prefix; text content wraps the root content ownership
model. Line readers preserve line endings, while the stack-only tokenizer records positions and
returns slices of character sequences. Format parsers supply their own token definitions.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content`](../../content/assimalign-cohesion-content/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ITextContent` | `src/Abstractions/ITextContent.cs` |
| `TextContentFactory` | `src/TextContentFactory.cs` |
| `TextEncodingDetector` | `src/TextEncodingDetector.cs` |
| `TextLine` | `src/TextLine.cs` |
| `TextLineEnding` | `src/TextLineEnding.cs` |
| `TextLineReader` | `src/TextLineReader.cs` |
| `TextPosition` | `src/TextPosition.cs` |
| `TextToken` | `src/TextToken.cs` |
| `TextTokenDefinition` | `src/TextTokenDefinition.cs` |
| `TextTokenizer` | `src/TextTokenizer.cs` |
| `TextTokenizerOptions` | `src/TextTokenizerOptions.cs` |
| `TextTokenKind` | `src/TextTokenKind.cs` |
| `TextEncodingDetection` | `src/TextEncodingDetection.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/Assimalign.Cohesion.Content.Text.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/Abstractions/ITextContent.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextContentFactory.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextEncodingDetector.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextLine.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextLineEnding.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextLineReader.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextPosition.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextToken.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextTokenDefinition.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextTokenizer.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextTokenizerOptions.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextTokenKind.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/TextEncodingDetection.cs`.
