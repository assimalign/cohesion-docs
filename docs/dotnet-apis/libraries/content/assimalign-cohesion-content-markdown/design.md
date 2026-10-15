# Assimalign.Cohesion.Content.Markdown design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Markdown`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Block and inline parsing share the text tokenizer. Unsupported syntax degrades to literal text
rather than expanding the accepted language implicitly. `MarkdownText` supplies the facade and
reader/writer seams; document nodes remain mutable for transformations before canonical writing.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content`, `Assimalign.Cohesion.Content.Text`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src/Assimalign.Cohesion.Content.Markdown.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Markdown/src`.
