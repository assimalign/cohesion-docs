# Assimalign.Cohesion.Content.Bmff design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Content.Bmff`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Explicit box factories separate container structure from codecs. Many box and visitor operations
remain unimplemented, and the reader's unknown-box error is not a stable malformed-input contract.
Disposing the default reader closes its stream; the media fixture test requires a developer-local
file.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Content.Media`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src/Assimalign.Cohesion.Content.Bmff.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Content/README.md`.

- **Source** — `cohesion/libraries/Content/Assimalign.Cohesion.Content.Bmff/src`.
