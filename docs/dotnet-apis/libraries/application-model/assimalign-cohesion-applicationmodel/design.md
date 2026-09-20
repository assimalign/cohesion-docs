# Assimalign.Cohesion.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Descriptors own graph identity, and `Build()` validates dependencies and commands before
realization. Commands contain portable desired state, so payloads must not contain secrets. Area
packages add typed composition verbs without moving resource runtimes into the base model.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src/Assimalign.Cohesion.ApplicationModel.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/src`.
