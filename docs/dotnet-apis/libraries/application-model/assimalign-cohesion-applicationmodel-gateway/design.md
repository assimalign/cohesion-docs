# Assimalign.Cohesion.ApplicationModel.Gateway design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Readiness and required command application gate dependent startup. Stop preserves declarations,
whereas teardown removes owned commands and resources in reverse order. Command clients receive the
target application's certificate validator; replacing a client must preserve that trust boundary.

## Factory implementation migration (#1465)

Composition uses the public sealed IdentityModel/DataProtection implementations sanctioned by
owner decision 59. Token audience checks use IdentityModel.Token's `HasAudience` extension;
where the caller previously accepted null or blank audiences, an ordinal audience-list match
preserves those inputs locally. Existing validation guards remain in place. This package's
public API and behavior are unchanged from 10.0.0-preview.1. Its updated dependency creation
signatures require rebuilding; the source and binary breaks are recorded in the JWT and
DataProtection package designs on decision 15's terms. Advisory: not applicable: API shape.

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
