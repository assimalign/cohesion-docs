# Assimalign.Cohesion.Http.Forms design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Forms`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Form parsing is an opt-in application concern above the raw protocol body. The feature owns parsing,
limits, and temporary-file spill behavior. Keeping these operations out of the protocol core avoids
imposing form lifecycle costs on clients and proxies.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Assimalign.Cohesion.Http.Forms.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src`.
