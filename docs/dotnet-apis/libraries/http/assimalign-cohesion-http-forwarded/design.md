# Assimalign.Cohesion.Http.Forwarded design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Forwarded`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The feature records effective values, original wire values, and trusted hop count. Extension members
read the feature first and fall back to wire values. Trust policy and header interpretation belong
to producers of the feature, not to consumers repeating their own parsing.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src/Assimalign.Cohesion.Http.Forwarded.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src`.
