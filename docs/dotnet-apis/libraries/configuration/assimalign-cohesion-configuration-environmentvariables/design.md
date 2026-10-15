# Assimalign.Cohesion.Configuration.EnvironmentVariables design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.EnvironmentVariables`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Provider options describe filtering rather than taking over process-environment ownership. The
provider enumerates values and projects their names onto configuration keys; builder extensions use
the same registration seam as other sources.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/Assimalign.Cohesion.Configuration.EnvironmentVariables.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src`.
