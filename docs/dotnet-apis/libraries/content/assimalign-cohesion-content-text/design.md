# Assimalign.Cohesion.Content.Text design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Text`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Encoding detection inspects a small byte prefix; text content wraps the root content ownership
model. Line readers preserve line endings, while the stack-only tokenizer records positions and
returns slices of character sequences. Format parsers supply their own token definitions.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src/Assimalign.Cohesion.Content.Text.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Text/src`.
