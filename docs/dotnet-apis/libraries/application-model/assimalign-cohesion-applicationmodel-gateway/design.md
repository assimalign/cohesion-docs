# Assimalign.Cohesion.ApplicationModel.Gateway design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Readiness and required command application gate dependent startup. Stop preserves declarations,
whereas teardown removes owned commands and resources in reverse order. Command clients receive the
target application's certificate validator; replacing a client must preserve that trust boundary.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.ApplicationModel`,
`Assimalign.Cohesion.ConfigurationStore.Client`, `Assimalign.Cohesion.Database.Client`,
`Assimalign.Cohesion.Hosting.Resources`, `Assimalign.Cohesion.IdentityHub.Client`,
`Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`, `Assimalign.Cohesion.Rezolvr.Client`,
`Assimalign.Cohesion.SecretStore.Client`, `Assimalign.Cohesion.Security.DataProtection`,
`System.Security.Cryptography.ProtectedData`. The [overview](index.md#dependencies) distinguishes
project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Assimalign.Cohesion.ApplicationModel.Gateway.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src`.
