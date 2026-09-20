# Assimalign.Cohesion.Hosting.Resources design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting.Resources`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Resources composes the Hosting run seam and Health vocabulary without reverse dependencies. Resource
SDKs opt into generated `ResourceRuntime` calls; assembly presence alone does not activate
supervision. Invocation context owns environment snapshots, mounts, control planes, and health
aggregation.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Hosting`,
`Assimalign.Cohesion.Hosting.Health`, `System.Security.Cryptography.ProtectedData`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Assimalign.Cohesion.Hosting.Resources.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src`.
