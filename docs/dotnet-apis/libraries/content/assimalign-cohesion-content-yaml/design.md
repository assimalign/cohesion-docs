# Assimalign.Cohesion.Content.Yaml design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Yaml`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The document model, event pipeline, parser, and emitter are separated behind `YamlText`. Scalar
resolution follows the core schema, while writing chooses stable block output and reconstructs
anchors and aliases for shared nodes. Text encoding detection is delegated to the text layer.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content`, `Assimalign.Cohesion.Content.Text`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src/Assimalign.Cohesion.Content.Yaml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Yaml/src`.
