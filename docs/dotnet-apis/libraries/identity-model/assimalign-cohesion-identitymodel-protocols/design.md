# Assimalign.Cohesion.IdentityModel.Protocols design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Protocols`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Shared roles, bindings, endpoint kinds, parties, and validation results prevent each protocol branch
from inventing incompatible foundations. Protocol response status is fail-closed. Concrete branches
supply protocol-specific messages without taking transport ownership.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel`,
`Assimalign.Cohesion.IdentityModel`, `Assimalign.Cohesion.IdentityModel.Protocols`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/Assimalign.Cohesion.IdentityModel.Protocols.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src`.
