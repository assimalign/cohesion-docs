# Assimalign.Cohesion.ObjectValidation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectValidation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Profiles separate rule declaration from execution. Member selectors are inspected through resolved
metadata instead of compiling expressions, while conditional predicates are supplied as delegates.
Options control failure aggregation and throwing without changing how profiles are authored.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src/Assimalign.Cohesion.ObjectValidation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectValidation/Assimalign.Cohesion.ObjectValidation/src`.
