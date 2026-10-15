# Example: Parse, render, and rewrite Markdown

Parse a Markdown document, render it, and modify its node tree.

[Examples](index.md) · [Assembly overview](../index.md)

## Code

```csharp
using Assimalign.Cohesion.Content.Markdown;

var document = MarkdownText.Parse("# Title\n\nBody with *emphasis* and [a link](/docs).");

// Render for serving.
var html = MarkdownText.ToHtml(document);

// Transform the tree and write canonical Markdown back out.
document.Blocks.Insert(0, new MarkdownThematicBreak());
var markdown = MarkdownText.Write(document);
```

## Walkthrough

`Parse` creates the document model and `ToHtml` renders that model. Inserting a
`MarkdownThematicBreak` changes the tree before `Write` emits canonical Markdown.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/OVERVIEW.md`.
