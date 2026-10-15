# Assimalign.Cohesion.Content.Markdown

Parses and writes a documented CommonMark subset and renders its document model as HTML.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Content](../index.md)

## Scope

Block and inline parsing share the text tokenizer. Unsupported syntax degrades to literal text
rather than expanding the accepted language implicitly. `MarkdownText` supplies the facade and
reader/writer seams; document nodes remain mutable for transformations before canonical writing.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Content`](../../content/assimalign-cohesion-content/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Content.Text`](../../content/assimalign-cohesion-content-text/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `MarkdownDocument` | `src/Nodes/MarkdownDocument.cs` |
| `MarkdownText` | `src/MarkdownText.cs` |
| `MarkdownThematicBreak` | `src/Nodes/MarkdownThematicBreak.cs` |
| `MarkdownBlock` | `src/Nodes/MarkdownBlock.cs` |
| `MarkdownBlockQuote` | `src/Nodes/MarkdownBlockQuote.cs` |
| `MarkdownCodeBlock` | `src/Nodes/MarkdownCodeBlock.cs` |
| `MarkdownCodeSpan` | `src/Nodes/MarkdownCodeSpan.cs` |
| `MarkdownEmphasis` | `src/Nodes/MarkdownEmphasis.cs` |
| `MarkdownHeading` | `src/Nodes/MarkdownHeading.cs` |
| `MarkdownImage` | `src/Nodes/MarkdownImage.cs` |
| `MarkdownInline` | `src/Nodes/MarkdownInline.cs` |
| `MarkdownLineBreak` | `src/Nodes/MarkdownLineBreak.cs` |
| `MarkdownLink` | `src/Nodes/MarkdownLink.cs` |
| `MarkdownList` | `src/Nodes/MarkdownList.cs` |
| `MarkdownListItem` | `src/Nodes/MarkdownListItem.cs` |
| `MarkdownLiteral` | `src/Nodes/MarkdownLiteral.cs` |

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Assimalign.Cohesion.Content.Markdown.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownDocument.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/MarkdownText.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownThematicBreak.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownBlock.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownBlockQuote.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownCodeBlock.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownCodeSpan.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownEmphasis.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownHeading.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownImage.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownInline.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownLineBreak.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownLink.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownList.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownListItem.cs`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Nodes/MarkdownLiteral.cs`.
